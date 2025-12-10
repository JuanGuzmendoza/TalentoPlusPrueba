using Microsoft.AspNetCore.Mvc;
using TalentoPlus.Application.Interfaces;

[Route("api/auth")] 
[ApiController] 
public class AuthController : ControllerBase
{
    private readonly IEmployeeService _employeeService;

    
    public AuthController(IEmployeeService employeeService)
    {
        _employeeService = employeeService;
    }


    [HttpPost("register")] 
    public async Task<IActionResult> RegisterEmpleado([FromBody] EmpleadoRegistroDto model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var (employee, errors) = await _employeeService.RegisterEmployeeAsync(model);

        if (employee != null)
        {
            return Ok(new 
            { 
                Message = "Empleado y usuario registrados con éxito.", 
                EmployeeId = employee.Id,
                UserId = employee.UserId
            });
        }
        else
        {
            return BadRequest(new 
            { 
                Message = "Fallo en el registro del empleado. Revise los errores.", 
                Errors = errors 
            });
        }
    }
    

}