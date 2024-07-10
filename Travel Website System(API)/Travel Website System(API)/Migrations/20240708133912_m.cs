using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travel_Website_System_API_.Migrations
{
    /// <inheritdoc />
    public partial class m : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ComingBackFlightDesination",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "ComingBackFlightSource",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "GoingFlightDestination",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "GoingFlightSource",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "HoltelLocation",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "NumberOFAvailableRooms",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "NumberOFPersons",
                table: "Services");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ComingBackFlightDesination",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ComingBackFlightSource",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoingFlightDestination",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GoingFlightSource",
                table: "Services",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HoltelLocation",
                table: "Services",
                type: "int",
                nullable: true);

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
    }
}
