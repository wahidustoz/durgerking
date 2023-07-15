using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace durgerking.Data.Migrations
{
    /// <inheritdoc />
    public partial class Category : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 7, 15, 12, 37, 38, 142, DateTimeKind.Utc).AddTicks(9354),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2023, 7, 10, 11, 29, 16, 314, DateTimeKind.Utc).AddTicks(6376));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 7, 15, 12, 37, 38, 142, DateTimeKind.Utc).AddTicks(7779),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2023, 7, 10, 11, 29, 16, 314, DateTimeKind.Utc).AddTicks(6142));

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(125)", maxLength: 125, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Food" },
                    { 2, "Snack" },
                    { 3, "Drink" },
                    { 4, "Salad" },
                    { 5, "Set" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 7, 10, 11, 29, 16, 314, DateTimeKind.Utc).AddTicks(6376),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2023, 7, 15, 12, 37, 38, 142, DateTimeKind.Utc).AddTicks(9354));

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Users",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(2023, 7, 10, 11, 29, 16, 314, DateTimeKind.Utc).AddTicks(6142),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldDefaultValue: new DateTime(2023, 7, 15, 12, 37, 38, 142, DateTimeKind.Utc).AddTicks(7779));
        }
    }
}
