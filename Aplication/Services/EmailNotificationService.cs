using System.Text;
using System.Text.Json;
using TalentoPlus.Application.Interfaces;

namespace TalentoPlus.Application.Services
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EmailNotificationService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task SendWelcomeEmailAsync(string name, string email)
        {
            var client = _httpClientFactory.CreateClient();
            var url = "https://script.google.com/macros/s/AKfycbweQdP0TxXh02tmmULqEgXuhmFWCaClrdWmLCXHODhGe3xOwf4-lDjyJwjjaCsXg_PeLg/exec";

            var payload = new { employeeName = name, employeeEmail = email };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(payload),
                Encoding.UTF8,
                "application/json"
            );


            var response = await client.PostAsync(url, jsonContent);
            

        }
    }
}