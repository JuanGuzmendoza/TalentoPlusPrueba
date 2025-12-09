using TalentoPlus.Domain.Entities;
using TalentoPlus.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TalentoPlus.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Configurar DB Context
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            // 2. Configurar el almacenamiento de Identity (Solo la parte de datos)
            // Nota: La parte de "AddIdentityApiEndpoints" es de la capa de API, 
            // pero la conexión con EF Core va aquí.

            return services;
        }
    }
}