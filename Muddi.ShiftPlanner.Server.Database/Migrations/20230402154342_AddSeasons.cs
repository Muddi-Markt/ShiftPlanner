using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Muddi.ShiftPlanner.Server.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSeasons : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var guidForFirstEntity = Guid.NewGuid();
            migrationBuilder.AddColumn<Guid>(
                name: "season_id",
                table: "shift_types",
                type: "uuid",
                nullable: false,
                defaultValue: guidForFirstEntity);

            migrationBuilder.AddColumn<Guid>(
                name: "season_id",
                table: "shift_locations",
                type: "uuid",
                nullable: false,
                defaultValue: guidForFirstEntity);

            migrationBuilder.AddColumn<Guid>(
                name: "season_id",
                table: "shift_frameworks",
                type: "uuid",
                nullable: false,
                defaultValue: guidForFirstEntity);

            migrationBuilder.CreateTable(
                name: "seasons",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_seasons", x => x.id);
                });

            migrationBuilder.InsertData("seasons", new[] { "id", "name", "start_date", "end_date" }, 
                new object[,]
                {
                    { 
                        guidForFirstEntity, 
                        "Kieler Woche 2022", 
                        new DateTime(2022, 06, 16, 0,0,0,DateTimeKind.Utc), 
                        new DateTime(2022, 06, 30, 0,0,0,DateTimeKind.Utc) 
                    }
                });

            migrationBuilder.CreateIndex(
                name: "ix_shift_types_season_id",
                table: "shift_types",
                column: "season_id");

            migrationBuilder.CreateIndex(
                name: "ix_shift_locations_season_id",
                table: "shift_locations",
                column: "season_id");

            migrationBuilder.CreateIndex(
                name: "ix_shift_frameworks_season_id",
                table: "shift_frameworks",
                column: "season_id");

            migrationBuilder.AddForeignKey(
                name: "fk_shift_frameworks_seasons_season_id",
                table: "shift_frameworks",
                column: "season_id",
                principalTable: "seasons",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_shift_locations_seasons_season_id",
                table: "shift_locations",
                column: "season_id",
                principalTable: "seasons",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_shift_types_seasons_season_id",
                table: "shift_types",
                column: "season_id",
                principalTable: "seasons",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_shift_frameworks_seasons_season_id",
                table: "shift_frameworks");

            migrationBuilder.DropForeignKey(
                name: "fk_shift_locations_seasons_season_id",
                table: "shift_locations");

            migrationBuilder.DropForeignKey(
                name: "fk_shift_types_seasons_season_id",
                table: "shift_types");

            migrationBuilder.DropTable(
                name: "seasons");

            migrationBuilder.DropIndex(
                name: "ix_shift_types_season_id",
                table: "shift_types");

            migrationBuilder.DropIndex(
                name: "ix_shift_locations_season_id",
                table: "shift_locations");

            migrationBuilder.DropIndex(
                name: "ix_shift_frameworks_season_id",
                table: "shift_frameworks");

            migrationBuilder.DropColumn(
                name: "season_id",
                table: "shift_types");

            migrationBuilder.DropColumn(
                name: "season_id",
                table: "shift_locations");

            migrationBuilder.DropColumn(
                name: "season_id",
                table: "shift_frameworks");
        }
    }
}
