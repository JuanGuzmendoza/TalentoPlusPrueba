using System.ComponentModel.DataAnnotations;

namespace TalentoPlus.Domain.Entities
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Relación: Un departamento tiene muchos empleados
        public ICollection<Employee>? Employees { get; set; }
    }
}