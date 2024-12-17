namespace ScheduleApplication.Shared.Classes
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }
        public bool IsNotFound { get; set; }

        private Result(bool isSuccess, T value, string error, bool isNotFound = false) 
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            IsNotFound = isNotFound;
        }

        public static Result<T> Success(T value) => new Result<T>(true, value, null);
        public static Result<T> Failure(string error) => new Result<T>(false, default, error);
        public static Result<T> NotFound(string message) => new Result<T>(false, default, message, true);
    }
}
