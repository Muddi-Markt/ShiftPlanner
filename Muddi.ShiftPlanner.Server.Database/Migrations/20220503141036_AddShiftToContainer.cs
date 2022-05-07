using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Muddi.ShiftPlanner.Server.Database.Migrations
{
    public partial class AddShiftToContainer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_containers_shift_frameworks_shift_framework_id",
                table: "containers");

            migrationBuilder.RenameColumn(
                name: "time_per_shift",
                table: "shift_frameworks",
                newName: "seconds_per_shift");

            migrationBuilder.RenameColumn(
                name: "shift_framework_id",
                table: "containers",
                newName: "framework_id");

            migrationBuilder.RenameIndex(
                name: "ix_containers_shift_framework_id",
                table: "containers",
                newName: "ix_containers_framework_id");

            migrationBuilder.AddColumn<Guid>(
                name: "shift_container_id",
                table: "shifts",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_shifts_shift_container_id",
                table: "shifts",
                column: "shift_container_id");

            migrationBuilder.AddForeignKey(
                name: "fk_containers_shift_frameworks_framework_id",
                table: "containers",
                column: "framework_id",
                principalTable: "shift_frameworks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_shifts_containers_shift_container_id",
                table: "shifts",
                column: "shift_container_id",
                principalTable: "containers",
                principalColumn: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_containers_shift_frameworks_framework_id",
                table: "containers");

            migrationBuilder.DropForeignKey(
                name: "fk_shifts_containers_shift_container_id",
                table: "shifts");

            migrationBuilder.DropIndex(
                name: "ix_shifts_shift_container_id",
                table: "shifts");

            migrationBuilder.DropColumn(
                name: "shift_container_id",
                table: "shifts");

            migrationBuilder.RenameColumn(
                name: "seconds_per_shift",
                table: "shift_frameworks",
                newName: "time_per_shift");

            migrationBuilder.RenameColumn(
                name: "framework_id",
                table: "containers",
                newName: "shift_framework_id");

            migrationBuilder.RenameIndex(
                name: "ix_containers_framework_id",
                table: "containers",
                newName: "ix_containers_shift_framework_id");

            migrationBuilder.AddForeignKey(
                name: "fk_containers_shift_frameworks_shift_framework_id",
                table: "containers",
                column: "shift_framework_id",
                principalTable: "shift_frameworks",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
