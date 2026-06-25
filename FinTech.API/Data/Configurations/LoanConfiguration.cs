using FinTech.API.Enums;
using FinTech.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FinTech.API.Data.Configurations;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.ToTable("Loans");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();

        builder.Property(x => x.Amount).HasColumnType("decimal(12,2)").IsRequired();
        builder.Property(x => x.Term).IsRequired();
        builder.Property(x => x.InterestRate).HasColumnType("decimal(5,4)").IsRequired();
        builder.Property(x => x.LoanType)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();
        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .HasDefaultValue(LoanStatus.Pending)
            .IsRequired();
        builder.Property(x => x.MonthlyPayment).HasColumnType("decimal(12,2)");
        builder.Property(x => x.CreatedAt).HasDefaultValueSql("NOW()").IsRequired();
        builder.Property(x => x.UpdatedAt);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Loans)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.UserId);

        builder.ToTable(t => t.HasCheckConstraint("CK_Loan_Amount", "\"Amount\" >= 500 AND \"Amount\" <= 50000"));
        builder.ToTable(t => t.HasCheckConstraint("CK_Loan_Term", "\"Term\" >= 6 AND \"Term\" <= 60"));
        builder.ToTable(t => t.HasCheckConstraint("CK_Loan_InterestRate", "\"InterestRate\" >= 0.18 AND \"InterestRate\" <= 0.35"));
    }
}
