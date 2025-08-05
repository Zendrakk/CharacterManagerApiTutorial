using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CharacterManagerApiTutorial.Migrations
{
    /// <inheritdoc />
    public partial class Complete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CharacterMappings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FactionId = table.Column<int>(type: "int", nullable: false),
                    RaceId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CharacterMappings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Characters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Level = table.Column<int>(type: "int", nullable: false),
                    FactionId = table.Column<int>(type: "int", nullable: false),
                    RaceId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    RealmId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Characters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ClassTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FactionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FactionTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RaceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Realms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realms", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RefreshTokenExpiration = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CharacterMappings",
                columns: new[] { "Id", "ClassId", "FactionId", "RaceId" },
                values: new object[,]
                {
                    { 1, 1, 1, 1 },
                    { 2, 2, 1, 1 },
                    { 3, 3, 1, 1 },
                    { 4, 4, 1, 1 },
                    { 5, 5, 1, 1 },
                    { 6, 6, 1, 1 },
                    { 7, 1, 1, 2 },
                    { 8, 2, 1, 2 },
                    { 9, 5, 1, 2 },
                    { 10, 6, 1, 2 },
                    { 11, 2, 1, 3 },
                    { 12, 3, 1, 3 },
                    { 13, 4, 1, 3 },
                    { 14, 5, 1, 3 },
                    { 15, 1, 2, 4 },
                    { 16, 2, 2, 4 },
                    { 17, 4, 2, 4 },
                    { 18, 1, 2, 5 },
                    { 19, 2, 2, 5 },
                    { 20, 3, 2, 5 },
                    { 21, 4, 2, 5 },
                    { 22, 5, 2, 5 },
                    { 23, 1, 2, 6 },
                    { 24, 2, 2, 6 },
                    { 25, 3, 2, 6 },
                    { 26, 4, 2, 6 },
                    { 27, 5, 2, 6 },
                    { 28, 6, 2, 6 }
                });

            migrationBuilder.InsertData(
                table: "ClassTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Warrior" },
                    { 2, "Archer" },
                    { 3, "Wizard" },
                    { 4, "Ninja" },
                    { 5, "Medic" },
                    { 6, "Paladin" }
                });

            migrationBuilder.InsertData(
                table: "FactionTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Light Side" },
                    { 2, "Dark Side" }
                });

            migrationBuilder.InsertData(
                table: "RaceTypes",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "Human" },
                    { 2, "Dwarf" },
                    { 3, "Elf" },
                    { 4, "Orc" },
                    { 5, "Zombie" },
                    { 6, "Alien" }
                });

            migrationBuilder.InsertData(
                table: "Realms",
                columns: new[] { "Id", "Name", "Type" },
                values: new object[,]
                {
                    { 1, "Frostgard", "Neutral" },
                    { 2, "Barren Land", "Neutral" },
                    { 3, "Wraithwind", "PVP" },
                    { 4, "Amber Expanse", "Neutral" },
                    { 5, "Boiling Isle", "PVP" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CharacterMappings");

            migrationBuilder.DropTable(
                name: "Characters");

            migrationBuilder.DropTable(
                name: "ClassTypes");

            migrationBuilder.DropTable(
                name: "FactionTypes");

            migrationBuilder.DropTable(
                name: "RaceTypes");

            migrationBuilder.DropTable(
                name: "Realms");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
