using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPI.Models.Concrete;

namespace WebAPI.EntityConfiguration;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        #region Customer Model Creation
        builder.ToTable("Customers").HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("Id");
        builder.Property(u => u.UserID).HasColumnName("UserId").IsRequired();
        builder.Property(u => u.Demand).HasColumnName("Demand").IsRequired();

        builder.HasOne(u => u.User).WithOne(u => u.Customer).HasForeignKey<Customer>(u => u.UserID);
        #endregion

        //Customer[] customerSeeds =
        //{
        //        new(1, 1, 50),
        //        new(2, 2, 50),
        //        new(3, 3, 75),
        //        new(4, 4, 75)
        //    };
        //builder.HasData(customerSeeds);
    }
}

