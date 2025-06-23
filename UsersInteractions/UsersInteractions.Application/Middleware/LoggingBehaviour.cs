
using MediatR;
using Microsoft.Extensions.Logging; // Upewnij się, że masz pakiet Microsoft.Extensions.Logging

namespace UsersInteractions.Application.Middleware
{
    public class AddCommentCommand
        : IRequest
    {
    }

    public class LoggingBehavior<TRequest, TResponse> : 
        IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest
    {

    //     where TRequest : IRequest<TResponse>
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{typeof(TRequest).Name}");
            var response = await next();
            _logger.LogInformation("test");
            return response;
        }

    }
}