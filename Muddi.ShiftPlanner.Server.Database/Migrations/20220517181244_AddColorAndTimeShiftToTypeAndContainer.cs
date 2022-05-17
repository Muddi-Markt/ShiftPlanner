using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Muddi.ShiftPlanner.Server.Database.Migrations
{
    public partial class AddColorAndTimeShiftToTypeAndContainer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_containers_shift_locations_shift_location_id",
                table: "containers");

            migrationBuilder.DropTable(
                name: "shift_framework_type_count");

            migrationBuilder.RenameColumn(
                name: "shift_location_id",
                table: "containers",
                newName: "shift_location_entity_id");

            migrationBuilder.RenameIndex(
                name: "ix_containers_shift_location_id",
                table: "containers",
                newName: "ix_containers_shift_location_entity_id");

            migrationBuilder.AddColumn<string>(
                name: "color",
                table: "shift_types",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "starting_time_shift",
                table: "shift_types",
                type: "interval",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "color",
                table: "containers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "shift_framework_type_count_entity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shift_framework_id = table.Column<Guid>(type: "uuid", nullable: false),
                    shift_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shift_framework_type_count_entity", x => x.id);
                    table.ForeignKey(
                        name: "fk_shift_framework_type_count_entity_shift_frameworks_shift_fra",
                        column: x => x.shift_framework_id,
                        principalTable: "shift_frameworks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_shift_framework_type_count_entity_shift_types_shift_type_id",
                        column: x => x.shift_type_id,
                        principalTable: "shift_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_shift_framework_type_count_entity_shift_framework_id",
                table: "shift_framework_type_count_entity",
                column: "shift_framework_id");

            migrationBuilder.CreateIndex(
                name: "ix_shift_framework_type_count_entity_shift_type_id",
                table: "shift_framework_type_count_entity",
                column: "shift_type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_containers_shift_locations_shift_location_entity_id",
                table: "containers",
                column: "shift_location_entity_id",
                principalTable: "shift_locations",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_containers_shift_locations_shift_location_entity_id",
                table: "containers");

            migrationBuilder.DropTable(
                name: "shift_framework_type_count_entity");

            migrationBuilder.DropColumn(
                name: "color",
                table: "shift_types");

            migrationBuilder.DropColumn(
                name: "starting_time_shift",
                table: "shift_types");

            migrationBuilder.DropColumn(
                name: "color",
                table: "containers");

            migrationBuilder.RenameColumn(
                name: "shift_location_entity_id",
                table: "containers",
                newName: "shift_location_id");

            migrationBuilder.RenameIndex(
                name: "ix_containers_shift_location_entity_id",
                table: "containers",
                newName: "ix_containers_shift_location_id");

            migrationBuilder.CreateTable(
                name: "shift_framework_type_count",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shift_framework_id = table.Column<Guid>(type: "uuid", nullable: false),
                    shift_type_id = table.Column<Guid>(type: "uuid", nullable: false),
                    count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shift_framework_type_count", x => x.id);
                    table.ForeignKey(
                        name: "fk_shift_framework_type_count_shift_frameworks_shift_framework_id",
                        column: x => x.shift_framework_id,
                        principalTable: "shift_frameworks",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_shift_framework_type_count_shift_types_shift_type_id",
                        column: x => x.shift_type_id,
                        principalTable: "shift_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_shift_framework_type_count_shift_framework_id",
                table: "shift_framework_type_count",
                column: "shift_framework_id");

            migrationBuilder.CreateIndex(
                name: "ix_shift_framework_type_count_shift_type_id",
                table: "shift_framework_type_count",
                column: "shift_type_id");

            migrationBuilder.AddForeignKey(
                name: "fk_containers_shift_locations_shift_location_id",
                table: "containers",
                column: "shift_location_id",
                principalTable: "shift_locations",
                principalColumn: "id");
        }
    }
}
