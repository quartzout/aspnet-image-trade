using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Users.Migrations
{
#pragma warning disable CS8981 // Имя типа содержит только строчные символы ASCII. Такие имена могут резервироваться для языка.
    public partial class update : Migration
#pragma warning restore CS8981 // Имя типа содержит только строчные символы ASCII. Такие имена могут резервироваться для языка.
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BirthYear",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<float>(
                name: "AccountBalance",
                table: "AspNetUsers",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<string>(
                name: "DisplayName",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountBalance",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DisplayName",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<DateTime>(
                name: "BirthYear",
                table: "AspNetUsers",
                type: "date",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
