using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinkShortener.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnQrCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "QrCode",
                table: "Links",
                type: "BLOB",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QrCode",
                table: "Links");
        }
    }
}
