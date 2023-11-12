using WebAPI.Models.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebAPI.EntityConfiguration;

public partial class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        #region User Model Creation
        builder.ToTable("Users").HasKey(k => k.Id);
        builder.Property(u => u.Id).HasColumnName("Id").UseIdentityColumn(1, 1);
        builder.Property(u => u.FirstName).HasColumnName("FirstName").HasMaxLength(50).IsRequired();
        builder.Property(u => u.LastName).HasColumnName("LastName").HasMaxLength(50).IsRequired();
        builder.Property(u => u.PhoneNumber).HasColumnName("PhoneNumber").HasMaxLength(50).IsRequired();
        builder.Property(u => u.Address).HasColumnName("Address").HasMaxLength(50).IsRequired();
        builder.Property(u => u.Email).HasColumnName("Email").HasMaxLength(200).IsRequired();
        builder.Property(u => u.PasswordHash).HasColumnName("PasswordHash").HasColumnType("varbinary(500)").IsRequired();
        builder.Property(u => u.PasswordSalt).HasColumnName("PasswordSalt").HasColumnType("varbinary(500)").IsRequired();
        builder.Property(u => u.RegistrationDate).HasColumnName("RegistrationDate").IsRequired();
        builder.Property(u => u.UserStatus).HasColumnName("UserStatus").IsRequired();
        builder.Property(u => u.ImageUrl).HasColumnName("ImageUrl").IsRequired(false);


        builder.HasMany(u => u.UserOperationClaims).WithOne(u => u.User);

        #endregion

        //User[] userSeeds =
        //{
        //    new User
        //    {
        //        Id = 1,
        //        FirstName = "John",
        //        LastName = "Doe",
        //        PhoneNumber = "555-123-4567",
        //        Address = "123 Main St",
        //        Email = "john@example.com",
        //        PasswordHash = Encoding.ASCII.GetBytes("password_hash_1"),
        //        PasswordSalt = Encoding.ASCII.GetBytes("password_salt_1"),
        //        RegistrationDate = DateTime.Now,
        //        UserStatus = true
        //    },
        //    new User
        //    {
        //        Id = 2,
        //        FirstName = "Jane",
        //        LastName = "Smith",
        //        PhoneNumber = "555-987-6543",
        //        Address = "456 Elm St",
        //        Email = "jane@example.com",
        //        PasswordHash = Encoding.ASCII.GetBytes("password_hash_2"),
        //        PasswordSalt = Encoding.ASCII.GetBytes("password_salt_2"),
        //        RegistrationDate = DateTime.Now,
        //        UserStatus = true
        //    },
        //    new User
        //    {
        //        Id = 3,
        //        FirstName = "Alice",
        //        LastName = "Johnson",
        //        PhoneNumber = "555-555-5555",
        //        Address = "789 Oak St",
        //        Email = "alice@example.com",
        //        PasswordHash = Encoding.ASCII.GetBytes("password_hash_3"),
        //        PasswordSalt = Encoding.ASCII.GetBytes("password_salt_3"),
        //        RegistrationDate = DateTime.Now,
        //        UserStatus = true
        //    },
        //    new User
        //    {
        //        Id = 4,
        //        FirstName = "Bob",
        //        LastName = "Brown",
        //        PhoneNumber = "555-111-2222",
        //        Address = "101 Pine St",
        //        Email = "bob@example.com",
        //        PasswordHash = Encoding.ASCII.GetBytes("password_hash_4"),
        //        PasswordSalt = Encoding.ASCII.GetBytes("password_salt_4"),
        //        RegistrationDate = DateTime.Now,
        //        UserStatus = true
        //    }
        //};
    }
}
