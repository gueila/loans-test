using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinTech.API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserPasswords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Users""
                SET ""PasswordHash"" = '11223344'
                WHERE ""Email"" IN ('juan@email.com', 'maria@email.com');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE ""Users""
                SET ""PasswordHash"" = '1234'
                WHERE ""Email"" IN ('juan@email.com', 'maria@email.com');
            ");
        }
    }
}
