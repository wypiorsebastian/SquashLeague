using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SquashLeague.Application.Contracts;
using SquashLeague.Application.DTO;
using SquashLeague.Application.Models;
using System;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace SquashLeague.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class IdentityController : BaseApiController
    {
        private readonly IAuthService _authService;        
        public IdentityController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }
        

        [HttpPost(Name = "Signup")]
        [ProducesResponseType(typeof(UserDTO), Status200OK)]
        public async Task<UserDTO> Signup(SignupModel signupModel)
        {
            signupModel.ConfirmationLink = Url.ActionLink("ConfirmEmail", "Identity");
            await _authService.RegisterUser(signupModel);
            return new UserDTO();
        }

        [HttpPost("Signin")]
        [ProducesResponseType(typeof(UserDTO), Status200OK)]
        public async Task<ActionResult> SignIn(SigninModel signinModel)
        {
            var signinResult = await _authService.Signin(signinModel);

            return Ok(new SuccessSignInResponse
            {
                Token = signinResult.Token,
                RefreshToken = signinResult.RefreshToken
            });
        }
        
        [HttpPost("RefreshToken")]
        [ProducesResponseType(typeof(UserDTO), Status200OK)]
        public async Task<ActionResult> RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            string tokenResult = string.Empty;
            //var tokenRefreshResult = await _authService.RefreshTokenAsync(refreshTokenRequest);
            var tokenRefreshResult = await _authService.VerifyAndGenerateTokenAsync(refreshTokenRequest);
            //return Ok(new { token = tokenResult});
            return Ok(new SuccessSignInResponse
            {
                Token = tokenRefreshResult.Token,
                RefreshToken = tokenRefreshResult.RefreshToken

            });
        }

        [HttpGet("ConfirmEmail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<bool> ConfirmEmail(string userName, string token)
        {
            return await _authService.ConfirmRegistration(userName, token);
        }

    }
}
