using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrim.Event.Analytics.Api.Migrations
{
    /// <inheritdoc />
    public partial class seed_count_analysis_feature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "hrim_analytics",
                table: "hrim_features",
                columns: new[] { "id", "code", "concurrent_token", "created_at", "description", "feature_type", "is_deleted", "is_on", "updated_at", "variable_name" },
                values: new object[] { new Guid("023f9105-6f6d-4ef7-b53f-c3bfa8a1a2e2"), "count", 1L, new DateTime(2023, 7, 8, 20, 18, 52, 864, DateTimeKind.Utc), "Calculates number of events and provides some calculation on duration lengths", "Analysis", null, true, new DateTime(2023, 7, 8, 20, 18, 52, 864, DateTimeKind.Utc), "FEAT_COUNT_ANALYSIS" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "hrim_analytics",
                table: "hrim_features",
                keyColumn: "id",
                keyValue: new Guid("023f9105-6f6d-4ef7-b53f-c3bfa8a1a2e2"));
        }
    }
}
