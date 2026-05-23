using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Muddi.ShiftPlanner.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddBlockReasonToShift : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "block_reason",
                table: "shifts",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "block_reason",
                table: "shifts");
        }
    }
}
