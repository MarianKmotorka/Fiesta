﻿using System.Collections.Generic;

namespace Fiesta.WebApi.Middleware.ExceptionHanlding
{
    public class ErrorResponse
    {
        public string ErrorCode { get; set; }

        public string ErrorMessage { get; set; }

        public List<ErrorDetail> ErrorDetails { get; set; }

        public ErrorResponse()
        {
            ErrorDetails = new List<ErrorDetail>();
        }
    }
}
