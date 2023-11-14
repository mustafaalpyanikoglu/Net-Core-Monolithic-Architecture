namespace WebAPI.Models.Dtos.UserDtos;

public class UserDto : IDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool UserStatus { get; set; }
    public string? ImageUrl { get; set; }

    public int? OperationClaimId { get; set; }
    public string? OperationClaimName { get; set; }

}
