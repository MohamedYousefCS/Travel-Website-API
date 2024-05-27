using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travel_Website_System_API_.Migrations
{
    /// <inheritdoc />
    public partial class editPackageServiceBooking : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__BookingPa__clien__59063A47",
                table: "BookingPackage");

            migrationBuilder.DropForeignKey(
                name: "FK__BookingPa__packa__59FA5E80",
                table: "BookingPackage");

            migrationBuilder.DropForeignKey(
                name: "FK__BookingSe__clien__5535A963",
                table: "BookingService");

            migrationBuilder.DropForeignKey(
                name: "FK__BookingSe__servi__5629CD9C",
                table: "BookingService");

            migrationBuilder.DropIndex(
                name: "IX_BookingService_clientId",
                table: "BookingService");

            migrationBuilder.DropIndex(
                name: "IX_BookingPackage_clientId",
                table: "BookingPackage");

            migrationBuilder.RenameColumn(
                name: "BookingQuantity",
                table: "Package",
                newName: "QuantityAvailable");

            migrationBuilder.CreateIndex(
                name: "IX_BookingService_clientId_serviceId",
                table: "BookingService",
                columns: new[] { "clientId", "serviceId" },
                unique: true,
                filter: "[clientId] IS NOT NULL AND [serviceId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPackage_clientId_packageId",
                table: "BookingPackage",
                columns: new[] { "clientId", "packageId" },
                unique: true,
                filter: "[clientId] IS NOT NULL AND [packageId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK__BookingPa__clien__59063A47",
                table: "BookingPackage",
                column: "clientId",
                principalTable: "Client",
                principalColumn: "clientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__BookingPa__packa__59FA5E80",
                table: "BookingPackage",
                column: "packageId",
                principalTable: "Package",
                principalColumn: "packageId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__BookingSe__clien__5535A963",
                table: "BookingService",
                column: "clientId",
                principalTable: "Client",
                principalColumn: "clientId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK__BookingSe__servi__5629CD9C",
                table: "BookingService",
                column: "serviceId",
                principalTable: "Services",
                principalColumn: "serviceId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__BookingPa__clien__59063A47",
                table: "BookingPackage");

            migrationBuilder.DropForeignKey(
                name: "FK__BookingPa__packa__59FA5E80",
                table: "BookingPackage");

            migrationBuilder.DropForeignKey(
                name: "FK__BookingSe__clien__5535A963",
                table: "BookingService");

            migrationBuilder.DropForeignKey(
                name: "FK__BookingSe__servi__5629CD9C",
                table: "BookingService");

            migrationBuilder.DropIndex(
                name: "IX_BookingService_clientId_serviceId",
                table: "BookingService");

            migrationBuilder.DropIndex(
                name: "IX_BookingPackage_clientId_packageId",
                table: "BookingPackage");

            migrationBuilder.RenameColumn(
                name: "QuantityAvailable",
                table: "Package",
                newName: "BookingQuantity");

            migrationBuilder.CreateIndex(
                name: "IX_BookingService_clientId",
                table: "BookingService",
                column: "clientId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPackage_clientId",
                table: "BookingPackage",
                column: "clientId");

            migrationBuilder.AddForeignKey(
                name: "FK__BookingPa__clien__59063A47",
                table: "BookingPackage",
                column: "clientId",
                principalTable: "Client",
                principalColumn: "clientId");

            migrationBuilder.AddForeignKey(
                name: "FK__BookingPa__packa__59FA5E80",
                table: "BookingPackage",
                column: "packageId",
                principalTable: "Package",
                principalColumn: "packageId");

            migrationBuilder.AddForeignKey(
                name: "FK__BookingSe__clien__5535A963",
                table: "BookingService",
                column: "clientId",
                principalTable: "Client",
                principalColumn: "clientId");

            migrationBuilder.AddForeignKey(
                name: "FK__BookingSe__servi__5629CD9C",
                table: "BookingService",
                column: "serviceId",
                principalTable: "Services",
                principalColumn: "serviceId");
        }
    }
}
