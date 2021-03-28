using SquashLeague.Application.DTO;
using SquashLeague.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SquashLeague.Application.Contracts
{
    public interface IAuthService
    {
        Task<UserDTO> RegisterUser(SignupModel signupModel);
        Task<string> Signin(SigninModel signinModel);
        Task<bool> ConfirmRegistration(string userName, string registrationToken);
    }
}
