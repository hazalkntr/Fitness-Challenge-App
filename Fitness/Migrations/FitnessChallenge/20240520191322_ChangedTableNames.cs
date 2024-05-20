using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fitness.Migrations.FitnessChallenge
{
    /// <inheritdoc />
    public partial class ChangedTableNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "challenges",
                columns: table => new
                {
                    challengeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nchar(50)", fixedLength: true, maxLength: 50, nullable: true),
                    description = table.Column<string>(type: "nchar(100)", fixedLength: true, maxLength: 100, nullable: true),
                    startDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    endDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    isDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__chal__A0C649523BA50520", x => x.challengeId);
                });

            migrationBuilder.CreateTable(
                name: "challengeParticipants",
                columns: table => new
                {
                    participantId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<int>(type: "int", nullable: false),
                    challengeId = table.Column<int>(type: "int", nullable: false),
                    joinDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    progress = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__chal__4EE79210DC8DA1C5", x => x.participantId);
                    table.ForeignKey(
                        name: "FK__chall__chall__4E88ABD4",
                        column: x => x.challengeId,
                        principalTable: "challenges",
                        principalColumn: "challengeId");
                });

            migrationBuilder.CreateTable(
                name: "leaderboard",
                columns: table => new
                {
                    leaderboardId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    challengeId = table.Column<int>(type: "int", nullable: false),
                    userId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    rank = table.Column<int>(type: "int", nullable: true),
                    score = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__lead__3B9417B5E438283C", x => x.leaderboardId);
                    table.ForeignKey(
                        name: "FK__leade__chall__5165187F",
                        column: x => x.challengeId,
                        principalTable: "challenges",
                        principalColumn: "challengeId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_challengeParticipants_challengeId",
                table: "challengeParticipants",
                column: "challengeId");

            migrationBuilder.CreateIndex(
                name: "IX_leaderboard_challengeId",
                table: "leaderboard",
                column: "challengeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "challengeParticipants");

            migrationBuilder.DropTable(
                name: "leaderboard");

            migrationBuilder.DropTable(
                name: "challenges");
        }
    }
}
