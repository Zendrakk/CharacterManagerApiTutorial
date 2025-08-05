namespace CharacterManagerApiTutorial.Models
{
    /// <summary>
    /// Represents the result of an operation without a return value.
    /// Indicates success or failure and contains an optional error message.
    /// </summary>
    public class Result
    {
        // Indicates whether the operation was successful.
        public bool IsSuccess { get; set; }

        // The error message if the operation failed; null if successful.
        public string? Error { get; set; }

        // Creates a successful result.
        public static Result Success() => new() { IsSuccess = true };

        // Creates a failed result with the provided error message.
        public static Result Failure(string error) => new() { IsSuccess = false, Error = error };
    }


    /// <summary>
    /// Represents the result of an operation that returns a value.
    /// Inherits from <see cref="Result"/> and adds a Value payload.
    /// </summary>
    public class Result<T> : Result
    {
        // The value returned if the operation was successful; null if failed.
        public T? Value { get; init; }

        // Creates a successful result with a value.
        public static Result<T> Success(T value) => new() { IsSuccess = true, Value = value };

        /// Creates a failed result with the provided error message, and hides the base class's Failure method to return a Result.
        public static new Result<T> Failure(string error) => new() { IsSuccess = false, Error = error };
    }
}
