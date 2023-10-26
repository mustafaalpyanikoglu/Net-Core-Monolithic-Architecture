using MediatR;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using WebAPI.CrossCuttingConcerns;
using WebAPI.CrossCuttingConcerns.Logging;

namespace WebAPI.Application.Pipelines.Logging
{
    /// <summary>
    /// Pipeline behavior for logging requests and responses using a LoggerServiceBase implementation.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request to be logged.</typeparam>
    /// <typeparam name="TResponse">The type of the response returned by the request.</typeparam>
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, ILoggableRequest
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly LoggerServiceBase _loggerServiceBase;

        /// <summary>
        /// Initializes a new instance of the LoggingBehavior class.
        /// </summary>
        /// <param name="loggerServiceBase">The LoggerServiceBase implementation used for logging.</param>
        /// <param name="httpContextAccessor">The IHttpContextAccessor to access the HttpContext.</param>
        public LoggingBehavior(LoggerServiceBase loggerServiceBase, IHttpContextAccessor httpContextAccessor)
        {
            _loggerServiceBase = loggerServiceBase;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Handles the logging of requests and responses in the pipeline.
        /// </summary>
        /// <param name="request">The request to be logged.</param>
        /// <param name="next">The RequestHandlerDelegate representing the next delegate in the pipeline.</param>
        /// <param name="cancellationToken">The CancellationToken for the request handling.</param>
        /// <returns>A Task representing the asynchronous handling of the request and response.</returns>
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            List<LogParameter> logParameters =
                new()
                {
                    new LogParameter { Type = request.GetType().Name, Value = request }
                };

            LogDetail logDetail =
                new()
                {
                    MethodName = next.Method.Name,
                    Parameters = logParameters,
                    User = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "?"
                };

            _loggerServiceBase.Info(JsonSerializer.Serialize(logDetail));
            return await next();
        }
    }
}
