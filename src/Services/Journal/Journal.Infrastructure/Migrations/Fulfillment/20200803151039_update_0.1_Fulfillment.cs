using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Journal.Infrastructure.Migrations.Fulfillment
{
    public partial class update_01_Fulfillment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Fulfillments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventId = table.Column<string>(nullable: true),
                    ExtractionFulfillmentId = table.Column<string>(nullable: true),
                    JournalType = table.Column<int>(nullable: false),
                    ArticlesCount = table.Column<int>(nullable: false),
                    InsertedCount = table.Column<int>(nullable: false),
                    UpdatedCount = table.Column<int>(nullable: false),
                    TotalProcessedCount = table.Column<int>(nullable: false),
                    ProcessingTimeInMilliseconds = table.Column<int>(nullable: false),
                    JobStartedDate = table.Column<DateTime>(nullable: false),
                    JobCompletedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fulfillments", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fulfillments");
        }
    }
}
