using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Hrim.Event.Analytics.Api.Migrations
{
    public partial class add_event_types_and_tags_and_users : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "hrim_analytics");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "hrim_users",
                schema: "hrim_analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    email = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time of entity instance creation"),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true, comment: "Date and UTC time of entity instance last update "),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    concurrent_token = table.Column<long>(type: "bigint", nullable: false, comment: "Update is possible only when this token equals to the token in the storage")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hrim_users", x => x.id);
                    table.CheckConstraint("CK_hrim_users_concurrent_token", "concurrent_token > 0");
                },
                comment: "An authorized user");

            migrationBuilder.CreateTable(
                name: "duration_event_types",
                schema: "hrim_analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    started_on = table.Column<DateOnly>(type: "date", nullable: false, comment: "Date when an event started"),
                    started_at = table.Column<DateTimeOffset>(type: "timetz", nullable: false, comment: "Time with end-user timezone when an event started"),
                    finished_on = table.Column<DateOnly>(type: "date", nullable: true, comment: "Date when an event finished"),
                    finished_at = table.Column<DateTimeOffset>(type: "timetz", nullable: true, comment: "Time with end-user timezone when an event finished"),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time of entity instance creation"),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true, comment: "Date and UTC time of entity instance last update "),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    concurrent_token = table.Column<long>(type: "bigint", nullable: false, comment: "Update is possible only when this token equals to the token in the storage"),
                    name = table.Column<string>(type: "text", nullable: false, comment: "Event type name, e.g. 'nice mood', 'headache', etc"),
                    description = table.Column<string>(type: "text", nullable: true, comment: "Description given by user, when user_event_type based on this one will be created."),
                    color = table.Column<string>(type: "text", nullable: false, comment: "A color that events will be drawing with in a calendar. e.g. 'red', '#ff0000'"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false, comment: "A user who created an instance of this event type"),
                    is_public = table.Column<bool>(type: "boolean", nullable: false, comment: "A color that events will be drawing with in a calendar. e.g. 'red', '#ff0000'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_duration_event_types", x => x.id);
                    table.CheckConstraint("CK_db_duration_event_types_concurrent_token", "concurrent_token > 0");
                    table.ForeignKey(
                        name: "FK_duration_event_types_hrim_users_created_by",
                        column: x => x.created_by,
                        principalSchema: "hrim_analytics",
                        principalTable: "hrim_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "When it is important to register an event that has start time and end time this system_event_type can be used.\nThis kind of events may occur several times a day.");

            migrationBuilder.CreateTable(
                name: "hrim_tags",
                schema: "hrim_analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    tag = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false, comment: "A user id who created a tag"),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time of entity instance creation"),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true, comment: "Date and UTC time of entity instance last update "),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    concurrent_token = table.Column<long>(type: "bigint", nullable: false, comment: "Update is possible only when this token equals to the token in the storage")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hrim_tags", x => x.id);
                    table.CheckConstraint("CK_hrim_tags_concurrent_token", "concurrent_token > 0");
                    table.ForeignKey(
                        name: "FK_hrim_tags_hrim_users_created_by",
                        column: x => x.created_by,
                        principalSchema: "hrim_analytics",
                        principalTable: "hrim_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "A tag that could be linked to an instance of any entity");

            migrationBuilder.CreateTable(
                name: "occurrence_event_types",
                schema: "hrim_analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    occurred_on = table.Column<DateOnly>(type: "date", nullable: false, comment: "Date when an event occurred"),
                    occurred_at = table.Column<DateTimeOffset>(type: "timetz", nullable: false, comment: "Time with end-user timezone when an event occurred"),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time of entity instance creation"),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true, comment: "Date and UTC time of entity instance last update "),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    concurrent_token = table.Column<long>(type: "bigint", nullable: false, comment: "Update is possible only when this token equals to the token in the storage"),
                    name = table.Column<string>(type: "text", nullable: false, comment: "Event type name, e.g. 'nice mood', 'headache', etc"),
                    description = table.Column<string>(type: "text", nullable: true, comment: "Description given by user, when user_event_type based on this one will be created."),
                    color = table.Column<string>(type: "text", nullable: false, comment: "A color that events will be drawing with in a calendar. e.g. 'red', '#ff0000'"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false, comment: "A user who created an instance of this event type"),
                    is_public = table.Column<bool>(type: "boolean", nullable: false, comment: "A color that events will be drawing with in a calendar. e.g. 'red', '#ff0000'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_occurrence_event_types", x => x.id);
                    table.CheckConstraint("CK_db_occurrence_event_types_concurrent_token", "concurrent_token > 0");
                    table.ForeignKey(
                        name: "FK_occurrence_event_types_hrim_users_created_by",
                        column: x => x.created_by,
                        principalSchema: "hrim_analytics",
                        principalTable: "hrim_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "When the main importance is the fact that an event occurred.\nThis kind of events may occur several times a day.");

            migrationBuilder.CreateIndex(
                name: "IX_duration_event_types_created_by_name",
                schema: "hrim_analytics",
                table: "duration_event_types",
                columns: new[] { "created_by", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_duration_event_types_created_by_started_on",
                schema: "hrim_analytics",
                table: "duration_event_types",
                columns: new[] { "created_by", "started_on" })
                .Annotation("Npgsql:IndexInclude", new[] { "started_at", "finished_on", "finished_at", "color", "name" });

            migrationBuilder.CreateIndex(
                name: "IX_hrim_tags_created_by",
                schema: "hrim_analytics",
                table: "hrim_tags",
                column: "created_by")
                .Annotation("Npgsql:IndexInclude", new[] { "tag" });

            migrationBuilder.CreateIndex(
                name: "IX_occurrence_event_types_created_by_name",
                schema: "hrim_analytics",
                table: "occurrence_event_types",
                columns: new[] { "created_by", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_occurrence_event_types_created_by_occurred_at",
                schema: "hrim_analytics",
                table: "occurrence_event_types",
                columns: new[] { "created_by", "occurred_at" })
                .Annotation("Npgsql:IndexInclude", new[] { "color", "name" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "duration_event_types",
                schema: "hrim_analytics");

            migrationBuilder.DropTable(
                name: "hrim_tags",
                schema: "hrim_analytics");

            migrationBuilder.DropTable(
                name: "occurrence_event_types",
                schema: "hrim_analytics");

            migrationBuilder.DropTable(
                name: "hrim_users",
                schema: "hrim_analytics");
        }
    }
}
