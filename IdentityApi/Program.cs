using IdentityApi;
using IdentityApi.Models;
using IdentityApi.Models.Interface;
using IdentityApi.Models.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });
    // Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();

string conStrig = builder.Configuration.GetConnectionString("conString");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(conStrig));
builder.Services.Configure<IdentityOptions>(options => {
    options.SignIn.RequireConfirmedEmail = true;
    options.User.RequireUniqueEmail = true;
}); //For Cutom Authorization or set identity options.
builder.Services.AddIdentity<CustomUser,IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();
var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
await db.Database.MigrateAsync();
var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
string[] roleNames = { "Admin", "User" };
foreach (var roleName in roleNames)
{
    var roleExists = await roleManager.RoleExistsAsync(roleName);
    if (!roleExists)
    {
        await roleManager.CreateAsync(new IdentityRole(roleName));
    }
}

var userManager = scope.ServiceProvider.GetRequiredService<UserManager<CustomUser>>();
var adminUser = await userManager.FindByNameAsync("admin");
if (adminUser == null)
{
    adminUser = new CustomUser
    {
        UserName = "admin",
        FullName = "admin",
        Email = "admin@domain.com",
        EmailConfirmed = true,
        PhoneNumberConfirmed = true,
        LockoutEnabled = false
    };
    var result = await userManager.CreateAsync(adminUser, "Adminpassword@123");
    if (result.Succeeded)
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
