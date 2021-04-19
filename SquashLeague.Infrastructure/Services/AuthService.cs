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
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Options;
using SquashLeague.Infrastructure.Settings;
using SquashLeague.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace SquashLeague.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly AppDbContext _appDbContext;

        public AuthService(UserManager<AppUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            SignInManager<AppUser> signInManager,
            IConfiguration configuration,
            IEmailService emailService,
            IOptions<JwtSettings> jwtSettings, 
            TokenValidationParameters tokenValidationParameters,
            AppDbContext appDbContext)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
            _tokenValidationParameters = tokenValidationParameters ?? throw new ArgumentNullException(nameof(tokenValidationParameters));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _jwtSettings = jwtSettings.Value ?? throw new ArgumentNullException(nameof(jwtSettings.Value));
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        public async Task<UserDTO> RegisterUser(SignupModel signupModel)
        {
            const string playerRole = "Player";
            const string adminRole = "Admin";

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

            return result.Succeeded;
        }

        public async Task<AuthenticationResult> Signin(SigninModel signinModel)
        {
            var issuer = _jwtSettings.Issuer;// _configuration["Tokens:Issuer"];
            var audience = _jwtSettings.Audience;// _configuration["Tokens:Audience"];
            var key = _jwtSettings.Key;//_configuration["Tokens:Key"];
            var tokenLifeTime = _jwtSettings.TokenLifeTime;
            //var result = string.Empty;
            

            var signinResult =
                    await _signInManager.PasswordSignInAsync(signinModel.UserName, signinModel.Password, false, false);
            if (!signinResult.Succeeded) return new AuthenticationResult
            {
                Errors = new[] {"Authentication failed"}
            };
            
            var user = await _userManager.FindByNameAsync(signinModel.UserName);
            if (user == null) return new AuthenticationResult
            {
                Errors = new[] { "User doesn't exist" }
            };

            return await GenerateAuthenticationResultAsync(user);
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultAsync(AppUser appUser)
        {
            var issuer = _jwtSettings.Issuer;// _configuration["Tokens:Issuer"];
            var audience = _jwtSettings.Audience;// _configuration["Tokens:Audience"];
            var key = _jwtSettings.Key;//_configuration["Tokens:Key"];
            var tokenLifeTime = _jwtSettings.TokenLifeTime;
            //var result = string.Empty;
            

            var userClaims = await _userManager.GetRolesAsync(appUser);
            //var userRoles = from role in userClaims select new Claim(ClaimTypes.Role, role);
            var userRoles = userClaims.Select(x => new Claim(ClaimTypes.Role, x));

            var claims = new List<Claim>()
            {
                new Claim(JwtRegisteredClaimNames.Email , appUser.Email),
                new Claim(JwtRegisteredClaimNames.Jti , appUser.Id),
                new Claim("xxx", "yyy")
            };

            claims.AddRange(userRoles);

            var keyBytes = Encoding.UTF8.GetBytes(key);
            var theKey = new SymmetricSecurityKey(keyBytes);
            var creds = new SigningCredentials(theKey, SecurityAlgorithms.HmacSha256Signature);
            var token = new JwtSecurityToken(issuer, audience, claims, expires: DateTime.Now.AddSeconds(300), signingCredentials: creds);

            var refreshToken = new RefreshToken
            {
                JwtId = token.Id,
                UserId = appUser.Id,
                IsUsed = false,
                CreationDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddMonths(6),
                Token = Guid.NewGuid().ToString()
            };

            await _appDbContext.RefreshTokens.AddAsync(refreshToken);
            await _appDbContext.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken.Token
            };
        }

        public async Task<AuthenticationResult> VerifyAndGenerateTokenAsync(RefreshTokenRequest refreshTokenRequest)
        {
            try
            {
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var tokenToBeVerified = jwtTokenHandler.ValidateToken(refreshTokenRequest.Token,
                                                                    _tokenValidationParameters,
                                                                    out var validatedToken);

                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var isHmacSha256Encrypted = jwtSecurityToken.Header.Alg.Equals("http://www.w3.org/2001/04/xmldsig-more#hmac-sha256");
                    
                    //var isHmacSha256Encrypted = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                    //                                                StringComparison.InvariantCultureIgnoreCase);
                    if (isHmacSha256Encrypted is false)
                        return null;

                    var utcExpiryDate = long.Parse(tokenToBeVerified.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                    var expiryDate = ConvertUnixTimeStampToDateTime(utcExpiryDate);

                    if (expiryDate > DateTime.UtcNow)
                        return new AuthenticationResult
                        {
                            Success = false,
                            Errors = new[] { "Token hasn't expired yet" }
                        };

                    var persistedToken = await _appDbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == refreshTokenRequest.RefreshToken);

                    if (persistedToken is null)
                        return new AuthenticationResult
                        {
                            Success = false,
                            Errors = new[] { "Token doesn't exist" }
                        };

                    if (persistedToken.IsUsed)
                        return new AuthenticationResult
                        {
                            Success = false,
                            Errors = new[] { "Token has been used" }
                        };

                    if (persistedToken.IsInvalidated)
                        return new AuthenticationResult
                        {
                            Success = false,
                            Errors = new[] { "Token has been invalidated" }
                        };

                    var jti = tokenToBeVerified.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

                    if (persistedToken.JwtId != jti)
                        return new AuthenticationResult
                        {
                            Success = false,
                            Errors = new[] { "Token doesn't match" }
                        };

                    persistedToken.IsUsed = true;
                    _appDbContext.RefreshTokens.Update(persistedToken);
                    await _appDbContext.SaveChangesAsync();

                    var currentUser = await _userManager.FindByIdAsync(persistedToken.UserId);

                    return await GenerateAuthenticationResultAsync(currentUser);
                }

                return null;
            }
            catch (Exception exception)
            {               

                return null;
            }
        }

        private DateTime ConvertUnixTimeStampToDateTime(long unixTimeStamp)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(unixTimeStamp)
                .ToLocalTime();
        }
    }
}
