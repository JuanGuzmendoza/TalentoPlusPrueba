using TalentoPlus.Domain.Entities;
using TalentoPlus.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;

namespace TalentoPlus.Extensions 
{
    public static class ApiExtensions
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
        {
          
            services.AddControllers();
            services.AddEndpointsApiExplorer();

 
            services.AddAuthorization();

            services.AddIdentityApiEndpoints<User>(options =>
            {

                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false; 
                options.Password.RequiredLength = 6; 
                options.User.AllowedUserNameCharacters = 
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ñÑáéíóúÁÉÍÓÚ";
    
                options.User.RequireUniqueEmail = true; 
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

            return services;
        }

        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Ingresa el token JWT",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new string[] {}
                    }
                });
            });

            return services;
        }
    }
}