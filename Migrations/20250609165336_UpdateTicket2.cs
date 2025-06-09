using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTicket2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    TicketID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RouteID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BaseFare = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BusType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FromStage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsRedeemed = table.Column<bool>(type: "bit", nullable: false),
                    MobileNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Passengers = table.Column<int>(type: "int", nullable: false),
                    RedeemedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    StagesTravelled = table.Column<int>(type: "int", nullable: false),
                    ToStage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalFare = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.TicketID);
                    table.ForeignKey(
                        name: "FK_Tickets_BusRoutes_RouteID",
                        column: x => x.RouteID,
                        principalTable: "BusRoutes",
                        principalColumn: "RouteID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_RouteID",
                table: "Tickets",
                column: "RouteID");
        }
    }
}
