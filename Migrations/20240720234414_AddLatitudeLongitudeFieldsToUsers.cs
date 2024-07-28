using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrestamosCreciendo.Migrations
{
    /// <inheritdoc />
    public partial class AddLatitudeLongitudeFieldsToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "lat",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "lng",
                table: "Users",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "lat",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "lng",
                table: "Users");
        }
    }
}
