using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CommunicatorService.Migrations
{
    public partial class SanyaPidor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrivateMessage",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: true),
                    SendingDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ReceiptDateTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    SenderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RecipientId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrivateMessage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrivateMessage_User_RecipientId",
                        column: x => x.RecipientId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PrivateMessage_User_SenderId",
                        column: x => x.SenderId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsersRelation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Displayed = table.Column<bool>(type: "INTEGER", nullable: false),
                    Muted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Blocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    SubjectId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TargetId = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersRelation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsersRelation_User_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsersRelation_User_TargetId",
                        column: x => x.TargetId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessage_RecipientId",
                table: "PrivateMessage",
                column: "RecipientId");

            migrationBuilder.CreateIndex(
                name: "IX_PrivateMessage_SenderId",
                table: "PrivateMessage",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersRelation_SubjectId",
                table: "UsersRelation",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersRelation_TargetId",
                table: "UsersRelation",
                column: "TargetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrivateMessage");

            migrationBuilder.DropTable(
                name: "UsersRelation");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
