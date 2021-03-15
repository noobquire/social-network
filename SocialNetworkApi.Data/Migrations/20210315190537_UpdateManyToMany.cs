using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialNetworkApi.Data.Migrations
{
    public partial class UpdateManyToMany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserChat_AspNetUsers_UserId1",
                table: "UserChat");

            migrationBuilder.DropForeignKey(
                name: "FK_UserChat_Chats_ChatId1",
                table: "UserChat");

            migrationBuilder.DropIndex(
                name: "IX_UserChat_ChatId1",
                table: "UserChat");

            migrationBuilder.DropIndex(
                name: "IX_UserChat_UserId1",
                table: "UserChat");

            migrationBuilder.DropColumn(
                name: "ChatId1",
                table: "UserChat");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "UserChat");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ChatId1",
                table: "UserChat",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "UserChat",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserChat_ChatId1",
                table: "UserChat",
                column: "ChatId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserChat_UserId1",
                table: "UserChat",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_UserChat_AspNetUsers_UserId1",
                table: "UserChat",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserChat_Chats_ChatId1",
                table: "UserChat",
                column: "ChatId1",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
