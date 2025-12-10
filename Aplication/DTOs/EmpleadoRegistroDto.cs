using System.ComponentModel.DataAnnotations;

public class EmpleadoRegistroDto
{
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    // Campos para el Employee
    [Required]
    public string DocumentNumber { get; set; }

    [Required]
    public string FirstName { get; set; } 

    [Required]
    public string LastName { get; set; }  
    

    [Required]
    public DateTime BirthDate { get; set; }

    public string Address { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    [Required]
    public string Position { get; set; } = string.Empty;

    public decimal Salary { get; set; }
    
    public DateTime HireDate { get; set; } 


    public string EducationLevel { get; set; } = string.Empty;

    [MaxLength(500)] 
    public string ProfessionalProfile { get; set; } = string.Empty;

    [Required]
    public int DepartmentId { get; set; }
    
}