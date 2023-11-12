using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebAPI.Models.Concrete;

namespace WebAPI.EntityConfiguration;

public class CustomerWarehouseCostConfiguration : IEntityTypeConfiguration<CustomerWarehouseCost>
{
    public void Configure(EntityTypeBuilder<CustomerWarehouseCost> builder)
    {
        #region CustomerWarehouseCost Model Creation
        builder.ToTable("CustomerWarehouseCosts").HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("Id");
        builder.Property(u => u.WarehouseID).HasColumnName("WarehouseID").IsRequired();
        builder.Property(u => u.CustomerId).HasColumnName("CustomerId").IsRequired();
        builder.Property(u => u.Cost).HasColumnName("Cost").IsRequired();


        builder.HasOne(u => u.Warehouse).WithMany().HasForeignKey(u => u.WarehouseID);
        builder.HasOne(u => u.Customer).WithMany().HasForeignKey(u => u.CustomerId);

        #endregion


        //CustomerWarehouseCost[] customerWarehouseCostSeeds =
        //{
        //    new(1,1,1,100.1),
        //    new(2,1,2,200.2),
        //    new(3,1,3,2000.3),
        //    new(4,2,1,100.4),
        //    new(5,2,2,200.5),
        //    new(6,2,3,2000.6),
        //    new(7,3,1,200.7),
        //    new(8,3,2,100.8),
        //    new(9,3,3,200.9),
        //    new(10,4,1,200.10),
        //    new(11,4,2,200.11),
        //    new(12,4,3,100.12)
        //};
        //builder.HasData(customerWarehouseCostSeeds);
    }
}