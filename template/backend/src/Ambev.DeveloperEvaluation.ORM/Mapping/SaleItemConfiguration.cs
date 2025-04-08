using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");

        builder.HasKey(si => si.Id);

        builder.Property(si => si.Id)
            .HasColumnType("uuid")
            .ValueGeneratedOnAdd();

        builder.Property(si => si.SaleId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(si => si.ProductId)
            .HasColumnType("uuid")
            .IsRequired();

        builder.Property(si => si.ProductName)
            .HasColumnType("text")
            .IsRequired();

        builder.Property(si => si.Quantity)
            .HasColumnType("integer")
            .IsRequired();

        builder.Property(si => si.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(si => si.Discount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(si => si.TotalValue)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasOne<Sale>()
            .WithMany(s => s.Items)
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}