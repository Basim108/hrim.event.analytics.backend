using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrim.Event.Analytics.Api.Migrations
{
    /// <inheritdoc />
    public partial class support_event_types_hierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "parent_id",
                schema: "hrim_analytics",
                table: "event_types",
                type: "uuid",
                nullable: true,
                comment: "Reference to a more general event type, which this type is specified in some context\nFor example, if current event type is Hatha Yoga, its parent type might be just general Yoga.");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                schema: "analysis",
                table: "analysis_by_event_type",
                type: "timestamptz",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                comment: "Date and UTC time of entity instance last update ",
                oldClrType: typeof(DateTime),
                oldType: "timestamptz",
                oldNullable: true,
                oldComment: "Date and UTC time of entity instance last update ");

            migrationBuilder.CreateIndex(
                name: "IX_event_types_parent_id",
                schema: "hrim_analytics",
                table: "event_types",
                column: "parent_id");

            migrationBuilder.AddForeignKey(
                name: "FK_event_types_event_types_parent_id",
                schema: "hrim_analytics",
                table: "event_types",
                column: "parent_id",
                principalSchema: "hrim_analytics",
                principalTable: "event_types",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_event_types_event_types_parent_id",
                schema: "hrim_analytics",
                table: "event_types");

            migrationBuilder.DropIndex(
                name: "IX_event_types_parent_id",
                schema: "hrim_analytics",
                table: "event_types");

            migrationBuilder.DropColumn(
                name: "parent_id",
                schema: "hrim_analytics",
                table: "event_types");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                schema: "analysis",
                table: "analysis_by_event_type",
                type: "timestamptz",
                nullable: true,
                comment: "Date and UTC time of entity instance last update ",
                oldClrType: typeof(DateTime),
                oldType: "timestamptz",
                oldComment: "Date and UTC time of entity instance last update ");
        }
    }
}
