using FluentValidation;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace UsersInteractions.Infrastructure.Middleware;

public class ValidationFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    private readonly IValidator<T>? _validator;
    private readonly ILogger<ValidationFilter<T>> _logger;
    
    public ValidationFilter(IServiceProvider serviceProvider, ILogger<ValidationFilter<T>> logger)
    {
        _logger = logger;
        _validator = serviceProvider.GetService<IValidator<T>>();
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        if (_validator == null)
        {
            await next.Send(context);
            return;
        }

        var validationResult = await _validator.ValidateAsync(context.Message, context.CancellationToken);

        if (validationResult.IsValid)
        {
            await next.Send(context);
        }
        else
        {
            _logger.LogError($"Masstransit validation exception: \n{string.Join('\n', validationResult.Errors)}");
            
            throw new ValidationException(validationResult.Errors);
        }
    }

    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("validation");
    }
}