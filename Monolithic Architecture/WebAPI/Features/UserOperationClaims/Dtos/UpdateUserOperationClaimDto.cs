using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.UserOperationClaims.Dtos;

public class UpdateUserOperationClaimDto : IDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int OperationClaimId { get; set; }
}
