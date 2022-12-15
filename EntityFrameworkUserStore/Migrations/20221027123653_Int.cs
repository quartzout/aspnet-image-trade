using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RazorPages.Migrations
{
    public partial class Int : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountBalance",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<int>(
                name: "CoinBalance",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CoinBalance",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<float>(
                name: "AccountBalance",
                table: "AspNetUsers",
                type: "real",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
