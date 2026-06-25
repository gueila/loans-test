using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FinTech.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    MonthlyIncome = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Loans",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    Term = table.Column<int>(type: "integer", nullable: false),
                    InterestRate = table.Column<decimal>(type: "numeric(5,4)", nullable: false),
                    LoanType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    MonthlyPayment = table.Column<decimal>(type: "numeric(12,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Loans", x => x.Id);
                    table.CheckConstraint("CK_Loan_Amount", "\"Amount\" >= 500 AND \"Amount\" <= 50000");
                    table.CheckConstraint("CK_Loan_InterestRate", "\"InterestRate\" >= 0.18 AND \"InterestRate\" <= 0.35");
                    table.CheckConstraint("CK_Loan_Term", "\"Term\" >= 6 AND \"Term\" <= 60");
                    table.ForeignKey(
                        name: "FK_Loans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PaymentSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LoanId = table.Column<Guid>(type: "uuid", nullable: false),
                    PaymentNumber = table.Column<int>(type: "integer", nullable: false),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalPayment = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    Principal = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    Interest = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    RemainingBalance = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentSchedules_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IdempotencyKey = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(12,2)", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Pending"),
                    LoanId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transactions_Loans_LoanId",
                        column: x => x.LoanId,
                        principalTable: "Loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Transactions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "Email", "MonthlyIncome", "Name", "PasswordHash" },
                values: new object[,]
                {
                    { new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "juan@email.com", 5000.00m, "Juan Pérez", "$2a$11$placeholder" },
                    { new Guid("b2c3d4e5-f6a7-8901-bcde-f12345678901"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "maria@email.com", 8000.00m, "María García", "$2a$11$placeholder" }
                });

            migrationBuilder.InsertData(
                table: "Loans",
                columns: new[] { "Id", "Amount", "CreatedAt", "InterestRate", "LoanType", "MonthlyPayment", "Status", "Term", "UpdatedAt", "UserId" },
                values: new object[] { new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), 5000.00m, new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 0.24m, "Fixed", 467.26m, "Active", 12, null, new Guid("a1b2c3d4-e5f6-7890-abcd-ef1234567890") });

            migrationBuilder.InsertData(
                table: "PaymentSchedules",
                columns: new[] { "Id", "DueDate", "Interest", "LoanId", "PaymentNumber", "Principal", "RemainingBalance", "TotalPayment" },
                values: new object[,]
                {
                    { new Guid("a3b4c5d6-e7f8-9012-3456-789012345678"), new DateTime(2025, 11, 15, 0, 0, 0, 0, DateTimeKind.Utc), 24.44m, new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), 10, 442.82m, 909.64m, 467.26m },
                    { new Guid("a7b8c9d0-e1f2-3456-7890-123456789abc"), new DateTime(2025, 5, 15, 0, 0, 0, 0, DateTimeKind.Utc), 69.61m, new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), 4, 397.65m, 3451.28m, 467.26m },
                    { new Guid("b4c5d6e7-f8a9-0123-4567-890123456789"), new DateTime(2025, 12, 15, 0, 0, 0, 0, DateTimeKind.Utc), 16.43m, new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), 11, 450.83m, 458.81m, 467.26m },
                    { new Guid("b8c9d0e1-f2a3-4567-8901-234567890abc"), new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Utc), 62.41m, new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), 5, 404.85m, 3046.43m, 467.26m },
                    { new Guid("c5d6e7f8-a9b0-1234-5678-901234567890"), new DateTime(2026, 1, 15, 0, 0, 0, 0, DateTimeKind.Utc), 8.30m, new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), 12, 458.81m, 0.00m, 467.26m },
                    { new Guid("c9d0e1f2-a3b4-5678-9012-345678901abc"), new DateTime(2025, 7, 15, 0, 0, 0, 0, DateTimeKind.Utc), 55.09m, new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), 6, 412.17m, 2634.26m, 467.26m },
                    { new Guid("d0e1f2a3-b4c5-6789-0123-45678901abcd"), new DateTime(2025, 8, 15, 0, 0, 0, 0, DateTimeKind.Utc), 47.63m, new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), 7, 419.63m, 2214.63m, 467.26m }
                });

            migrationBuilder.InsertData(
                table: "PaymentSchedules",
                columns: new[] { "Id", "DueDate", "Interest", "LoanId", "PaymentNumber", "Principal", "RemainingBalance", "Status", "TotalPayment" },
                values: new object[] { new Guid("d4e5f6a7-b8c9-0123-def4-567890123456"), new DateTime(2025, 2, 15, 0, 0, 0, 0, DateTimeKind.Utc), 90.43m, new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), 1, 376.83m, 4623.17m, "Paid", 467.26m });

            migrationBuilder.InsertData(
                table: "PaymentSchedules",
                columns: new[] { "Id", "DueDate", "Interest", "LoanId", "PaymentNumber", "Principal", "RemainingBalance", "TotalPayment" },
                values: new object[] { new Guid("e1f2a3b4-c5d6-7890-1234-5678901abcde"), new DateTime(2025, 9, 15, 0, 0, 0, 0, DateTimeKind.Utc), 40.04m, new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), 8, 427.22m, 1787.41m, 467.26m });

            migrationBuilder.InsertData(
                table: "PaymentSchedules",
                columns: new[] { "Id", "DueDate", "Interest", "LoanId", "PaymentNumber", "Principal", "RemainingBalance", "Status", "TotalPayment" },
                values: new object[] { new Guid("e5f6a7b8-c9d0-1234-ef56-789012345678"), new DateTime(2025, 3, 15, 0, 0, 0, 0, DateTimeKind.Utc), 83.61m, new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), 2, 383.65m, 4239.52m, "Paid", 467.26m });

            migrationBuilder.InsertData(
                table: "PaymentSchedules",
                columns: new[] { "Id", "DueDate", "Interest", "LoanId", "PaymentNumber", "Principal", "RemainingBalance", "TotalPayment" },
                values: new object[,]
                {
                    { new Guid("f2a3b4c5-d6e7-8901-2345-678901abcdef"), new DateTime(2025, 10, 15, 0, 0, 0, 0, DateTimeKind.Utc), 32.31m, new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), 9, 434.95m, 1352.46m, 467.26m },
                    { new Guid("f6a7b8c9-d0e1-2345-f678-901234567890"), new DateTime(2025, 4, 15, 0, 0, 0, 0, DateTimeKind.Utc), 76.67m, new Guid("c3d4e5f6-a7b8-9012-cdef-123456789012"), 3, 390.59m, 3848.93m, 467.26m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Loans_UserId",
                table: "Loans",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSchedules_LoanId",
                table: "PaymentSchedules",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentSchedules_LoanId_PaymentNumber",
                table: "PaymentSchedules",
                columns: new[] { "LoanId", "PaymentNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_IdempotencyKey",
                table: "Transactions",
                column: "IdempotencyKey",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_LoanId",
                table: "Transactions",
                column: "LoanId");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Status",
                table: "Transactions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_Type",
                table: "Transactions",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_UserId",
                table: "Transactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentSchedules");

            migrationBuilder.DropTable(
                name: "Transactions");

            migrationBuilder.DropTable(
                name: "Loans");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
