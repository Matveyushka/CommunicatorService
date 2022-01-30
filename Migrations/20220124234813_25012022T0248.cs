using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunicatorService.Migrations
{
    public partial class _25012022T0248 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Deleted",
                table: "PrivateMessage",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deleted",
                table: "PrivateMessage");
        }
    }
}
