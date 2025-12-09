using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Text;
using System.Text.Json; // Necesario para serializar el JSON
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;
using TalentoPlus.Domain.Interfaces;

namespace TalentoPlus.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IGenericRepository<Employee> _employeeRepository;
        private readonly IGenericRepository<Department> _departmentRepository;
        private readonly UserManager<User> _userManager;
        private readonly IHttpClientFactory _httpClientFactory; // Nuevo servicio

        public EmployeeService(
            IGenericRepository<Employee> employeeRepository,
            IGenericRepository<Department> departmentRepository,
            UserManager<User> userManager,
            IHttpClientFactory httpClientFactory) // Inyección del Factory
        {
            _employeeRepository = employeeRepository;
            _departmentRepository = departmentRepository;
            _userManager = userManager;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Employee> CreateEmployeeAsync(Employee employee)
        {
            await _employeeRepository.AddAsync(employee);
            return employee;
        }

        public async Task ImportEmployeesFromExcelAsync(IFormFile file)
        {
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);

                using (var workbook = new XLWorkbook(stream))
                {
                    var worksheet = workbook.Worksheet(1);
                    var rows = worksheet.RangeUsed().RowsUsed().Skip(1);

                    var listaEmpleados = new List<Employee>();

                    foreach (var row in rows)
                    {
                        try
                        {
                            // 1. LECTURA DE DATOS
                            string nombreDepto = row.Cell(14).GetValue<string>().Trim();
                            if (string.IsNullOrWhiteSpace(nombreDepto)) continue;

                            string documento = row.Cell(1).GetValue<string>().Trim();
                            string firstName = row.Cell(2).GetValue<string>().Trim();
                            string lastName = row.Cell(3).GetValue<string>().Trim();
                            string email = row.Cell(7).GetValue<string>().Trim();

                            // 2. LOGICA DEPARTAMENTO
                            var departamento = await _departmentRepository
                                .FirstOrDefaultAsync(d => d.Name == nombreDepto);

                            if (departamento == null)
                            {
                                departamento = new Department { Name = nombreDepto };
                                await _departmentRepository.AddAsync(departamento);
                            }

                            // 3. LOGICA FECHAS
                            DateTime birthDate = DateTime.SpecifyKind(new DateTime(1900, 1, 1), DateTimeKind.Utc);
                            string birthStr = row.Cell(4).GetValue<string>().Trim().Replace("'", "");
                            if (DateTime.TryParse(birthStr, out DateTime birthParsed))
                                birthDate = DateTime.SpecifyKind(birthParsed, DateTimeKind.Utc);

                            DateTime hireDate = DateTime.UtcNow;
                            string hireStr = row.Cell(10).GetValue<string>().Trim().Replace("'", "");
                            if (DateTime.TryParse(hireStr, out DateTime hireParsed))
                                hireDate = DateTime.SpecifyKind(hireParsed, DateTimeKind.Utc);

                            // 4. PARSEOS
                            decimal.TryParse(row.Cell(9).GetValue<string>().Trim(), out decimal salario);
                            string estadoStr = row.Cell(11).GetValue<string>().Trim();
                            if (!Enum.TryParse<EmployeeStatus>(estadoStr, true, out var estadoEnum))
                                estadoEnum = EmployeeStatus.Activo;

                            // 5. USUARIO IDENTITY
                            string? userIdAsignado = null;
                            var existingUser = await _userManager.FindByEmailAsync(email);
                            bool isNewUser = false; // Bandera para saber si enviamos correo

                            if (existingUser == null)
                            {
                                var newUser = new User
                                {
                                    UserName = email,
                                    Email = email,
                                    FirstName = firstName,
                                    LastName = lastName,
                                    EmailConfirmed = true
                                };

                                var result = await _userManager.CreateAsync(newUser, documento);

                                if (result.Succeeded)
                                {
                                    userIdAsignado = newUser.Id;
                                    await _userManager.AddToRoleAsync(newUser, "User");
                                    isNewUser = true; // ¡Usuario creado exitosamente!
                                }
                                else
                                {
                                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                                    Console.WriteLine($"Error user: {errors}");
                                }
                            }
                            else
                            {
                                userIdAsignado = existingUser.Id;
                            }

                            // =========================================================
                            // 5.1 ENVÍO DE CORREO (SOLO SI ES NUEVO)
                            // =========================================================
                            if (isNewUser)
                            {
                                // No usamos 'await' para no bloquear la importación masiva si el correo tarda,
                                // o usamos 'await' si es crítico que se envíe. Aquí uso await seguro.
                                try 
                                {
                                    string fullName = $"{firstName} {lastName}";
                                    await SendWelcomeEmailAsync(fullName, email);
                                }
                                catch (Exception exMail)
                                {
                                    Console.WriteLine($"Error enviando correo a {email}: {exMail.Message}");
                                    // No lanzamos throw para que siga importando el empleado a la BD
                                }
                            }
                            // =========================================================

                            // 6. CREACIÓN EMPLEADO
                            var nuevoEmpleado = new Employee
                            {
                                Id = Guid.NewGuid(),
                                DocumentNumber = documento,
                                FirstName = firstName,
                                LastName = lastName,
                                BirthDate = birthDate,
                                Address = row.Cell(5).GetValue<string>().Trim(),
                                Phone = row.Cell(6).GetValue<string>(),
                                Email = email,
                                Position = row.Cell(8).GetValue<string>().Trim(),
                                Salary = salario,
                                HireDate = hireDate,
                                Status = estadoEnum,
                                EducationLevel = row.Cell(12).GetValue<string>(),
                                ProfessionalProfile = row.Cell(13).GetValue<string>().Trim(),
                                DepartmentId = departamento.Id,
                                UserId = userIdAsignado
                            };

                            listaEmpleados.Add(nuevoEmpleado);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Fila {row.RowNumber()}: {ex.Message}");
                        }
                    }

                    if (listaEmpleados.Any())
                    {
                        await _employeeRepository.AddRangeAsync(listaEmpleados);
                    }
                }
            }
        }

        // =========================================================================
        // MÉTODO PRIVADO PARA LLAMAR AL APPSCRIPT DE GOOGLE
        // =========================================================================
        private async Task SendWelcomeEmailAsync(string name, string email)
        {
            var client = _httpClientFactory.CreateClient();
            
            // URL de tu App Script
            var url = "https://script.google.com/macros/s/AKfycbweQdP0TxXh02tmmULqEgXuhmFWCaClrdWmLCXHODhGe3xOwf4-lDjyJwjjaCsXg_PeLg/exec";

            // Creamos el objeto anónimo con los datos que espera tu script:
            // Tu script usa: data.employeeName y data.employeeEmail
            var payload = new
            {
                employeeName = name,
                employeeEmail = email 
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );

            // Google AppScript a veces hace redirecciones (302), HttpClient las sigue automáticamente por defecto.
            var response = await client.PostAsync(url, jsonContent);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error al llamar al AppScript. Status: {response.StatusCode}");
            }
        }
    }
}