using WebAPI.Application.Dtos;

namespace WebAPI.Features.OperationClaims.Dtos;

public class UpdatedOperationClaimDto : IDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}
