using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using YouTubeClone.Domain.Data;
using YouTubeClone.Domain.Data.Seeder;
using YouTubeClone.Domain.Interfaces;
using YouTubeClone.Domain.Repositories;
using YouTubeClone.Domain.Services;
using YouTubeClone.Shared.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuration: SeedSettings ကို appsettings.json ကနေ ဖတ်ပြီး Singleton အဖြစ် Register လုပ်မယ်
var seedSettings = builder.Configuration.GetSection("SeedSettings").Get<SeedSettings>()
                   ?? throw new InvalidOperationException("SeedSettings is missing!");
builder.Services.AddSingleton(seedSettings);

// JwtSettings ကို appsettings.json ကနေ Bind လုပ်ပြီး Register လုပ်ခြင်း
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings is missing!");
builder.Services.AddSingleton(jwtSettings);

// Authentication Service ကို JWT Bearer သုံးပြီး Configure လုပ်ခြင်း
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
    };
});

builder.Services.AddAuthorization();

// Add services to the container
builder.Services.AddControllers()
    .AddApplicationPart(typeof(AppDbContext).Assembly);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Unit of Work and Seeder
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IDbSeeder, DbSeeder>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddOpenApi();

var app = builder.Build();

// Database Seeding Execution: App စတက်တာနဲ့ တစ်ခါတည်း Run ခိုင်းမယ်
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<IDbSeeder>();
    try
    {
        await seeder.SeedAsync();
        Console.WriteLine("Database Seeding Successful!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Seeding Failed: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
