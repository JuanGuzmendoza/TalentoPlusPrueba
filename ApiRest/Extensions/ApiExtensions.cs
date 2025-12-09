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
            // 1. Controladores
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            // 2. Configurar Identity y Auth (API Endpoints + Roles)
            services.AddAuthorization();

            services.AddIdentityApiEndpoints<User>(options =>
            {
                // 1. RELAJAR POLITICAS DE PASSWORD (Para que acepte cédulas)
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false; // Permite sin simbolos (!@#)
                options.Password.RequiredLength = 6; // Minimo 6 caracteres

                // 2. PERMITIR CARACTERES EN EL USUARIO (Para tildes en correos)
                options.User.AllowedUserNameCharacters = 
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ñÑáéíóúÁÉÍÓÚ";
    
                // 3. VALIDACION DE EMAIL UNICA
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