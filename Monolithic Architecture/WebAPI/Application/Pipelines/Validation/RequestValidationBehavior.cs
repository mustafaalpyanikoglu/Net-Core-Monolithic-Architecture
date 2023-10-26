using FluentValidation;
using FluentValidation.Results;
using MediatR;
using WebAPI.CrossCuttingConcerns.Exceptions.Types;
using ValidationException = WebAPI.CrossCuttingConcerns.Exceptions.Types.ValidationException;

public class RequestValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the RequestValidationBehavior class.
    /// </summary>
    /// <param name="validators">The collection of validators for the request.</param>
    public RequestValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <summary>
    /// Handles the validation of the request in the pipeline.
    /// </summary>
    /// <param name="request">The request to be validated.</param>
    /// <param name="next">The RequestHandlerDelegate representing the next delegate in the pipeline.</param>
    /// <param name="cancellationToken">The CancellationToken for the request handling.</param>
    /// <returns>A Task representing the asynchronous handling of the validated request and response.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ValidationContext<object> context = new ValidationContext<object>(request);
        IEnumerable<ValidationExceptionModel> errors = _validators
            .Select(validator => validator.Validate(context))
            .SelectMany(result => result.Errors)
            .Where(failure => failure != null)
            .GroupBy(
                keySelector: p => p.PropertyName,
                resultSelector: (propertyName, errors) =>
                    new ValidationExceptionModel { Property = propertyName, Errors = errors.Select(e => e.ErrorMessage) }
            )
            .ToList();

        if (errors.Any())
            throw new ValidationException(errors);

        TResponse response = await next();
        return response;
    }
}
