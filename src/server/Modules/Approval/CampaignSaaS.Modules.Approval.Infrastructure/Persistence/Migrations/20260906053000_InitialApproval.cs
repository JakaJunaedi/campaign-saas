using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampaignSaaS.Modules.Approval.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialApproval : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "approval_reviews",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organization_id = table.Column<Guid>(type: "uuid", nullable: false),
                    content_submission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reviewer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    decision = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    feedback_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_approval_reviews", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "idx_reviews_submission",
                table: "approval_reviews",
                column: "content_submission_id");

            migrationBuilder.CreateIndex(
                name: "idx_reviews_org",
                table: "approval_reviews",
                column: "organization_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "approval_reviews");
        }
    }
}
