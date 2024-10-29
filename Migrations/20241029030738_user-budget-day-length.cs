using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace web_api.Migrations
{
    /// <inheritdoc />
    public partial class userbudgetdaylength : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BudgetEndDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "BudgetStartDate",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "BudgetDayLength",
                table: "Users",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BudgetDayLength",
                table: "Users");

            migrationBuilder.AddColumn<DateTime>(
                name: "BudgetEndDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "BudgetStartDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
