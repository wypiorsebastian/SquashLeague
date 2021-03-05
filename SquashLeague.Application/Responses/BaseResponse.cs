using System;
using System.Collections.Generic;
using System.Text;

namespace SquashLeague.Application.Responses
{
    public class BaseResponse
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public List<string> ValidationErrors { get; set; }

        public BaseResponse()
        {
            IsSuccess = true;
        }

        public BaseResponse(string message = null)
        {
            IsSuccess = true;
            Message = message;
        }

        public BaseResponse(string message, bool isSuccess)
        {
            Message = message;
            IsSuccess = isSuccess;
        }
    }
}
