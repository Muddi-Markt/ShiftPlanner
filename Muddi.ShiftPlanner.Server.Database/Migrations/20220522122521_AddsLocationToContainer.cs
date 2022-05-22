using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Muddi.ShiftPlanner.Server.Database.Migrations
{
    public partial class AddsLocationToContainer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_containers_shift_locations_shift_location_entity_id",
                table: "containers");

            migrationBuilder.AlterColumn<Guid>(
                name: "shift_location_entity_id",
                table: "containers",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_containers_shift_locations_shift_location_entity_id",
                table: "containers",
                column: "shift_location_entity_id",
                principalTable: "shift_locations",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_containers_shift_locations_shift_location_entity_id",
                table: "containers");

            migrationBuilder.AlterColumn<Guid>(
                name: "shift_location_entity_id",
                table: "containers",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "fk_containers_shift_locations_shift_location_entity_id",
                table: "containers",
                column: "shift_location_entity_id",
                principalTable: "shift_locations",
                principalColumn: "id");
        }
    }
}
