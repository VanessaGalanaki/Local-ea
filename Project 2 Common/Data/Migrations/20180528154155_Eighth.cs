using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Project_2_Common.Data.Migrations
{
    public partial class Eighth : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FromId = table.Column<string>(nullable: true),
                    RatingText = table.Column<string>(nullable: true),
                    RatingValue = table.Column<decimal>(nullable: false),
                    ToId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.ForeignKey(
                        name: "ForeignKey_ReviewerID",
                        column: x => x.FromId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "ForeignKey_AwardedID",
                        column: x => x.ToId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_FromId",
                table: "Ratings",
                column: "FromId");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_ToId",
                table: "Ratings",
                column: "ToId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ratings");
        }
    }
}
