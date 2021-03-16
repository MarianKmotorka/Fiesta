namespace Fiesta.WebApi.Middleware.ExceptionHanlding
{
    public class ErrorDetail
    {
        public string PropertyName { get; set; }

        public string Message { get; set; }

        public string Code { get; set; }

        public object CustomState { get; set; }
    }
}
