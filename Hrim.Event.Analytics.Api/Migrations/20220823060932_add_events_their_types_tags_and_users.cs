using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable
#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Api.Migrations
{
    public partial class add_events_their_types_tags_and_users : Migration
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
                name: "event_types",
                schema: "hrim_analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    name = table.Column<string>(type: "text", nullable: false, comment: "Event type name, e.g. 'nice mood', 'headache', etc"),
                    description = table.Column<string>(type: "text", nullable: true, comment: "Description given by user, when user_event_type based on this one will be created."),
                    color = table.Column<string>(type: "text", nullable: false, comment: "A color that events will be drawing with in a calendar. e.g. 'red', '#ff0000'"),
                    created_by = table.Column<Guid>(type: "uuid", nullable: false, comment: "A user who created an instance of this event type"),
                    is_public = table.Column<bool>(type: "boolean", nullable: false, comment: " An owner who created this event_type could share it with other end-users"),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time of entity instance creation"),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true, comment: "Date and UTC time of entity instance last update "),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    concurrent_token = table.Column<long>(type: "bigint", nullable: false, comment: "Update is possible only when this token equals to the token in the storage")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_types", x => x.id);
                    table.CheckConstraint("CK_user_event_types_concurrent_token", "concurrent_token > 0");
                    table.ForeignKey(
                        name: "FK_event_types_hrim_users_created_by",
                        column: x => x.created_by,
                        principalSchema: "hrim_analytics",
                        principalTable: "hrim_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "User defined event types.\nhttps://hrimsoft.atlassian.net/wiki/spaces/HRIMCALEND/pages/65566/System+Event+Types");

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
                name: "duration_events",
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
                    created_by = table.Column<Guid>(type: "uuid", nullable: false, comment: "A user who created an instance of this event type"),
                    event_type_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Event type on which current event is based.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_duration_events", x => x.id);
                    table.CheckConstraint("CK_db_duration_events_concurrent_token", "concurrent_token > 0");
                    table.ForeignKey(
                        name: "FK_duration_events_event_types_event_type_id",
                        column: x => x.event_type_id,
                        principalSchema: "hrim_analytics",
                        principalTable: "event_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_duration_events_hrim_users_created_by",
                        column: x => x.created_by,
                        principalSchema: "hrim_analytics",
                        principalTable: "hrim_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "When it is important to register an event that has start time and end time this system_event_type can be used.\nThis kind of events may occur several times a day and can cross each other.");

            migrationBuilder.CreateTable(
                name: "occurrence_events",
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
                    created_by = table.Column<Guid>(type: "uuid", nullable: false, comment: "A user who created an instance of this event type"),
                    event_type_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "Event type on which current event is based.")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_occurrence_events", x => x.id);
                    table.CheckConstraint("CK_db_occurrence_events_concurrent_token", "concurrent_token > 0");
                    table.ForeignKey(
                        name: "FK_occurrence_events_event_types_event_type_id",
                        column: x => x.event_type_id,
                        principalSchema: "hrim_analytics",
                        principalTable: "event_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_occurrence_events_hrim_users_created_by",
                        column: x => x.created_by,
                        principalSchema: "hrim_analytics",
                        principalTable: "hrim_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "When the main importance is the fact that an event occurred.\nThis kind of events may occur several times a day.");

            migrationBuilder.CreateIndex(
                name: "IX_duration_events_created_by_started_on",
                schema: "hrim_analytics",
                table: "duration_events",
                columns: new[] { "created_by", "started_on" })
                .Annotation("Npgsql:IndexInclude", new[] { "event_type_id", "started_at", "finished_on", "finished_at", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "IX_duration_events_event_type_id",
                schema: "hrim_analytics",
                table: "duration_events",
                column: "event_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_types_created_by_name",
                schema: "hrim_analytics",
                table: "event_types",
                columns: new[] { "created_by", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_hrim_tags_created_by",
                schema: "hrim_analytics",
                table: "hrim_tags",
                column: "created_by")
                .Annotation("Npgsql:IndexInclude", new[] { "tag" });

            migrationBuilder.CreateIndex(
                name: "IX_occurrence_events_created_by_occurred_at",
                schema: "hrim_analytics",
                table: "occurrence_events",
                columns: new[] { "created_by", "occurred_at" })
                .Annotation("Npgsql:IndexInclude", new[] { "event_type_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "IX_occurrence_events_event_type_id",
                schema: "hrim_analytics",
                table: "occurrence_events",
                column: "event_type_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "duration_events",
                schema: "hrim_analytics");

            migrationBuilder.DropTable(
                name: "hrim_tags",
                schema: "hrim_analytics");

            migrationBuilder.DropTable(
                name: "occurrence_events",
                schema: "hrim_analytics");

            migrationBuilder.DropTable(
                name: "event_types",
                schema: "hrim_analytics");

            migrationBuilder.DropTable(
                name: "hrim_users",
                schema: "hrim_analytics");
        }
    }
}
