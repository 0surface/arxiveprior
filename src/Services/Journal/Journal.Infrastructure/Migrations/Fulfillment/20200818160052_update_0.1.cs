using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Journal.Infrastructure.Migrations.Fulfillment
{
    public partial class update_01 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ProcessingTimeInMilliseconds",
                table: "Fulfillments",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "IsPending",
                table: "Fulfillments",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "Fulfillments",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "QueryFromDate",
                table: "Fulfillments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "QueryToDate",
                table: "Fulfillments",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsPending",
                table: "Fulfillments");

            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "Fulfillments");

            migrationBuilder.DropColumn(
                name: "QueryFromDate",
                table: "Fulfillments");

            migrationBuilder.DropColumn(
                name: "QueryToDate",
                table: "Fulfillments");

            migrationBuilder.AlterColumn<int>(
                name: "ProcessingTimeInMilliseconds",
                table: "Fulfillments",
                type: "int",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
