using WebAPI.Models.Concrete;

namespace BusinessLayer.Features.UserOperationClaims.Constants;

public static class UserOperationClaimMessages
{
    public const string UserOperationClaimNotFound = "user operation claim not found";
    public const string UserOperationClaimFound = "user operation claim found";
    public const string UserOperationClaimAdded = "added user operation claim";
    public const string UserOperationClaimDeleted = "deleted user operation claim";
    public const string UserOperationClaimUpdated = "updated user operation claim";
    public const string TheUserAlreadyHasSuchRole = "The user already has such a role";
    public const string TheUserAlreadyHasRole = "The user already has a role";
}
