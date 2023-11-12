using BusinessLayer.Features.UserOperationClaims.Dtos;
using WebAPI.Persistence.Paging;

namespace BusinessLayer.Features.UserOperationClaims.Models;

public class UserOperationClaimListModel : BasePageableModel
{
    public IList<UserOperationClaimListDto> Items { get; set; }
}
