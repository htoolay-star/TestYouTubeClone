using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using YouTubeClone.Shared.Constants;
using YouTubeClone.Domain.Data;
using YouTubeClone.Domain.Data.Seeder;
using YouTubeClone.Domain.Interfaces;
using YouTubeClone.Domain.Mappings;
using YouTubeClone.Domain.Repositories;
using YouTubeClone.Domain.Services;
using YouTubeClone.Shared.DTOs;
using YouTubeClone.Shared.Models;
using YouTubeClone.Shared.Validators;

namespace YouTubeCloneAPI.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            var redisConnectionString = config.GetConnectionString("RedisConnection");
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnectionString;
                options.InstanceName = "HR_PMS_";
            });

            services.AddSingleton(config.GetSection("SeedSettings").Get<SeedSettings>()
                ?? throw new InvalidOperationException(AuthMessages.System.SeedSettingsMissing));
            services.AddSingleton(config.GetSection("Mailtrap").Get<EmailSettings>()
                ?? throw new InvalidOperationException(AuthMessages.System.EmailSettingsMissing));

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IDbSeeder, DbSeeder>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IFileService, FileService>();

            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(MappingProfile).Assembly));

            services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
            services.AddFluentValidationAutoValidation();

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var errors = context.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray());

                    return new BadRequestObjectResult(new ErrorResponse(400, AuthMessages.System.ValidationFailed, errors));
                };
            });

            return services;
        }
    }
}
