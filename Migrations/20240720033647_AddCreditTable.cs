using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace PrestamosCreciendo.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AgentHasClient",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Id_agent = table.Column<int>(type: "integer", nullable: false),
                    Id_client = table.Column<int>(type: "integer", nullable: false),
                    Id_wallet = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgentHasClient", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Credit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Amount_neto = table.Column<float>(type: "real", nullable: false),
                    Order_list = table.Column<int>(type: "integer", nullable: false),
                    Id_user = table.Column<int>(type: "integer", nullable: false),
                    Id_agent = table.Column<int>(type: "integer", nullable: false),
                    Payment_number = table.Column<int>(type: "integer", nullable: false),
                    Utility = table.Column<float>(type: "real", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credit", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgentHasClient");

            migrationBuilder.DropTable(
                name: "Credit");
        }
    }
}
