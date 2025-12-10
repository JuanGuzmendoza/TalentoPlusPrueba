namespace TalentoPlus.Application.Interfaces
{
    public interface IEmailNotificationService
    {
        Task SendWelcomeEmailAsync(string name, string email);
    }
}