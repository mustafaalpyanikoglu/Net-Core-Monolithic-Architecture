using WebAPI.Models.Concrete;

namespace WebAPI.Security.Jwt;

public interface ITokenHelper
{
    AccessToken CreateToken(User user, IList<OperationClaim> operationClaims);
    RefreshToken CreateRefreshToken(User user, string ipAddress);
}
