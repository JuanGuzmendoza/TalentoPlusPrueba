using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace TalentoPlus.Domain.Entities
{
    public class Department
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [JsonIgnore] 
        public ICollection<Employee>? Employees { get; set; }
    }
}