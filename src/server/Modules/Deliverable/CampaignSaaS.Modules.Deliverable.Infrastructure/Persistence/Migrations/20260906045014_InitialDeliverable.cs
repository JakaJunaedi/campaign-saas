using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampaignSaaS.Modules.Deliverable.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialDeliverable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "content_submissions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    deliverable_id = table.Column<Guid>(type: "uuid", nullable: false),
                    version_number = table.Column<int>(type: "integer", nullable: false),
                    media_object_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    media_file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    media_file_size = table.Column<long>(type: "bigint", nullable: false),
                    caption = table.Column<string>(type: "text", nullable: true),
                    submitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_content_submissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "deliverables",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    campaign_id = table.Column<Guid>(type: "uuid", nullable: false),
                    campaign_creator_id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(250)", maxLength: 250, nullable: false),
                    platform = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    content_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    brief_notes = table.Column<string>(type: "text", nullable: true),
                    due_date = table.Column<DateOnly>(type: "date", nullable: false),
                    posting_date = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    live_url = table.Column<string>(type: "text", nullable: true),
                    proof_media_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_deliverables", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_submissions_deliverable",
                table: "content_submissions",
                column: "deliverable_id");

            migrationBuilder.CreateIndex(
                name: "uq_submission_version",
                table: "content_submissions",
                columns: new[] { "deliverable_id", "version_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_deliverables_campaign",
                table: "deliverables",
                column: "campaign_id");

            migrationBuilder.CreateIndex(
                name: "idx_deliverables_org_status",
                table: "deliverables",
                columns: new[] { "organization_id", "status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "content_submissions");

            migrationBuilder.DropTable(
                name: "deliverables");
        }
    }
}
