using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrim.Event.Analytics.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_analysis_by_event_type_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "analysis");

            migrationBuilder.CreateTable(
                name: "analysis_by_event_type",
                schema: "analysis",
                columns: table => new
                {
                    event_type_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Events of this event type will be analysed"),
                    analysis_code = table.Column<string>(type: "text", nullable: false),
                    is_on = table.Column<bool>(type: "boolean", nullable: false, comment: "Enable/disable analysis for a particular event-type"),
                    settings = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time of entity instance creation"),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true, comment: "Date and UTC time of entity instance last update "),
                    concurrent_token = table.Column<long>(type: "bigint", nullable: false, comment: "Update is possible only when this token equals to the token in the storage")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_analysis_by_event_type", x => new { x.event_type_id, x.analysis_code });
                    table.CheckConstraint("CK_analysis_by_event_types_concurrent_token", "concurrent_token > 0");
                    table.ForeignKey(
                        name: "FK_analysis_by_event_type_event_types_event_type_id",
                        column: x => x.event_type_id,
                        principalSchema: "hrim_analytics",
                        principalTable: "event_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Analysis that is made around events of a particular event-type");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "analysis_by_event_type",
                schema: "analysis");
        }
    }
}
