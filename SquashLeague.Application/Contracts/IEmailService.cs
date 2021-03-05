using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SquashLeague.Application.Contracts
{
    public interface IEmailService
    {
        Task<bool> SendEmail(string emailAddress, string mailContent);
    }
}
