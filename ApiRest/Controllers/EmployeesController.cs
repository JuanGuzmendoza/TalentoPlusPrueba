

using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Entities;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    public EmployeesController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }
    
    private object GetUserInfo()
    {
        return new
        {
            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
            Email = User.FindFirstValue(ClaimTypes.Email),
            Role = User.FindFirstValue(ClaimTypes.Role),
        };
    }

    // POST: api/Employees/import
    [HttpPost("import")]
    public async Task<IActionResult> ImportExcel(IFormFile file)
    {
        var userInfo = GetUserInfo();

        try
        {
            await _employeeService.ImportEmployeesFromExcelAsync(file);

            return Ok(new
            {
                Message = "Importación completada exitosamente.",
                Usuario = userInfo
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new
            {
                Error = ex.Message,
                Detail = ex.InnerException?.Message,
                Usuario = userInfo
            });
        }
    }

    // POST: api/Employees
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Employee employee)
    {
        var userInfo = GetUserInfo();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var created = await _employeeService.CreateEmployeeAsync(employee);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new
        {
            created,
            Usuario = userInfo
        });
    }

    // GET: api/Employees
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var employees = await _employeeService.GetAllEmployeesAsync();
        var userInfo = GetUserInfo();

        return Ok(new
        {
            employees,
            Usuario = userInfo
        });
    }

    // GET: api/Employees/{id}
    [HttpGet("{id:guid}", Name = "GetById")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userInfo = GetUserInfo();

        var employee = await _employeeService.GetEmployeeByIdAsync(id);

        if (employee == null)
            return NotFound(new
            {
                Message = $"Empleado con ID {id} no encontrado.",
                Usuario = userInfo
            });

        return Ok(new
        {
            employee,
            Usuario = userInfo
        });
    }

    // PUT: api/Employees/{id}
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Employee employee)
    {
        var userInfo = GetUserInfo();

        if (id != employee.Id)
            return BadRequest(new
            {
                Message = "El ID de la ruta no coincide con el ID del empleado.",
                Usuario = userInfo
            });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var updated = await _employeeService.UpdateEmployeeAsync(employee);
            return Ok(new
            {
                updated,
                Usuario = userInfo
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Error = "Error al actualizar el empleado.",
                Detail = ex.Message,
                Usuario = userInfo
            });
        }
    }

    // DELETE: api/Employees/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userInfo = GetUserInfo();

        var result = await _employeeService.DeleteEmployeeAsync(id);

        if (!result)
            return NotFound(new
            {
                Message = $"Empleado con ID {id} no encontrado.",
                Usuario = userInfo
            });

        return Ok(new
        {
            Message = "Empleado eliminado correctamente.",
            Usuario = userInfo
        });
    }
}
