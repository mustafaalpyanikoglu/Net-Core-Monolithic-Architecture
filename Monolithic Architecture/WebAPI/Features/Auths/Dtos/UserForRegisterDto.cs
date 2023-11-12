using WebAPI.Application.Dtos;
using Microsoft.AspNetCore.Http;

namespace BusinessLayer.Features.Auths.Dtos;

public class UserForRegisterDto : IDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public string Password { get; set; }
}
