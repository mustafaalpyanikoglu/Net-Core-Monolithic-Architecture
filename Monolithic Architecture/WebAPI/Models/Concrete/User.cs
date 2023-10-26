using WebAPI.Persistence.Repositories;

namespace WebAPI.Models.Concrete;

public class User : Entity
{
    #region User Properties
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public DateTime RegistrationDate { get; set; }
    public bool UserStatus { get; set; }
    #endregion

    public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; }

    public User()
    {
        UserOperationClaims = new HashSet<UserOperationClaim>();
    }

    public User
        (int id, string firstName, string lastName, string phoneNumber, string address,
        string email, byte[] passwordHash, byte[] passwordSalt,
        bool userStatus, DateTime registrationDate) : this()
    {
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
        PasswordHash = passwordHash;
        PasswordSalt = passwordSalt;
        RegistrationDate = registrationDate;
        UserStatus = userStatus;
    }
}
