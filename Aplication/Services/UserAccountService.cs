using Microsoft.AspNetCore.Identity;
using TalentoPlus.Application.Interfaces;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Application.Services
{
    public class UserAccountService : IUserAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailNotificationService _emailService; 

        public UserAccountService(UserManager<User> userManager, IEmailNotificationService emailService) 
        {
            _userManager = userManager;
            _emailService = emailService;
        }


        public async Task<(string? UserId, bool Succeeded, IEnumerable<string> Errors, bool IsNew)> RegisterOrRetrieveUserAsync(
            string email, 
            string firstName, 
            string lastName, 
            string password, 
            string role)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);
    
            if (existingUser != null)
            {
                return (existingUser.Id, true, Enumerable.Empty<string>(), false); 
            }

            var newUser = new User
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                EmailConfirmed = true
            };
    
            var result = await _userManager.CreateAsync(newUser, password);

            if (!result.Succeeded)
            {
                return (null, false, result.Errors.Select(e => e.Description), false); 
            }

            await _userManager.AddToRoleAsync(newUser, string.IsNullOrWhiteSpace(role) ? "User" : role);

            _ = _emailService.SendWelcomeEmailAsync($"{firstName} {lastName}", email); 

            return (newUser.Id, true, Enumerable.Empty<string>(), true); 
        }
    }
}