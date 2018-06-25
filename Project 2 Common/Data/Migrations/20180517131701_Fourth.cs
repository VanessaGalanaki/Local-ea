using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Project_2_Common.Data.Migrations
{
    public partial class Fourth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiverName",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SenderName",
                table: "Messages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiverName",
                table: "Messages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderName",
                table: "Messages",
                nullable: true);
        }
    }
}
