using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Travel_Website_System_API_.Migrations
{
    /// <inheritdoc />
    public partial class update_IN_Payment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Category",
                columns: table => new
                {
                    categoryId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Category__23CAF1D8310AC031", x => x.categoryId);
                });

            migrationBuilder.CreateTable(
                name: "ServiceProviders",
                columns: table => new
                {
                    serviceProviderId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Logo = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__ServiceP__00FEF614563FA657", x => x.serviceProviderId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    userId = table.Column<int>(type: "int", nullable: false),
                    UserName = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Fname = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Lname = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    Gender = table.Column<string>(type: "char(1)", unicode: false, fixedLength: true, maxLength: 1, nullable: true),
                    SSN = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: true),
                    Password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Role = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__CB9A1CFF129F083A", x => x.userId);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    serviceId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Image = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    QuantityAvailable = table.Column<int>(type: "int", nullable: true),
                    StartDate = table.Column<DateTime>(type: "date", nullable: true),
                    price = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true),
                    serviceProviderId = table.Column<int>(type: "int", nullable: true),
                    categoryId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Services__455070DF92061F6A", x => x.serviceId);
                    table.ForeignKey(
                        name: "FK__Services__catego__4222D4EF",
                        column: x => x.categoryId,
                        principalTable: "Category",
                        principalColumn: "categoryId");
                    table.ForeignKey(
                        name: "FK__Services__servic__412EB0B6",
                        column: x => x.serviceProviderId,
                        principalTable: "ServiceProviders",
                        principalColumn: "serviceProviderId");
                });

            migrationBuilder.CreateTable(
                name: "Admin",
                columns: table => new
                {
                    adminId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Admin__AD0500A6EB9C0073", x => x.adminId);
                    table.ForeignKey(
                        name: "FK__Admin__userId__47DBAE45",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "Client",
                columns: table => new
                {
                    clientId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Client__81A2CBE1E24ECB7F", x => x.clientId);
                    table.ForeignKey(
                        name: "FK__Client__userId__3A81B327",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "CustomerService",
                columns: table => new
                {
                    customerServiceId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Customer__408B6EB754B8A0FE", x => x.customerServiceId);
                    table.ForeignKey(
                        name: "FK__CustomerS__userI__6B24EA82",
                        column: x => x.userId,
                        principalTable: "Users",
                        principalColumn: "userId");
                });

            migrationBuilder.CreateTable(
                name: "Package",
                columns: table => new
                {
                    packageId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Image = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    TotalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true),
                    startDate = table.Column<DateTime>(type: "date", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: true),
                    adminId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Package__AA8D20C8A785F2B5", x => x.packageId);
                    table.ForeignKey(
                        name: "FK__Package__adminId__4D94879B",
                        column: x => x.adminId,
                        principalTable: "Admin",
                        principalColumn: "adminId");
                });

            migrationBuilder.CreateTable(
                name: "BookingService",
                columns: table => new
                {
                    BookingServiceId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: true),
                    Data = table.Column<DateTime>(type: "date", nullable: true),
                    clientId = table.Column<int>(type: "int", nullable: true),
                    serviceId = table.Column<int>(type: "int", nullable: true),
                    allowingTime = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BookingS__43F55CB10F4AF1E4", x => x.BookingServiceId);
                    table.ForeignKey(
                        name: "FK__BookingSe__clien__5535A963",
                        column: x => x.clientId,
                        principalTable: "Client",
                        principalColumn: "clientId");
                    table.ForeignKey(
                        name: "FK__BookingSe__servi__5629CD9C",
                        column: x => x.serviceId,
                        principalTable: "Services",
                        principalColumn: "serviceId");
                });

            migrationBuilder.CreateTable(
                name: "LoveService",
                columns: table => new
                {
                    clientId = table.Column<int>(type: "int", nullable: false),
                    serviceId = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LoveServ__65F7CCEC15E2A4A1", x => new { x.clientId, x.serviceId });
                    table.ForeignKey(
                        name: "FK__LoveServi__clien__6477ECF3",
                        column: x => x.clientId,
                        principalTable: "Client",
                        principalColumn: "clientId");
                    table.ForeignKey(
                        name: "FK__LoveServi__servi__656C112C",
                        column: x => x.serviceId,
                        principalTable: "Services",
                        principalColumn: "serviceId");
                });

            migrationBuilder.CreateTable(
                name: "Chat",
                columns: table => new
                {
                    chatId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Logo = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: true),
                    customerServiceId = table.Column<int>(type: "int", nullable: true),
                    clientId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Chat__826385ADEA62FC88", x => x.chatId);
                    table.ForeignKey(
                        name: "FK__Chat__clientId__6EF57B66",
                        column: x => x.clientId,
                        principalTable: "Client",
                        principalColumn: "clientId");
                    table.ForeignKey(
                        name: "FK__Chat__customerSe__6E01572D",
                        column: x => x.customerServiceId,
                        principalTable: "CustomerService",
                        principalColumn: "customerServiceId");
                });

            migrationBuilder.CreateTable(
                name: "BookingPackage",
                columns: table => new
                {
                    BookingPackageId = table.Column<int>(type: "int", nullable: false),
                    Data = table.Column<DateTime>(type: "date", nullable: true),
                    quantity = table.Column<int>(type: "int", nullable: true),
                    clientId = table.Column<int>(type: "int", nullable: true),
                    packageId = table.Column<int>(type: "int", nullable: true),
                    allowingTime = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__BookingP__F74867E239B1827B", x => x.BookingPackageId);
                    table.ForeignKey(
                        name: "FK__BookingPa__clien__59063A47",
                        column: x => x.clientId,
                        principalTable: "Client",
                        principalColumn: "clientId");
                    table.ForeignKey(
                        name: "FK__BookingPa__packa__59FA5E80",
                        column: x => x.packageId,
                        principalTable: "Package",
                        principalColumn: "packageId");
                });

            migrationBuilder.CreateTable(
                name: "LovePackage",
                columns: table => new
                {
                    clientId = table.Column<int>(type: "int", nullable: false),
                    packageId = table.Column<int>(type: "int", nullable: false),
                    date = table.Column<DateTime>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__LovePack__1B0A19EDAB436C1A", x => new { x.clientId, x.packageId });
                    table.ForeignKey(
                        name: "FK__LovePacka__clien__60A75C0F",
                        column: x => x.clientId,
                        principalTable: "Client",
                        principalColumn: "clientId");
                    table.ForeignKey(
                        name: "FK__LovePacka__packa__619B8048",
                        column: x => x.packageId,
                        principalTable: "Package",
                        principalColumn: "packageId");
                });

            migrationBuilder.CreateTable(
                name: "PackageService",
                columns: table => new
                {
                    serviceId = table.Column<int>(type: "int", nullable: false),
                    packageId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__PackageS__DFF8A2D34D1E9D41", x => new { x.serviceId, x.packageId });
                    table.ForeignKey(
                        name: "FK__PackageSe__packa__52593CB8",
                        column: x => x.packageId,
                        principalTable: "Package",
                        principalColumn: "packageId");
                    table.ForeignKey(
                        name: "FK__PackageSe__servi__5165187F",
                        column: x => x.serviceId,
                        principalTable: "Services",
                        principalColumn: "serviceId");
                });

            migrationBuilder.CreateTable(
                name: "Message",
                columns: table => new
                {
                    messageId = table.Column<int>(type: "int", nullable: false),
                    status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    sender = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true),
                    chatId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Message__4808B993CF62E7D5", x => x.messageId);
                    table.ForeignKey(
                        name: "FK__Message__chatId__71D1E811",
                        column: x => x.chatId,
                        principalTable: "Chat",
                        principalColumn: "chatId");
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PaymentID = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    Method = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Currencey = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookingPackageId = table.Column<int>(type: "int", nullable: true),
                    BookingServiceId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Payment__9B556A58D50CDE2C", x => x.PaymentID);
                    table.ForeignKey(
                        name: "FK__Payment__Booking__5CD6CB2B",
                        column: x => x.BookingPackageId,
                        principalTable: "BookingPackage",
                        principalColumn: "BookingPackageId");
                    table.ForeignKey(
                        name: "FK__Payment__Booking__5DCAEF64",
                        column: x => x.BookingServiceId,
                        principalTable: "BookingService",
                        principalColumn: "BookingServiceId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admin_userId",
                table: "Admin",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPackage_clientId",
                table: "BookingPackage",
                column: "clientId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingPackage_packageId",
                table: "BookingPackage",
                column: "packageId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingService_clientId",
                table: "BookingService",
                column: "clientId");

            migrationBuilder.CreateIndex(
                name: "IX_BookingService_serviceId",
                table: "BookingService",
                column: "serviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_clientId",
                table: "Chat",
                column: "clientId");

            migrationBuilder.CreateIndex(
                name: "IX_Chat_customerServiceId",
                table: "Chat",
                column: "customerServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Client_userId",
                table: "Client",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerService_userId",
                table: "CustomerService",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_LovePackage_packageId",
                table: "LovePackage",
                column: "packageId");

            migrationBuilder.CreateIndex(
                name: "IX_LoveService_serviceId",
                table: "LoveService",
                column: "serviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Message_chatId",
                table: "Message",
                column: "chatId");

            migrationBuilder.CreateIndex(
                name: "IX_Package_adminId",
                table: "Package",
                column: "adminId");

            migrationBuilder.CreateIndex(
                name: "IX_PackageService_packageId",
                table: "PackageService",
                column: "packageId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_BookingPackageId",
                table: "Payment",
                column: "BookingPackageId");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_BookingServiceId",
                table: "Payment",
                column: "BookingServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_categoryId",
                table: "Services",
                column: "categoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_serviceProviderId",
                table: "Services",
                column: "serviceProviderId");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__CA1E8E3C7CEBD894",
                table: "Users",
                column: "SSN",
                unique: true,
                filter: "[SSN] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LovePackage");

            migrationBuilder.DropTable(
                name: "LoveService");

            migrationBuilder.DropTable(
                name: "Message");

            migrationBuilder.DropTable(
                name: "PackageService");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Chat");

            migrationBuilder.DropTable(
                name: "BookingPackage");

            migrationBuilder.DropTable(
                name: "BookingService");

            migrationBuilder.DropTable(
                name: "CustomerService");

            migrationBuilder.DropTable(
                name: "Package");

            migrationBuilder.DropTable(
                name: "Client");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Admin");

            migrationBuilder.DropTable(
                name: "Category");

            migrationBuilder.DropTable(
                name: "ServiceProviders");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
