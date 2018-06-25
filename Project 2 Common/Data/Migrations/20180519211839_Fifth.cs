using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Project_2_Common.Data.Migrations
{
    public partial class Fifth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_ReceiverId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "hasBeenRead",
                table: "Messages",
                newName: "Read");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateSent",
                table: "Messages",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_ReceiverID",
                table: "Messages",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "ForeignKey_SenderID",
                table: "Messages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "ForeignKey_ReceiverID",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "ForeignKey_SenderID",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "DateSent",
                table: "Messages");

            migrationBuilder.RenameColumn(
                name: "Read",
                table: "Messages",
                newName: "hasBeenRead");

            migrationBuilder.AddColumn<string>(
                name: "Date",
                table: "Messages",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_ReceiverId",
                table: "Messages",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_AspNetUsers_SenderId",
                table: "Messages",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
