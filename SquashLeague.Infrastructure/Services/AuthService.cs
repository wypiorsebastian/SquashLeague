using Microsoft.AspNetCore.Identity;
using SquashLeague.Application.Contracts;
using SquashLeague.Application.DTO;
using SquashLeague.Application.Models;
using SquashLeague.Domain.Entities;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SquashLeague.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailService _emailService;

        public AuthService(UserManager<AppUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            SignInManager<AppUser> signInManager,
            IEmailService emailService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task<UserDTO> RegisterUser(SignupModel signupModel)
        {
            var playerRole = "Player";
            var adminRole = "Admin";

            if (!await _roleManager.RoleExistsAsync(playerRole))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(playerRole));
            }
            if (!await _roleManager.RoleExistsAsync(adminRole))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            if (await _userManager.FindByNameAsync(signupModel.UserName) != null)
                throw new Exception($"User {signupModel.UserName} already exists");

            var userForCreation = new AppUser
            {
                UserName = signupModel.UserName,
                Email = signupModel.Email,
                FirstName = signupModel.FirstName,
                LastName = signupModel.LastName
            };

            
            var userCreationresult = await _userManager.CreateAsync(userForCreation, signupModel.Password);
            var user = await _userManager.FindByNameAsync(signupModel.UserName);
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var confirmationLink = $"{signupModel.ConfirmationLink}\\{new { userId = user.Id, @token = token }}";

            //_emailService.SendEmail(user.Email, )


            return new UserDTO();
        }

        public async Task<bool> ConfirmRegistration(string userName, string registrationToken)
        {
            var user = await _userManager.FindByNameAsync(userName);
            var result = await _userManager.ConfirmEmailAsync(user, registrationToken);

            if(result.Succeeded)
            {
                return true;
            }

            return false;
        }
    }
}
