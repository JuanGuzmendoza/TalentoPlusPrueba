using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        // POST: api/Employees/import
        // Este endpoint recibe el archivo Excel
        [HttpPost("import")]
        public async Task<IActionResult> ImportExcel(IFormFile file)
        {
            // Validaciones básicas de entrada
            if (file == null || file.Length == 0)
                return BadRequest("Por favor sube un archivo Excel válido.");

            // Validar extensión (opcional pero recomendado)
            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("El archivo debe ser formato .xlsx");

            try
            {
                await _employeeService.ImportEmployeesFromExcelAsync(file);
                return Ok(new { Message = "Importación completada exitosamente." });
            }
            catch (Exception ex)
            {
                // Retornamos el error para que veas si falla algo en el Excel (ej: formato de fecha)
                return BadRequest(new { Error = ex.Message, Detail = ex.InnerException?.Message });
            }
        }

        // POST: api/Employees
        // Endpoint para crear uno manual (requerido por el CRUD básico)
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Employee employee)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var created = await _employeeService.CreateEmployeeAsync(employee);
            return CreatedAtAction(nameof(Create), new { id = created.Id }, created);
        }
    }
}