namespace WebAPI.Models.Dtos.AuthDtos;

public class UserForChangePasswordDto : IDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
