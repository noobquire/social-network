using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SocialNetworkApi.Data.Migrations
{
    public partial class ChatModelAndFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileId",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<bool>(
                name: "IsAdmin",
                table: "UserChat",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Chats",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Chats",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdmin",
                table: "UserChat");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Chats");

            migrationBuilder.AddColumn<Guid>(
                name: "ProfileId",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
