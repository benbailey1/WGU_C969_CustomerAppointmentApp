namespace ScheduleApplication.Features.Login
{
    public class AuthResult<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public string Error { get; }

        private AuthResult(bool isSuccess, T value, string error)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
        }

        public static AuthResult<T> Success(T value) => new AuthResult<T>(true, value, null);
        public static AuthResult<T> Failure(string error) => new AuthResult<T>(false, default, error);
    }
}
