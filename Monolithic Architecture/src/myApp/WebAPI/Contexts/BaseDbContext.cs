using Microsoft.EntityFrameworkCore;
using WebAPI.Models.Concrete;

namespace WebAPI.Contexts;
public class BaseDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<OperationClaim> OperationClaims { get; set; }
    public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerWarehouseCost> CustomerWarehouseCosts { get; set; }
    public DbSet<EmailAuthenticator> EmailAuthenticators { get; set; }
    public DbSet<OtpAuthenticator> OtpAuthenticators { get; set; }
    public DbSet<Warehouse> Warehouses { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=DESKTOP-I6QFS5F;Database=MyAppDB; Trusted_Connection=True;");
        optionsBuilder.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
    }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        #region Customer Model Creation
        builder.Entity<Customer>().ToTable("Customers").HasKey(u => u.Id);
        builder.Entity<Customer>().Property(u => u.Id).HasColumnName("Id");
        builder.Entity<Customer>().Property(u => u.UserID).HasColumnName("UserId").IsRequired();
        builder.Entity<Customer>().Property(u => u.Demand).HasColumnName("Demand").IsRequired();

        builder.Entity<Customer>().HasOne(u => u.User).WithOne(u => u.Customer).HasForeignKey<Customer>(u => u.UserID);
        #endregion

        #region CustomerWarehouseCost Model Creation
        builder.Entity<CustomerWarehouseCost>().ToTable("CustomerWarehouseCosts").HasKey(u => u.Id);
        builder.Entity<CustomerWarehouseCost>().Property(u => u.Id).HasColumnName("Id");
        builder.Entity<CustomerWarehouseCost>().Property(u => u.WarehouseID).HasColumnName("WarehouseID").IsRequired();
        builder.Entity<CustomerWarehouseCost>().Property(u => u.CustomerId).HasColumnName("CustomerId").IsRequired();
        builder.Entity<CustomerWarehouseCost>().Property(u => u.Cost).HasColumnName("Cost").IsRequired();

        builder.Entity<CustomerWarehouseCost>().HasOne(u => u.Warehouse).WithMany().HasForeignKey(u => u.WarehouseID);
        builder.Entity<CustomerWarehouseCost>().HasOne(u => u.Customer).WithMany().HasForeignKey(u => u.CustomerId);
        #endregion

        #region Email Authenticator Model Creation
        builder.Entity<EmailAuthenticator>().ToTable("EmailAuthenticators").HasKey(e => e.Id);
        builder.Entity<EmailAuthenticator>().Property(e => e.UserId).HasColumnName("UserId");
        builder.Entity<EmailAuthenticator>().Property(e => e.ActivationKey).HasColumnName("ActivationKey");
        builder.Entity<EmailAuthenticator>().Property(e => e.IsVerified).HasColumnName("IsVerified");
        builder.Entity<EmailAuthenticator>().HasOne(e => e.User);
        #endregion

        #region OperationClaim Model Creation
        builder.Entity<OperationClaim>().ToTable("OperationClaims").HasKey(u => u.Id);
        builder.Entity<OperationClaim>().Property(u => u.Id).HasColumnName("Id").UseIdentityColumn(1, 1);
        builder.Entity<OperationClaim>().Property(u => u.Name).HasColumnName("Name").HasMaxLength(50).IsRequired();
        builder.Entity<OperationClaim>().Property(u => u.Description).HasColumnName("Description").HasMaxLength(500).IsRequired();

        OperationClaim[] operationClaimSeeds =
        {
                new(1, "admin","Authorized with full access."),
                new(2, "user","Person registered in the system"),
                new(3, "intern","Registered intern")
            };
        builder.Entity<OperationClaim>().HasData(operationClaimSeeds);
        #endregion

        #region Otp Authenticator Model Creation
        builder.Entity<OtpAuthenticator>().ToTable("OtpAuthenticators").HasKey(e => e.Id);
        builder.Entity<OtpAuthenticator>().Property(e => e.UserId).HasColumnName("UserId");
        builder.Entity<OtpAuthenticator>().Property(e => e.SecretKey).HasColumnName("SecretKey");
        builder.Entity<OtpAuthenticator>().Property(e => e.IsVerified).HasColumnName("IsVerified");
        builder.Entity<OtpAuthenticator>().HasOne(e => e.User);
        #endregion

        #region Refresh Token Model Creation
        builder.Entity<RefreshToken>().ToTable("RefreshTokens").HasKey(r => r.Id);
        builder.Entity<RefreshToken>().Property(r => r.Id).HasColumnName("Id");
        builder.Entity<RefreshToken>().Property(r => r.UserId).HasColumnName("UserId");
        builder.Entity<RefreshToken>().Property(r => r.Token).HasColumnName("Token");
        builder.Entity<RefreshToken>().Property(r => r.Expires).HasColumnName("Expires");
        builder.Entity<RefreshToken>().Property(r => r.Created).HasColumnName("Created");
        builder.Entity<RefreshToken>().Property(r => r.CreatedByIp).HasColumnName("CreatedByIp");
        builder.Entity<RefreshToken>().Property(r => r.Revoked).HasColumnName("Revoked");
        builder.Entity<RefreshToken>().Property(r => r.RevokedByIp).HasColumnName("RevokedByIp");
        builder.Entity<RefreshToken>().Property(r => r.ReplacedByToken).HasColumnName("ReplacedByToken");
        builder.Entity<RefreshToken>().Property(r => r.ReasonRevoked).HasColumnName("ReasonRevoked");
        builder.Entity<RefreshToken>().HasOne(r => r.User);
        #endregion

        #region User Model Creation
        builder.Entity<User>().ToTable("Users").HasKey(k => k.Id);
        builder.Entity<User>().Property(u => u.Id).HasColumnName("Id").UseIdentityColumn(1, 1);
        builder.Entity<User>().Property(u => u.FirstName).HasColumnName("FirstName").HasMaxLength(50).IsRequired();
        builder.Entity<User>().Property(u => u.LastName).HasColumnName("LastName").HasMaxLength(50).IsRequired();
        builder.Entity<User>().Property(u => u.PhoneNumber).HasColumnName("PhoneNumber").HasMaxLength(50).IsRequired();
        builder.Entity<User>().Property(u => u.Address).HasColumnName("Address").HasMaxLength(50).IsRequired();
        builder.Entity<User>().Property(u => u.Email).HasColumnName("Email").HasMaxLength(200).IsRequired();
        builder.Entity<User>().Property(u => u.PasswordHash).HasColumnName("PasswordHash").HasColumnType("varbinary(500)").IsRequired();
        builder.Entity<User>().Property(u => u.PasswordSalt).HasColumnName("PasswordSalt").HasColumnType("varbinary(500)").IsRequired();
        builder.Entity<User>().Property(u => u.RegistrationDate).HasColumnName("RegistrationDate").IsRequired();
        builder.Entity<User>().Property(u => u.UserStatus).HasColumnName("UserStatus").IsRequired();
        builder.Entity<User>().Property(u => u.ImageUrl).HasColumnName("ImageUrl").IsRequired(false);

        builder.Entity<User>().HasMany(u => u.UserOperationClaims).WithOne(u => u.User);
        #endregion

        #region UserOperationClaim Model Creation
        builder.Entity<UserOperationClaim>().ToTable("UserOperationClaims").HasKey(u => u.Id);
        builder.Entity<UserOperationClaim>().Property(u => u.Id).HasColumnName("Id");
        builder.Entity<UserOperationClaim>().Property(u => u.UserId).HasColumnName("UserId").IsRequired();
        builder.Entity<UserOperationClaim>().Property(u => u.OperationClaimId).HasColumnName("OperationClaimId").IsRequired();

        builder.Entity<UserOperationClaim>().HasOne(u => u.User).WithMany(u => u.UserOperationClaims).HasForeignKey(u => u.UserId);
        builder.Entity<UserOperationClaim>().HasOne(u => u.OperationClaim).WithMany().HasForeignKey(u => u.OperationClaimId);
        #endregion

        #region Warehouse Model Creation
        builder.Entity<Warehouse>().ToTable("Warehouses").HasKey(u => u.Id);
        builder.Entity<Warehouse>().Property(u => u.Id).HasColumnName("Id");
        builder.Entity<Warehouse>().Property(u => u.SetupCost).HasColumnName("SetupCost").IsRequired();
        builder.Entity<Warehouse>().Property(u => u.Capacity).HasColumnName("Capacity").IsRequired();
        #endregion
    }
}
