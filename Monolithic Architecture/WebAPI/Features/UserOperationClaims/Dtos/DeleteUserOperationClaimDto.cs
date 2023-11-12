using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.UserOperationClaims.Dtos;

public class DeleteUserOperationClaimDto : IDto
{
    public int Id { get; set; }
}
