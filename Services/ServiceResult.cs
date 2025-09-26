namespace StilSepetiApp.Services
{
    
    public class ServiceResult
    {
        public bool Success { get; protected set; }
        public string Message { get; protected set; }

        public static ServiceResult SuccessBuilder(string message) =>
            new ServiceResult { Success = true, Message = message };

        public static ServiceResult FailureBuilder(string message) =>
            new ServiceResult { Success = false, Message = message };
    }

   
    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; set; }
        public string? ErrorCode { get; set; }

        public static ServiceResult<T> SuccessResult(T data, string message = "") =>
            new ServiceResult<T>
            {
                Success = true,
                Message = message,
                Data = data
            };

        public static ServiceResult<T> FailureResult(string message, string? errorCode = null) =>
            new ServiceResult<T>
            {
                Success = false,
                Message = message,
                ErrorCode = errorCode
            };
    }
}