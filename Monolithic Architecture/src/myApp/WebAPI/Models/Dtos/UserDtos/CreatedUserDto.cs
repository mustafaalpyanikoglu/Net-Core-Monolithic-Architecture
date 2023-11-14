namespace WebAPI.Models.Dtos.UserDtos;

public class CreatedUserDto : IDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public DateTime RegistrationDate { get; set; }
    public string? ImageUrl { get; set; }
}
