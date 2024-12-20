using System.Collections.Generic;

namespace ScheduleApplication.Features.Appointments
{
    public class AppointmentResult<T>
    {
        public bool IsSuccess { get; }
        public T Value { get; }
        public List<string> Errors { get; }
        public bool IsNotFound { get; }
        public bool IsValidationError { get; }

        private AppointmentResult(bool isSuccess, T value, List<string> errors, bool isNotFound = false, bool isValidationError = false)
        {
            IsSuccess = isSuccess;
            Value = value;
            Errors = errors ?? new List<string>();
            IsNotFound = isNotFound;
            IsValidationError = isValidationError;
        }

        public static AppointmentResult<T> Success(T value) => new AppointmentResult<T>(true, value, null);

        public static AppointmentResult<T> Failure(string error) => new AppointmentResult<T>(false, default, new List<string> { error });

        public static AppointmentResult<T> Failure(List<string> errors) => new AppointmentResult<T>(false, default, errors);

        public static AppointmentResult<T> NotFound(string message) => new AppointmentResult<T>(false, default, new List<string> { message }, true);

        public static AppointmentResult<T> ValidationError(List<string> errors) => new AppointmentResult<T>(false, default, errors, false, true);
    }
}
