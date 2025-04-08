using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnType("uuid")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.SaleNumber)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(s => s.SaleDate)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        builder.Property(s => s.Client)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(s => s.Branch)
            .IsRequired()
            .HasColumnType("text");

        builder.Property(s => s.IsCancelled)
            .HasColumnType("boolean")
            .IsRequired();

        builder.Property(s => s.TotalValue)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.HasMany(s => s.Items)
            .WithOne()
            .HasForeignKey(si => si.SaleId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}