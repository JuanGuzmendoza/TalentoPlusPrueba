using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IGenericRepository<Department> _departmentRepository;

        public DepartmentService(IGenericRepository<Department> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _departmentRepository.GetAllAsync();
        }

        public async Task<Department?> GetByIdAsync(int id)
        {
            return await _departmentRepository.GetByIdAsync(id);
        }

        public async Task<Department> CreateAsync(Department department)
        {
            await _departmentRepository.AddAsync(department);
            return department;
        }

        public async Task UpdateAsync(Department department)
        {
            await _departmentRepository.UpdateAsync(department);
        }

        public async Task DeleteAsync(int id)
        {
            await _departmentRepository.DeleteAsync(id);
        }

        public async Task<int> GetOrCreateIdByNameAsync(string name)
        {
            var cleanName = name.Trim();
            
            var departamento = await _departmentRepository.FirstOrDefaultAsync(d => d.Name == cleanName);

            if (departamento == null)
            {
                departamento = new Department { Name = cleanName };
                await _departmentRepository.AddAsync(departamento);
            }

            return departamento.Id;
        }
    }
}