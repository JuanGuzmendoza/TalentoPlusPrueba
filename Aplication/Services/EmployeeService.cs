// TalentoPlus.Application.Services/EmployeeService.cs

using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IGenericRepository<Employee> _employeeRepository;
        
        // Servicios Auxiliares
        private readonly IDepartmentService _departmentService; 
        private readonly IDateParserService _dateParser;
        private readonly IEmailNotificationService _emailService;
        private readonly IUserAccountService _userAccountService;

        public EmployeeService(
            IGenericRepository<Employee> employeeRepository,
            IDepartmentService departmentService,
            IDateParserService dateParser,
            IEmailNotificationService emailService,
            IUserAccountService userAccountService)
        {
            _employeeRepository = employeeRepository;
            _departmentService = departmentService;
            _dateParser = dateParser;
            _emailService = emailService;
            _userAccountService = userAccountService;
        }

        public async Task<(Employee? employee, IEnumerable<string>? errors)> RegisterEmployeeAsync(EmpleadoRegistroDto dto)
        {
            var (userId, succeeded, errors, isNew) = await _userAccountService.RegisterOrRetrieveUserAsync(
                email: dto.Email,
                password: dto.DocumentNumber, 
                firstName: dto.FirstName,
                lastName: dto.LastName,
                role: "User"
            );

            if (!succeeded)
            {
                return (null, errors); 
            }
            var employee = new Employee
            {
                DocumentNumber = dto.DocumentNumber,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                BirthDate = dto.BirthDate,
                Address = dto.Address,
                Email = dto.Email,
                Phone = dto.Phone,
                Position = dto.Position,
                Salary = dto.Salary,
                HireDate = dto.HireDate, 
                Status = EmployeeStatus.Activo, 
                EducationLevel = dto.EducationLevel,
                ProfessionalProfile = dto.ProfessionalProfile,
                DepartmentId = dto.DepartmentId,
        
                UserId = userId!
            };
    
            await _employeeRepository.AddAsync(employee);

            return (employee, null); 
        }
         
         
        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            // Nota: Aquí podrías añadir lógica para crear el User si lo haces por separado
            await _employeeRepository.AddAsync(employee);
            return employee;
        }

        public async Task ImportEmployeesFromExcelAsync(IFormFile file)
        {
            // ... (Tu implementación existente de ImportEmployeesFromExcelAsync) ...
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

            var listaEmpleados = new List<Employee>();

            foreach (var row in rows)
            {
                try
                {
                    string nombreDepto = row.Cell(14).GetValue<string>().Trim();
                    if (string.IsNullOrWhiteSpace(nombreDepto)) continue;

                    string documento = row.Cell(1).GetValue<string>().Trim();
                    string firstName = row.Cell(2).GetValue<string>().Trim();
                    string lastName = row.Cell(3).GetValue<string>().Trim();
                    string email = row.Cell(7).GetValue<string>().Trim();

                    var deptoId = await _departmentService.GetOrCreateIdByNameAsync(nombreDepto);

                    var userResult = await _userAccountService.RegisterOrRetrieveUserAsync(email, firstName, lastName, documento , "User");


                    var birthDate = _dateParser.ParseDate(row.Cell(4).GetValue<string>(), new DateTime(1900, 1, 1));
                    var hireDate = _dateParser.ParseDate(row.Cell(10).GetValue<string>(), DateTime.UtcNow);

                    var nuevoEmpleado = new Employee
                    {
                        Id = Guid.NewGuid(),
                        DocumentNumber = documento,
                        FirstName = firstName,
                        LastName = lastName,
                        BirthDate = birthDate,
                        HireDate = hireDate,
                        Email = email,
                        Address = row.Cell(5).GetValue<string>().Trim(),
                        Phone = row.Cell(6).GetValue<string>(),
                        Position = row.Cell(8).GetValue<string>().Trim(),
                        EducationLevel = row.Cell(12).GetValue<string>(),
                        ProfessionalProfile = row.Cell(13).GetValue<string>().Trim(),
                        DepartmentId = deptoId, // Usamos el ID obtenido del servicio
                        UserId = userResult.UserId,
                        Status = ParseStatus(row.Cell(11).GetValue<string>()),
                        Salary = ParseSalary(row.Cell(9).GetValue<string>())
                    };

                    listaEmpleados.Add(nuevoEmpleado);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fila {row.RowNumber()}: {ex.Message}");
                }
            }

            if (listaEmpleados.Any())
            {
                await _employeeRepository.AddRangeAsync(listaEmpleados);
            }
        }


        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _employeeRepository.GetAllAsync(e => e.Department!, e => e.User!);
        }

        public async Task<Employee?> GetEmployeeByIdAsync(Guid id)
        {
            return await _employeeRepository.FirstOrDefaultAsync(e => e.Id == id, e => e.Department!, e => e.User!);
        }

        public async Task<Employee> UpdateEmployeeAsync(Employee employee)
        {
            await _employeeRepository.UpdateAsync(employee);
            return employee;
        }

        public async Task<bool> DeleteEmployeeAsync(Guid id)
        {
            var employee = await _employeeRepository.GetByIdAsync(id);
    
            // 2. Eliminar la entidad Employee
            var exists = await _employeeRepository.ExistsAsync(id);
            if (!exists) return false;
            
            await _employeeRepository.DeleteAsync(id);
            return true;
        }


        private EmployeeStatus ParseStatus(string statusStr)
        {
            if (Enum.TryParse<EmployeeStatus>(statusStr.Trim(), true, out var result))
                return result;
            return EmployeeStatus.Activo;
        }

        private decimal ParseSalary(string salaryStr)
        {
            if (decimal.TryParse(salaryStr.Trim(), out decimal result))
                return result;
            return 0;
        }
    }
}