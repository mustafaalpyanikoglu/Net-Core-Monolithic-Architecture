using WebAPI.Persistence.Paging;
using WebAPI.Features.OperationClaims.Dtos;

namespace BusinessLayer.Features.OperationClaims.Models;

public class OperationClaimListModel : BasePageableModel
{
    public IList<OperationClaimListDto> Items { get; set; }
}
