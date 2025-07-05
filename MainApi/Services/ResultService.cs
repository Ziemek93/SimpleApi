namespace MainApi.Services;

/// <summary>
/// Representing result of service operations.
/// </summary>
/// <typeparam name="T">Succes type.</typeparam>
public class ServiceResult<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T Data { get; }
    public IReadOnlyList<string> Errors { get; }
    
    protected ServiceResult(bool isSuccess, T data, IEnumerable<string> errors)
    {
        var error = string.Join("\n", errors?.ToList() ?? []);//new List<string>());
        IsSuccess = isSuccess;
        Data = data;
        Errors = errors?.ToList() ?? new List<string>();
    }

    /// <summary>
    /// Creates a success result with data.
    /// </summary>
    public static ServiceResult<T> Success(T data)
    {
        return new ServiceResult<T>(true, data, null);
    }

    /// <summary>
    /// Creating a failure result with a single error.
    /// </summary>
    public static ServiceResult<T> Failure(string error)
    {
        return new ServiceResult<T>(false, default, new[] { error });
    }

    /// <summary>
    /// Creates a failure result with a list of errors.
    /// </summary>
    public static ServiceResult<T> Failure(IEnumerable<string> errors)
    {
        return new ServiceResult<T>(false, default, errors);
    }
}

/// <summary>
/// Represents result of service operations.
/// </summary>
public sealed class ServiceResult : ServiceResult<object>
{
    private ServiceResult(bool isSuccess, IEnumerable<string> errors) 
        : base(isSuccess, null, errors) { }

    /// <summary>
    /// Creates a success result.
    /// </summary>
    public static ServiceResult Success()
    {
        return new ServiceResult(true, null);
    }

    /// <summary>
    /// Creates a failure result with a single error.
    /// </summary>
    public static new ServiceResult Failure(string error)
    {
        return new ServiceResult(false, new[] { error });
    }

    /// <summary>
    /// Creates a failure result with a list of errors.
    /// </summary>
    public static new ServiceResult Failure(IEnumerable<string> errors)
    {
        return new ServiceResult(false, errors);
    }
}