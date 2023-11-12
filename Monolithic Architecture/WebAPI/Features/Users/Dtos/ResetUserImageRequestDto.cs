using WebAPI.Application.Dtos;

namespace BusinessLayer.Features.Users.Dtos;

public class ResetUserImageRequestDto : IDto
{
    public int Id { get; set; }
}
