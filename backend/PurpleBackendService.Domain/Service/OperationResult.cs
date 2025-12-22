namespace PurpleBackendService.Domain.Service
{
    /// <summary>
    /// The class acting as the result of the operation
    /// </summary>
    /// <typeparam name="T">The class that should return this result</typeparam>
    public class OperationResult<T>
    {
        /// <summary>
        /// Checking for successful completion of the operation
        /// </summary>
        public bool IsSuccess { get; private set; }

        /// <summary>
        /// Contains a list of errors in the form of strings
        /// </summary>
        public List<string> Errors { get; private set; } = new List<string>();

        /// <summary>
        /// Result of operation
        /// </summary>
        public T? Result { get; private set; }

        public static OperationResult<T> Success(T data) =>
            new()
            {
                IsSuccess = true,
                Result = data
            };

        public static OperationResult<T> Failure(params string[] errors) =>
            new()
            {
                IsSuccess = false,
                Errors = errors.ToList()
            };
    }
}

