using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrim.Event.Analytics.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_statistics_for_events_and_event_types : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "statistics_for_event_types",
                schema: "analysis",
                columns: table => new
                {
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "refers to an event type for which this calculation was made."),
                    analysis_code = table.Column<string>(type: "text", nullable: false, comment: "Code of analysis such as count, gap, etc"),
                    result = table.Column<string>(type: "text", nullable: true),
                    started_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time when an analysis has been started."),
                    finished_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time when an analysis has been finished."),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "The last run job id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statistics_for_event_types", x => new { x.entity_id, x.analysis_code });
                    table.ForeignKey(
                        name: "FK_statistics_for_event_types_event_types_entity_id",
                        column: x => x.entity_id,
                        principalSchema: "hrim_analytics",
                        principalTable: "event_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Stores results of calculation analysis for event types");

            migrationBuilder.CreateTable(
                name: "statistics_for_events",
                schema: "analysis",
                columns: table => new
                {
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "refers to an occurrence/duration event for which this calculation was made."),
                    analysis_code = table.Column<string>(type: "text", nullable: false, comment: "Code of analysis such as count, gap, etc"),
                    result = table.Column<string>(type: "text", nullable: true),
                    started_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time when an analysis has been started."),
                    finished_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time when an analysis has been finished."),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "The last run job id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statistics_for_events", x => new { x.entity_id, x.analysis_code });
                    table.ForeignKey(
                        name: "FK_statistics_for_events_duration_events_entity_id",
                        column: x => x.entity_id,
                        principalSchema: "hrim_analytics",
                        principalTable: "duration_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_statistics_for_events_occurrence_events_entity_id",
                        column: x => x.entity_id,
                        principalSchema: "hrim_analytics",
                        principalTable: "occurrence_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Stores results of calculation analysis for event types");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "statistics_for_event_types",
                schema: "analysis");

            migrationBuilder.DropTable(
                name: "statistics_for_events",
                schema: "analysis");
        }
    }
}
