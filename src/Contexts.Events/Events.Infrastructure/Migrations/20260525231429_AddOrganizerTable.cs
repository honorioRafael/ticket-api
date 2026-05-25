using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Events.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganizerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "organizers",
                schema: "events",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_organizers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_organizers_Email",
                schema: "events",
                table: "organizers",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "organizers",
                schema: "events");
        }
    }
}
