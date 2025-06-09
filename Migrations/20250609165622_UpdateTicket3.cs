using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTicket3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Fare",
                table: "Tickets",
                newName: "TotalFare");

            migrationBuilder.AddColumn<decimal>(
                name: "BaseFare",
                table: "Tickets",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Passengers",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BaseFare",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Passengers",
                table: "Tickets");

            migrationBuilder.RenameColumn(
                name: "TotalFare",
                table: "Tickets",
                newName: "Fare");
        }
    }
}
