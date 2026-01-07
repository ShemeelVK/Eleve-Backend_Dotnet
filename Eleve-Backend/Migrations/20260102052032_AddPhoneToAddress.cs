using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Eleve_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPhoneToAddress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_Name",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ShippingAddress_PhoneNumber",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShippingAddress_Name",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ShippingAddress_PhoneNumber",
                table: "Orders");
        }
    }
}
