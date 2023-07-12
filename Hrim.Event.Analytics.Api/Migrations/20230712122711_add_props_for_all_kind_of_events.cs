using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrim.Event.Analytics.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_props_for_all_kind_of_events : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "props",
                schema: "hrim_analytics",
                table: "occurrence_events",
                type: "jsonb",
                nullable: true,
                comment: "Some additional values associated with this event");

            migrationBuilder.AddColumn<string>(
                name: "props",
                schema: "hrim_analytics",
                table: "duration_events",
                type: "jsonb",
                nullable: true,
                comment: "Some additional values associated with this event");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "props",
                schema: "hrim_analytics",
                table: "occurrence_events");

            migrationBuilder.DropColumn(
                name: "props",
                schema: "hrim_analytics",
                table: "duration_events");
        }
    }
}
