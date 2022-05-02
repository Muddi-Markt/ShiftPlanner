using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Muddi.ShiftPlanner.Server.Database.Migrations
{
    public partial class AddLocations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "shift_location_id",
                table: "containers",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "shift_location_types",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shift_location_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shift_locations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    type_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shift_locations", x => x.id);
                    table.ForeignKey(
                        name: "fk_shift_locations_shift_location_types_type_id",
                        column: x => x.type_id,
                        principalTable: "shift_location_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_containers_shift_location_id",
                table: "containers",
                column: "shift_location_id");

            migrationBuilder.CreateIndex(
                name: "ix_shift_locations_type_id",
                table: "shift_locations",
                column: "type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_containers_shift_locations_shift_location_id",
                table: "containers",
                column: "shift_location_id",
                principalTable: "shift_locations",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_containers_shift_locations_shift_location_id",
                table: "containers");

            migrationBuilder.DropTable(
                name: "shift_locations");

            migrationBuilder.DropTable(
                name: "shift_location_types");

            migrationBuilder.DropIndex(
                name: "ix_containers_shift_location_id",
                table: "containers");

            migrationBuilder.DropColumn(
                name: "shift_location_id",
                table: "containers");
        }
    }
}
