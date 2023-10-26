using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using WebAPI.Application.Pipelines.Authorization;
using WebAPI.CrossCuttingConcerns.Exceptions.Types;
using WebAPI.Security.Constants;
using WebAPI.Security.SecurityExtensions;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>, ISecuredRequest
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Initializes a new instance of the AuthorizationBehavior class.
    /// </summary>
    /// <param name="httpContextAccessor">The IHttpContextAccessor to access the HttpContext.</param>
    public AuthorizationBehavior(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Handles the authorization of secured requests in the pipeline.
    /// </summary>
    /// <param name="request">The secured request to be authorized.</param>
    /// <param name="next">The RequestHandlerDelegate representing the next delegate in the pipeline.</param>
    /// <param name="cancellationToken">The CancellationToken for the request handling.</param>
    /// <returns>A Task representing the asynchronous handling of the authorized request and response.</returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        List<string>? userRoleClaims = _httpContextAccessor.HttpContext.User.ClaimRoles();

        if (userRoleClaims == null)
            throw new AuthorizationException("You are not authenticated.");

        bool isNotMatchedAUserRoleClaimWithRequestRoles = userRoleClaims
            .FirstOrDefault(
                userRoleClaim => userRoleClaim == GeneralOperationClaims.Admin || request.Roles.Any(role => role == userRoleClaim)
            )
            .IsNullOrEmpty();

        if (isNotMatchedAUserRoleClaimWithRequestRoles)
            throw new AuthorizationException("You are not authorized.");

        TResponse response = await next();
        return response;
    }
}
