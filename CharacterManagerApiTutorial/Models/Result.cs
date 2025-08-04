namespace CharacterManagerApiTutorial.Models
{
    public class Result
    {
        public bool IsSuccess { get; set; }
        public string? Error { get; set; }

        public static Result Success() => new() { IsSuccess = true };
        public static Result Failure(string error) => new() { IsSuccess = false, Error = error };
    }

    public class Result<T> : Result
    {
        public T? Value { get; init; }

        public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };
        public static new Result<T> Failure(string error) => new() { IsSuccess = false, Error = error };
    }
}
