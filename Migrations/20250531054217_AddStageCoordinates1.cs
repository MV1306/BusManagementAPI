using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddStageCoordinates1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "BusRouteStages");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "BusRouteStages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "BusRouteStages",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "BusRouteStages",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
