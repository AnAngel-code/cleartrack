using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketSystem.Migrations
{
    /// <inheritdoc />
    public partial class RemoveActionFromHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Action",
                table: "TicketHistories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Action",
                table: "TicketHistories",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
