using System.Collections.Generic;
using FluentValidation.Results;

namespace Fiesta.WebApi.Middleware.ExceptionHanlding
{
    public class ErrorResponseFactory
    {
        public static ErrorResponse CreateBadRequestErrorResponse(string errorMessage)
            => new()
            {
                ErrorCode = "BadRequest",
                ErrorMessage = errorMessage
            };

        public static ErrorResponse CreateBadRequestErrorResponse(IEnumerable<ValidationFailure> validationFailures)
        {
            var response = new ErrorResponse
            {
                ErrorCode = "ValidationError",
                ErrorMessage = "Request validation failed"
            };

            foreach (var failure in validationFailures)
            {
                var errorDetail = new ErrorDetail
                {
                    Code = failure.ErrorCode,
                    Message = failure.ErrorMessage,
                    CustomState = failure.CustomState,
                    PropertyName = failure.PropertyName
                };

                response.ErrorDetails.Add(errorDetail);
            }

            return response;
        }

        public static ErrorResponse CreateNotFoundErrorResponse(string errorMessage = null)
        {
            if (errorMessage != null && string.IsNullOrWhiteSpace(errorMessage))
                errorMessage = null;

            return new()
            {
                ErrorCode = "NotFound",
                ErrorMessage = errorMessage ?? "Resource was not found."
            };
        }

        public static ErrorResponse CreateForbiden403Response(string message = null)
            => new()
            {
                ErrorCode = "Forbidden",
                ErrorMessage = string.IsNullOrEmpty(message) ? "You cannot access this resource." : message
            };

        public static ErrorResponse CreateUnauthorizedn401Response(string message = null)
            => new()
            {
                ErrorCode = "Unauthorized",
                ErrorMessage = string.IsNullOrEmpty(message) ? "Request lacks valid authentication header." : message
            };
    }
}
