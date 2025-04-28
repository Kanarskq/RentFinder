using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Bookings.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changesInProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Bathrooms",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Bedrooms",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "HasBalcony",
                table: "Properties",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "HasParking",
                table: "Properties",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PetsAllowed",
                table: "Properties",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PropertyType",
                table: "Properties",
                type: "varchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<float>(
                name: "SquareFootage",
                table: "Properties",
                type: "float",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<int>(
                name: "YearBuilt",
                table: "Properties",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PropertyImage",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PropertyId = table.Column<int>(type: "int", nullable: false),
                    ImageData = table.Column<byte[]>(type: "longblob", nullable: false),
                    ImageType = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Caption = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UploadedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PropertyImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PropertyImage_Properties_PropertyId",
                        column: x => x.PropertyId,
                        principalTable: "Properties",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyImage_PropertyId",
                table: "PropertyImage",
                column: "PropertyId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PropertyImage");

            migrationBuilder.DropColumn(
                name: "Bathrooms",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "Bedrooms",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "HasBalcony",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "HasParking",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "PetsAllowed",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "PropertyType",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "SquareFootage",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "YearBuilt",
                table: "Properties");
        }
    }
}
