using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Muddi.ShiftPlanner.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class UpgradeDotnet9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_shift_framework_type_count_entity_shift_frameworks_shift_fra",
                table: "shift_framework_type_count_entity");

            migrationBuilder.AddForeignKey(
                name: "fk_shift_framework_type_count_entity_shift_frameworks_shift_fr",
                table: "shift_framework_type_count_entity",
                column: "shift_framework_id",
                principalTable: "shift_frameworks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_shift_framework_type_count_entity_shift_frameworks_shift_fr",
                table: "shift_framework_type_count_entity");

            migrationBuilder.AddForeignKey(
                name: "fk_shift_framework_type_count_entity_shift_frameworks_shift_fra",
                table: "shift_framework_type_count_entity",
                column: "shift_framework_id",
                principalTable: "shift_frameworks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
