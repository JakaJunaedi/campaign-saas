using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampaignSaaS.Modules.Campaign.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCampaign : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "campaign_creators",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    campaign_id = table.Column<Guid>(type: "uuid", nullable: false),
                    creator_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    agreed_rate = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaign_creators", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "campaigns",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    budget = table.Column<decimal>(type: "numeric(15,2)", precision: 15, scale: 2, nullable: false),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaigns", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_campaign_creators_campaign_id_creator_id",
                table: "campaign_creators",
                columns: new[] { "campaign_id", "creator_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_campaign_creators_organization_id",
                table: "campaign_creators",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "IX_campaigns_organization_id",
                table: "campaigns",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "IX_campaigns_organization_id_client_id",
                table: "campaigns",
                columns: new[] { "organization_id", "client_id" });

            migrationBuilder.CreateIndex(
                name: "IX_campaigns_organization_id_status",
                table: "campaigns",
                columns: new[] { "organization_id", "status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campaign_creators");

            migrationBuilder.DropTable(
                name: "campaigns");
        }
    }
}
