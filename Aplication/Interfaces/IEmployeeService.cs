using Microsoft.AspNetCore.Http;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Application.Interfaces
{
    public interface IEmployeeService
    {
        // Método para cargar masivamente desde Excel
        Task ImportEmployeesFromExcelAsync(IFormFile file);

        // Método para crear un solo empleado manual (el otro POST)
        Task<Employee> CreateEmployeeAsync(Employee employee);
    }
}