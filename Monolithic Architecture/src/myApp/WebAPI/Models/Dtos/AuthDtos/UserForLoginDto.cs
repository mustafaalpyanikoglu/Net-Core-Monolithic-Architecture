namespace WebAPI.Models.Dtos.AuthDtos;

public class UserForLoginDto : IDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
