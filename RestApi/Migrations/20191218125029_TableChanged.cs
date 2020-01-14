using Microsoft.EntityFrameworkCore.Migrations;

namespace RestApi.Migrations
{
    public partial class TableChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Statistics",
                table: "Statistics");

            migrationBuilder.RenameTable(
                name: "Statistics",
                newName: "Statistic");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Statistic",
                table: "Statistic",
                column: "StatID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Statistic",
                table: "Statistic");

            migrationBuilder.RenameTable(
                name: "Statistic",
                newName: "Statistics");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Statistics",
                table: "Statistics",
                column: "StatID");
        }
    }
}
