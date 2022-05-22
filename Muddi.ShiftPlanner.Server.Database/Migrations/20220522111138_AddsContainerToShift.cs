using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Muddi.ShiftPlanner.Server.Database.Migrations
{
    public partial class AddsContainerToShift : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_shifts_containers_shift_container_id",
                table: "shifts");

            migrationBuilder.AlterColumn<Guid>(
                name: "shift_container_id",
                table: "shifts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "fk_shifts_containers_shift_container_id",
                table: "shifts",
                column: "shift_container_id",
                principalTable: "containers",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_shifts_containers_shift_container_id",
                table: "shifts");

            migrationBuilder.AlterColumn<Guid>(
                name: "shift_container_id",
                table: "shifts",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "fk_shifts_containers_shift_container_id",
                table: "shifts",
                column: "shift_container_id",
                principalTable: "containers",
                principalColumn: "id");
        }
    }
}
