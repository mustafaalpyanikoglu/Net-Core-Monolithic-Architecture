using BusinessLayer.Features.Users.Dtos;
using WebAPI.Persistence.Paging;

namespace BusinessLayer.Features.Users.Models;

public class UserListModel : BasePageableModel
{
    public IList<UserListDto> Items { get; set; }
}
