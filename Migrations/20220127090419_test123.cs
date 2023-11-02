using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DF_EvolutionAPI.Migrations
{
    public partial class test123 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dfev");

            migrationBuilder.CreateTable(
                name: "KRAWeightages",
                schema: "dfev",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    displayname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    kraid = table.Column<int>(type: "int", nullable: false),
                    isactive = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    createby = table.Column<int>(type: "int", nullable: false),
                    updateby = table.Column<int>(type: "int", nullable: false),
                    createdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    updatedate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KRAWeightages", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KRAWeightages",
                schema: "dfev");
        }
    }
}
