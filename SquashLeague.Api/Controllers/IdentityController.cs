﻿using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("ConfirmEmail")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<bool> ConfirmEmail(string userId, string token)
        {
            return await _authService.ConfirmRegistration(userId, token);
        }

    }
}