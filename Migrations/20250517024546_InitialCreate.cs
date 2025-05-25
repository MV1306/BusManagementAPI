using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.CreateTable(
            //     name: "BusRoutes",
            //     columns: table => new
            //     {
            //         RouteID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //         RouteCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //         StartingPoint = table.Column<string>(type: "nvarchar(100)", nullable: false),
            //         EndingPoint = table.Column<string>(type: "nvarchar(100)", nullable: false),
            //         IsActive = table.Column<bool>(type: "bit", nullable: false),
            //         CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //         ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //         DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_BusRoutes", x => x.RouteID);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "FareMasters",
            //     columns: table => new
            //     {
            //         FareID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //         BusType = table.Column<string>(type: "nvarchar(max)", nullable: false),
            //         BaseFare = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
            //         FarePerStage = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
            //         IsActive = table.Column<bool>(type: "bit", nullable: false),
            //         CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //         ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //         DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_FareMasters", x => x.FareID);
            //     });

            // migrationBuilder.CreateTable(
            //     name: "BusRouteStages",
            //     columns: table => new
            //     {
            //         StageID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //         RouteID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
            //         StageName = table.Column<string>(type: "nvarchar(100)", nullable: false),
            //         StageOrder = table.Column<int>(type: "int", nullable: false),
            //         DistanceFromStart = table.Column<double>(type: "float", nullable: false),
            //         IsActive = table.Column<bool>(type: "bit", nullable: false),
            //         CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
            //         ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
            //         DeletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
            //     },
            //     constraints: table =>
            //     {
            //         table.PrimaryKey("PK_BusRouteStages", x => x.StageID);
            //         table.ForeignKey(
            //             name: "FK_BusRouteStages_BusRoutes_RouteID",
            //             column: x => x.RouteID,
            //             principalTable: "BusRoutes",
            //             principalColumn: "RouteID",
            //             onDelete: ReferentialAction.Cascade);
            //     });

            // migrationBuilder.CreateIndex(
            //     name: "IX_BusRouteStages_RouteID",
            //     table: "BusRouteStages",
            //     column: "RouteID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BusRouteStages");

            migrationBuilder.DropTable(
                name: "FareMasters");

            migrationBuilder.DropTable(
                name: "BusRoutes");
        }
    }
}
