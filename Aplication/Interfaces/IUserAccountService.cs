namespace TalentoPlus.Application.Interfaces
{
    public interface IUserAccountService
    {
        Task<(string? UserId, bool Succeeded, IEnumerable<string> Errors, bool IsNew)> RegisterOrRetrieveUserAsync(
            string email, 
            string firstName, 
            string lastName, 
            string password, 
            string role);
    }
}