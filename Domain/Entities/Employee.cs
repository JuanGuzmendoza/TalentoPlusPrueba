using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Domain.Entities
{
    public class Employee
    {
        public Guid Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string DocumentNumber { get; set; } = string.Empty; // Documento

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty; // Nombres

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;  // Apellidos

        public DateTime BirthDate { get; set; } // FechaNacimiento

        [MaxLength(200)]
        public string Address { get; set; } = string.Empty; // Direccion

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public string Position { get; set; } = string.Empty; // Cargo

        [Column(TypeName = "decimal(18,2)")]
        public decimal Salary { get; set; }

        public DateTime HireDate { get; set; } 

        public EmployeeStatus Status { get; set; } = EmployeeStatus.Activo;

        public string EducationLevel { get; set; } = string.Empty; 

        [MaxLength(500)] 
        public string ProfessionalProfile { get; set; } = string.Empty; 


        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public string? UserId { get; set; }
        public User? User { get; set; }
    }
}