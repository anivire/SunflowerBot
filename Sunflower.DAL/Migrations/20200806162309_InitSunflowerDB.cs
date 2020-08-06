using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Sunflower.Migrations
{
    public partial class InitSunflowerDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GuildId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MemberId = table.Column<ulong>(type: "INTEGER", nullable: false),
                    MemberUsername = table.Column<string>(type: "TEXT", nullable: true),
                    MemberSunCount = table.Column<int>(type: "INTEGER", nullable: false),
                    DailyCooldown = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserProfiles");
        }
    }
}
