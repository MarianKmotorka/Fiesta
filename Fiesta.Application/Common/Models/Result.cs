using System;
using System.Collections.Generic;
using System.Linq;

namespace Fiesta.Application.Common.Models
{
    public class Result<T>
    {
        internal Result(IEnumerable<string> errors)
        {
            if (errors is null || !errors.Any())
                throw new ArgumentException("Argument \"errors\" cannot be null or empty.");

            Succeeded = false;
            Errors = errors;
            Data = default;
        }

        internal Result(T data = default)
        {
            Data = data;
            Succeeded = true;
            Errors = Array.Empty<string>();
        }

        public bool Succeeded { get; }

        public T Data { get; }

        public IEnumerable<string> Errors { get; }

        public static Result<T> Success<T>(T data = default)
        {
            return new Result<T>(data);
        }

        public static Result<T> Failure(IEnumerable<string> errors)
        {
            return new Result<T>(errors);
        }

        public static Result<T> Failure(string error)
        {
            return new Result<T>(new[] { error });
        }
    }

    public class Result : Result<object>
    {
    }
}
