namespace PurpleBackendService.Core
{
    public class OperationResult<T>
    {
        public bool IsSuccess { get; private set; }
        public List<string> Errors { get; private set; } = new List<string>();
        public T? Data { get; private set; }

        public static OperationResult<T> Success(T data) =>
            new OperationResult<T>
            {
                IsSuccess = true,
                Data = data
            };

        public static OperationResult<T> Failure(params string[] errors) =>
            new OperationResult<T>
            {
                IsSuccess = false,
                Errors = errors.ToList()
            };
    }
}

