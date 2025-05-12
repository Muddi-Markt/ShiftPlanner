using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Muddi.ShiftPlanner.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AdminOnlyLocationTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "admin_only",
                table: "shift_location_types",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "admin_only",
                table: "shift_location_types");
        }
    }
}
