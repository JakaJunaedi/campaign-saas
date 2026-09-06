using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampaignSaaS.Modules.Creator.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "creators",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    phone_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    niche = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    social_accounts_json = table.Column<string>(type: "jsonb", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_creators", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_creators_organization_id",
                table: "creators",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "IX_creators_organization_id_niche",
                table: "creators",
                columns: new[] { "organization_id", "niche" });

            migrationBuilder.CreateIndex(
                name: "IX_creators_organization_id_status",
                table: "creators",
                columns: new[] { "organization_id", "status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "creators");
        }
    }
}
