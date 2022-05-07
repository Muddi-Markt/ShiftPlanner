using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Muddi.ShiftPlanner.Server.Database.Migrations
{
    public partial class SetEntitiesToFrameworkTypeCount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_shift_framework_type_count_shift_frameworks_shift_framework",
                table: "shift_framework_type_count");

            migrationBuilder.CreateIndex(
                name: "ix_shift_framework_type_count_shift_type_id",
                table: "shift_framework_type_count",
                column: "shift_type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_shift_framework_type_count_shift_frameworks_shift_framework_id",
                table: "shift_framework_type_count",
                column: "shift_framework_id",
                principalTable: "shift_frameworks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_shift_framework_type_count_shift_types_shift_type_id",
                table: "shift_framework_type_count",
                column: "shift_type_id",
                principalTable: "shift_types",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_shift_framework_type_count_shift_frameworks_shift_framework_id",
                table: "shift_framework_type_count");

            migrationBuilder.DropForeignKey(
                name: "fk_shift_framework_type_count_shift_types_shift_type_id",
                table: "shift_framework_type_count");

            migrationBuilder.DropIndex(
                name: "ix_shift_framework_type_count_shift_type_id",
                table: "shift_framework_type_count");

            migrationBuilder.AddForeignKey(
                name: "fk_shift_framework_type_count_shift_frameworks_shift_framework",
                table: "shift_framework_type_count",
                column: "shift_framework_id",
                principalTable: "shift_frameworks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
