using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Firmeza.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestRolesController : ControllerBase
    {
        // 1. ENDPOINT PÚBLICO
        // Cualquiera puede entrar aquí, con o sin token.
        [HttpGet("publico")]
        public IActionResult EndpointPublico()
        {
            return Ok(new
            {
                Mensaje = "Soy público. No necesitas estar logueado para verme.",
                Fecha = DateTime.Now
            });
        }

        // 2. ENDPOINT SOLO PARA ADMIN
        // Solo usuarios que tengan el rol "Admin"
        [HttpGet("solo-admin")]
        [Authorize(Roles = "Admin")]
        public IActionResult EndpointAdmin()
        {
            // Obtenemos el ID del usuario del token para mostrarlo
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.Identity?.Name;

            return Ok(new
            {
                Mensaje = "ACCESO CONCEDIDO: Eres Administrador.",
                UsuarioId = userId,
                Email = email,
                Rol = "Admin"
            });
        }

        // 3. ENDPOINT SOLO PARA USER
        // Solo usuarios que tengan el rol "User"
        [HttpGet("solo-user")]
        [Authorize(Roles = "User")]
        public IActionResult EndpointUser()
        {
            var email = User.Identity?.Name;

            return Ok(new
            {
                Mensaje = "ACCESO CONCEDIDO: Eres un Usuario estándar.",
                Email = email,
                Rol = "User"
            });
        }
    }
}