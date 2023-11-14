namespace WebAPI.Security.Jwt;

public class AccessToken
{
    public int UserID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? OperationClaimName { get; set; }
    public string Email { get; set; }
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}
