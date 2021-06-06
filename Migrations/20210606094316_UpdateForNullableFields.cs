using Microsoft.EntityFrameworkCore.Migrations;

namespace RemindONServer.Migrations
{
    public partial class UpdateForNullableFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "RemindONDevices",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "RemindONDevices");
        }
    }
}
