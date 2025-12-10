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
            services.AddScoped<IDepartmentService, DepartmentService>();

            services.AddScoped<IDateParserService, DateParserService>();
            services.AddScoped<IUserAccountService, UserAccountService>();
            services.AddScoped<IEmailNotificationService, EmailNotificationService>();

            return services;
        }
    }
}