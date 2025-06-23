using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace UsersInteractions.Application.Middleware;

public class ValidationHandlers<TRequest, TResponse>
    : IPipelineBehavior<TRequest, TResponse>
{
    private readonly ILogger<ValidationHandlers<TRequest, TResponse>> _logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators;
    public ValidationHandlers(IEnumerable<IValidator<TRequest>> validators, ILogger<ValidationHandlers<TRequest, TResponse>> logger)
    {
        _validators = validators;
        _logger = logger;
    }
    
    

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct = default)
    {
        var requestTypeName = typeof(TRequest).Name;

        if (!_validators.Any())
        {
            return await next();
        }
        
        var validationContext = new ValidationContext<TRequest>(request);

        var validationResults =
            await Task.WhenAll(_validators.Select(v => v.ValidateAsync(validationContext, ct)));

        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure != null)
            .ToList();

        if (failures.Any())
        {
            _logger.LogError($"MediatR validation failed for {requestTypeName}: {failures.Count} error(s). First error: {failures.First().ErrorMessage}");
            throw new ValidationException(failures);
        }

        return await next();
    }
}