using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travel_Website_System_API_.Migrations
{
    /// <inheritdoc />
    public partial class mmmmmm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HoltelLocation",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPrice",
                table: "Services",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FirstLocation",
                table: "Packages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "FirstLocationDuration",
                table: "Packages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecondLocation",
                table: "Packages",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SecondLocationDuration",
                table: "Packages",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoltelLocation",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "FirstLocation",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "FirstLocationDuration",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "SecondLocation",
                table: "Packages");

            migrationBuilder.DropColumn(
                name: "SecondLocationDuration",
                table: "Packages");
        }
    }
}
