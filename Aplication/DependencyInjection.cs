using Microsoft.Extensions.DependencyInjection;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Application.Services;

namespace TalentoPlus.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IEmployeeService, EmployeeService>();

            
            return services;
        }
    }
}