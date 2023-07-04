using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrim.Event.Analytics.Api.Migrations
{
    /// <inheritdoc />
    public partial class seed_gap_analysis_feature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "hrim_analytics",
                table: "hrim_features",
                columns: new[] { "id", "code", "concurrent_token", "created_at", "description", "feature_type", "is_deleted", "is_on", "updated_at", "variable_name" },
                values: new object[] { new Guid("2f1e83aa-a0f2-492f-af76-c6de43ad277b"), "gap", 1L, new DateTime(2023, 6, 21, 21, 2, 52, 864, DateTimeKind.Utc), "Calculates gaps between events of a specific event type", "Analysis", null, true, new DateTime(2023, 6, 21, 21, 2, 52, 864, DateTimeKind.Utc), "FEAT_GAP_ANALYSIS" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "hrim_analytics",
                table: "hrim_features",
                keyColumn: "id",
                keyValue: new Guid("2f1e83aa-a0f2-492f-af76-c6de43ad277b"));
        }
    }
}
