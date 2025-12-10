using Microsoft.AspNetCore.Http;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task ImportEmployeesFromExcelAsync(IFormFile file);
        Task<(Employee? employee, IEnumerable<string>? errors)> RegisterEmployeeAsync(EmpleadoRegistroDto dto);
        Task<Employee> CreateEmployeeAsync(Employee employee);

        Task<IEnumerable<Employee>> GetAllEmployeesAsync(); 
        Task<Employee?> GetEmployeeByIdAsync(Guid id); 
        Task<Employee> UpdateEmployeeAsync(Employee employee); 
        Task<bool> DeleteEmployeeAsync(Guid id); 
    }
}