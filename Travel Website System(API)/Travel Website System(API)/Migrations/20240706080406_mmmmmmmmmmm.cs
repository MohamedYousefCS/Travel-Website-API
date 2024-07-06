using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travel_Website_System_API_.Migrations
{
    /// <inheritdoc />
    public partial class mmmmmmmmmmm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComingFlightTime",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "GoingFlightTime",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Services");

            migrationBuilder.AlterColumn<int>(
                name: "HoltelLocation",
                table: "Services",
                type: "int",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOFAvailableRooms",
                table: "Services",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOFPersons",
                table: "Services",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NumberOFAvailableRooms",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "NumberOFPersons",
                table: "Services");

            migrationBuilder.AlterColumn<string>(
                name: "HoltelLocation",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ComingFlightTime",
                table: "Services",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "GoingFlightTime",
                table: "Services",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Services",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
