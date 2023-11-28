using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Udemy.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addCompanyTableToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StreetAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.Id);
                });

			migrationBuilder.AddColumn<int>(
			   name: "CompanyId",
			   table: "AspNetUsers",
			   type: "int",
			   nullable: true);

			migrationBuilder.CreateIndex(
				name: "IX_AspNetUsers_CompanyId",
				table: "AspNetUsers",
				column: "CompanyId");

			migrationBuilder.AddForeignKey(
				name: "FK_AspNetUsers_Companies_CompanyId",
				table: "AspNetUsers",
				column: "CompanyId",
				principalTable: "Companies",
				principalColumn: "Id",
				onDelete: ReferentialAction.Cascade);


           
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
           
        }
    }
}
