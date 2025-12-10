using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Application.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);
        Task<Department> CreateAsync(Department department);
        Task UpdateAsync(Department department);
        Task DeleteAsync(int id);
        Task<int> GetOrCreateIdByNameAsync(string name);
    }
}