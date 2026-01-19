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

        ///<summary>
        /// Creates a successful operation result
        /// </summary>
        /// <param name="data">Data to be returned</param>
        /// <returns>The result of successful operation</returns>
        public static OperationResult<T> Success(T data) =>
            new()
            {
                IsSuccess = true,
                Result = data
            };

        ///<summary>
        /// Creates an operation result with errors
        /// </summary>
        /// <param name="errors">List of errors or crash messages</param>
        /// <returns>The result of failed operation</returns>
        public static OperationResult<T> Failure(params string[] errors) =>
            new()
            {
                IsSuccess = false,
                Errors = errors.ToList()
            };
    }
}

