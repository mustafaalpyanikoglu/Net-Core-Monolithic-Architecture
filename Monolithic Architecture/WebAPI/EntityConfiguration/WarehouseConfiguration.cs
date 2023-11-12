using WebAPI.Models.Concrete;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebAPI.EntityConfiguration;

public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        #region Warehouse Model Creation
        builder.ToTable("Warehouses").HasKey(u => u.Id);
        builder.Property(u => u.Id).HasColumnName("Id");
        builder.Property(u => u.SetupCost).HasColumnName("SetupCost").IsRequired();
        builder.Property(u => u.Capacity).HasColumnName("Capacity").IsRequired();

        #endregion


        Warehouse[] warehouseSeeds =
        {
            new(1, 100, 100.123),
            new(2, 100, 100.456),
            new(3, 500, 100.789)
        };
        builder.HasData(warehouseSeeds);
    }
}
