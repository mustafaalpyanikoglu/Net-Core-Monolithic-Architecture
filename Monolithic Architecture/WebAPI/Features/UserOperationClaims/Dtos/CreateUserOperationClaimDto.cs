using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.UserOperationClaims.Dtos;

public class CreateUserOperationClaimDto : IDto
{
    public int UserId { get; set; }
    public int OperationClaimId { get; set; }
}
