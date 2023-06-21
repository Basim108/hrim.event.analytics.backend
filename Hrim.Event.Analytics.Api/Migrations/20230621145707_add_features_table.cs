using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrim.Event.Analytics.Api.Migrations
{
    /// <inheritdoc />
    public partial class add_features_table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "hrim_features",
                schema: "hrim_analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    is_on = table.Column<bool>(type: "boolean", nullable: false, comment: "When a feature is off then its hangfire jobs, in case existed, should not be proceeded or scheduled.\nand in case feature represents an analysis (e.g. count, gap) this analysis should not appear in the list of available analysis."),
                    feature_type = table.Column<int>(type: "integer", nullable: false, comment: "Could be one of:\nAnalysis"),
                    variable_name = table.Column<string>(type: "text", nullable: false, comment: "Environment variable name that controls is this feature set on/off"),
                    code = table.Column<string>(type: "text", nullable: false, comment: "Feature code"),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time of entity instance creation"),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true, comment: "Date and UTC time of entity instance last update "),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    concurrent_token = table.Column<long>(type: "bigint", nullable: false, comment: "Update is possible only when this token equals to the token in the storage")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hrim_features", x => x.id);
                    table.CheckConstraint("CK_hrim_features_concurrent_token", "concurrent_token > 0");
                },
                comment: "Features that might be on/off;\nfor example, analysis based on event-types, tags, events, etc");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hrim_features",
                schema: "hrim_analytics");
        }
    }
}
