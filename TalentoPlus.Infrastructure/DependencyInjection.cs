using TalentoPlus.Domain.Interfaces;
using TalentoPlus.Infrastructure.Persistence.Context;
using TalentoPlus.Infrastructure.Repositories;
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

            // 2. INYECCIÓN DEL REPOSITORIO GENÉRICO
            // Esto permite inyectar IGenericRepository<Employee> o IGenericRepository<Department>
            // sin tener que crear clases específicas.
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            return services;
        }
    }
}