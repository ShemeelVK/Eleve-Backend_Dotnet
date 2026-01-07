using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleve_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddUserFriendlyOrderId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderReference",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderReference",
                table: "Orders");
        }
    }
}
