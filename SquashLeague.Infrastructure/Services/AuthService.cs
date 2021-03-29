using Microsoft.AspNetCore.Identity;
using SquashLeague.Application.Contracts;
using SquashLeague.Application.DTO;
using SquashLeague.Application.Models;
using SquashLeague.Domain.Entities;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SquashLeague.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthService(UserManager<AppUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            SignInManager<AppUser> signInManager,
            IConfiguration configuration,
            IEmailService emailService)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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

            var confirmationData = new { userName = user.Id, registrationToken = token };

            var confirmationLink = $"{signupModel.ConfirmationLink}?userName={user.UserName}&token={token}";

            await _emailService.SendEmail(user.Email, confirmationLink);


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

        public async Task<string> Signin(SigninModel signinModel)
        {
            var issuer = _configuration["Tokens:Issuer"];
            var audience = _configuration["Tokens:Audience"];
            var key = _configuration["Tokens:Key"];
            string result = string.Empty;

            var signinResult =
                    await _signInManager.PasswordSignInAsync(signinModel.UserName, signinModel.Password, false, false);
            if (signinResult.Succeeded)
            {
                var user = await _userManager.FindByNameAsync(signinModel.UserName);
                if (user != null)
                {
                    var claims = new[]
                    {
                            new Claim(JwtRegisteredClaimNames.Email , user.Email),
                            new Claim(JwtRegisteredClaimNames.Jti , user.Id),
                            new Claim("xxx", "yyy")
                    };

                    var keyBytes = Encoding.UTF8.GetBytes(key);
                    var theKey = new SymmetricSecurityKey(keyBytes);
                    var creds = new SigningCredentials(theKey, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.Now.AddMinutes(30), signingCredentials: creds);

                    return new JwtSecurityTokenHandler().WriteToken(token);
                }
            }

            return result;
        }
    }
}
