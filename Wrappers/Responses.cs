namespace MauiCrud.Wrappers
{
    public class Responses
    {
        public static ApiResponse Failed(string message) => new ApiResponse
        {
            IsSuccess = false,
            Message = message
        };

        public static ApiResponse Success(string message, object? result = null) => new ApiResponse
        {
            IsSuccess = true,
            Result = result,
            Message = message
        };

        public static PaymentResponse FailedPayment(string message) => new PaymentResponse
        {
            IsSuccess = false,
            Message = message
        };

        public static PaymentResponse SuccessPayment(string? message, object? result, decimal amountDue) => new PaymentResponse
        {
            IsSuccess = true,
            Payments = result,
            Message = message,
            AmountDue = amountDue
        };

        public static LoginResponse FailedLogin(string message, LoginResponseType loginResponseType) => new LoginResponse
        {
            Message = message,
            LoginResponseType = loginResponseType
        };

        public static LoginResponse SuccessLogin(object? result) => new LoginResponse
        {
            LoginResponseType = LoginResponseType.Success,
            Result = result
        };
    }

    public class PaymentResponse
    {
        public bool IsSuccess { get; set; } = true;
        public object? Payments { get; set; }
        public decimal? AmountDue { get; set; }
        public string? Message { get; set; }
    }

    public class ApiResponse
    {
        public bool IsSuccess { get; set; } = true;
        public object? Result { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ServiceResponse
    {
        public ServiceResponse()
        {
            Success = true;
        }

        public object EntityId { get; set; } = 0;

        public bool Success { get; set; } = true;

        public string? Message { get; set; }

        public static ServiceResponse Failed(Exception error) => Failed(error.Message);
        public static ServiceResponse Failed(string message) => new ServiceResponse
        {
            Success = false,
            Message = message
        };
    }

    public class LoginResponse
    {
        public bool IsSuccess { get { return LoginResponseType == LoginResponseType.Success; } }
        public object? Result { get; set; }
        public string? Message { get; set; }
        public LoginResponseType LoginResponseType { get; set; }
    }
    public enum LoginResponseType
    {
        Success,
        AccountNotFound,
        InvalidPassword,
        AccountLocked,
        UnknownError
    }
}
