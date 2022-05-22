using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Muddi.ShiftPlanner.Server.Database.Migrations
{
    public partial class RenamesLocationInContainer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_containers_shift_locations_shift_location_entity_id",
                table: "containers");

            migrationBuilder.RenameColumn(
                name: "shift_location_entity_id",
                table: "containers",
                newName: "location_id");

            migrationBuilder.RenameIndex(
                name: "ix_containers_shift_location_entity_id",
                table: "containers",
                newName: "ix_containers_location_id");

            migrationBuilder.AddForeignKey(
                name: "fk_containers_shift_locations_location_id",
                table: "containers",
                column: "location_id",
                principalTable: "shift_locations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_containers_shift_locations_location_id",
                table: "containers");

            migrationBuilder.RenameColumn(
                name: "location_id",
                table: "containers",
                newName: "shift_location_entity_id");

            migrationBuilder.RenameIndex(
                name: "ix_containers_location_id",
                table: "containers",
                newName: "ix_containers_shift_location_entity_id");

            migrationBuilder.AddForeignKey(
                name: "fk_containers_shift_locations_shift_location_entity_id",
                table: "containers",
                column: "shift_location_entity_id",
                principalTable: "shift_locations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
