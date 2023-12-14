using WebAPI.Paging;

namespace WebAPI.Models.Dtos.UserDtos;

public class UserListModel : BasePageableModel
{
    public IList<UserListDto> Items { get; set; }
}
