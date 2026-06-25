using FinTech.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTech.API.Data.Configurations;

public class PaymentScheduleConfiguration : IEntityTypeConfiguration<PaymentSchedule>
{
    public void Configure(EntityTypeBuilder<PaymentSchedule> builder)
    {
        builder.ToTable("PaymentSchedules");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.PaymentNumber).IsRequired();
        builder.Property(x => x.DueDate).IsRequired();
        builder.Property(x => x.TotalPayment).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Principal).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Interest).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.RemainingBalance).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(PaymentScheduleStatus.Pending)
            .IsRequired();

        builder.HasOne(x => x.Loan)
            .WithMany(x => x.PaymentSchedules)
            .HasForeignKey(x => x.LoanId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.LoanId);
        builder.HasIndex(x => new { x.LoanId, x.PaymentNumber }).IsUnique();
    }
}
