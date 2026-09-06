using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampaignSaaS.Modules.Reporting.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialReporting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "campaign_metrics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    deliverable_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reach = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    impressions = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    views = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    likes = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    comments = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    shares = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    clicks = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    total_engagement = table.Column<long>(type: "bigint", nullable: false, defaultValue: 0L),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaign_metrics", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "campaign_reports",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    campaign_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    file_object_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    requested_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaign_reports", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "uq_metrics_deliverable",
                table: "campaign_metrics",
                column: "deliverable_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_metrics_org",
                table: "campaign_metrics",
                column: "organization_id");

            migrationBuilder.CreateIndex(
                name: "idx_reports_campaign",
                table: "campaign_reports",
                column: "campaign_id");

            migrationBuilder.CreateIndex(
                name: "idx_reports_org",
                table: "campaign_reports",
                column: "organization_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campaign_metrics");

            migrationBuilder.DropTable(
                name: "campaign_reports");
        }
    }
}
