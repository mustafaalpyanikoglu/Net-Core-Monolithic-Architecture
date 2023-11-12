using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.UserOperationClaims.Dtos;

public class UserOperationClaimListDto : IDto
{
    public string UserFirstName { get; set; }
    public string UserLastName { get; set; }
    public string OperationClaimName { get; set; }
    public string OperationClaimNameDescription { get; set; }
}
