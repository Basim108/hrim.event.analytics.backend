using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Hrim.Event.Analytics.Api.Migrations
{
    /// <inheritdoc />
    public partial class v2_initial_changed_pk_types_and_add_ltree_event_types : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "v2_analysis");

            migrationBuilder.EnsureSchema(
                name: "v2_hrim_analytics");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:ltree", ",,")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,");

            migrationBuilder.CreateTable(
                name: "hrim_features",
                schema: "v2_hrim_analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "uuid_generate_v4()"),
                    is_on = table.Column<bool>(type: "boolean", nullable: false, comment: "When a feature is off then its hangfire jobs, in case existed, should not be proceeded or scheduled.\nand in case feature represents an analysis (e.g. count, gap) this analysis should not appear in the list of available analysis."),
                    feature_type = table.Column<string>(type: "text", nullable: false, comment: "Could be one of:\nAnalysis"),
                    description = table.Column<string>(type: "text", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "hrim_users",
                schema: "v2_hrim_analytics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
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
                schema: "v2_hrim_analytics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    tree_node_path = table.Column<string>(type: "ltree", nullable: false),
                    parent_id = table.Column<long>(type: "bigint", nullable: true, comment: "Reference to a more general event type, which this type is specified in some context\nFor example, if current event type is Hatha Yoga, its parent type might be just general Yoga."),
                    name = table.Column<string>(type: "text", nullable: false, comment: "Event type name, e.g. 'nice mood', 'headache', etc"),
                    description = table.Column<string>(type: "text", nullable: true, comment: "Description given by user, when user_event_type based on this one will be created."),
                    color = table.Column<string>(type: "text", nullable: false, comment: "A color that events will be drawing with in a calendar. e.g. 'red', '#ff0000'"),
                    is_public = table.Column<bool>(type: "boolean", nullable: false, comment: " An owner who created this event_type could share it with other end-users"),
                    created_by = table.Column<long>(type: "bigint", nullable: false, comment: "A user who created an instance of this event type"),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time of entity instance creation"),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true, comment: "Date and UTC time of entity instance last update "),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    concurrent_token = table.Column<long>(type: "bigint", nullable: false, comment: "Update is possible only when this token equals to the token in the storage")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_event_types", x => x.id);
                    table.CheckConstraint("CK_db_event_types_concurrent_token", "concurrent_token > 0");
                    table.ForeignKey(
                        name: "FK_event_types_event_types_parent_id",
                        column: x => x.parent_id,
                        principalSchema: "v2_hrim_analytics",
                        principalTable: "event_types",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_event_types_hrim_users_created_by",
                        column: x => x.created_by,
                        principalSchema: "v2_hrim_analytics",
                        principalTable: "hrim_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "User defined event types.\nhttps://hrimsoft.atlassian.net/wiki/spaces/HRIMCALEND/pages/65566/System+Event+Types");

            migrationBuilder.CreateTable(
                name: "external_user_profiles",
                schema: "v2_hrim_analytics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false, comment: "A user id in current system to which this profile is linked to"),
                    external_user_id = table.Column<string>(type: "text", nullable: false, comment: "A user id in external identity provider"),
                    idp = table.Column<string>(type: "text", nullable: false, comment: "Identity provider that provided this profile"),
                    email = table.Column<string>(type: "text", nullable: true),
                    last_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, comment: "If null then profile was linked but never used as a login"),
                    full_name = table.Column<string>(type: "text", nullable: true),
                    first_name = table.Column<string>(type: "text", nullable: true),
                    last_name = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time of entity instance creation"),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true, comment: "Date and UTC time of entity instance last update "),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    concurrent_token = table.Column<long>(type: "bigint", nullable: false, comment: "Update is possible only when this token equals to the token in the storage")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_external_user_profiles", x => x.id);
                    table.CheckConstraint("CK_external_user_profiles_concurrent_token", "concurrent_token > 0");
                    table.ForeignKey(
                        name: "FK_external_user_profiles_hrim_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "v2_hrim_analytics",
                        principalTable: "hrim_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "user profiles from a specific idp such as Google, Facebook, etc");

            migrationBuilder.CreateTable(
                name: "hrim_tags",
                schema: "v2_hrim_analytics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    tag = table.Column<string>(type: "text", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: false, comment: "A user id who created a tag"),
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
                        principalSchema: "v2_hrim_analytics",
                        principalTable: "hrim_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "A tag that could be linked to an instance of any entity");

            migrationBuilder.CreateTable(
                name: "analysis_config_by_event_type",
                schema: "v2_analysis",
                columns: table => new
                {
                    event_type_id = table.Column<long>(type: "bigint", nullable: false, comment: "Events of this event type will be analysed"),
                    analysis_code = table.Column<string>(type: "text", nullable: false),
                    is_on = table.Column<bool>(type: "boolean", nullable: false, comment: "Enable/disable analysis for a particular event-type"),
                    settings = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time of entity instance creation"),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time of entity instance last update "),
                    concurrent_token = table.Column<long>(type: "bigint", nullable: false, comment: "Update is possible only when this token equals to the token in the storage")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_analysis_config_by_event_type", x => new { x.event_type_id, x.analysis_code });
                    table.CheckConstraint("CK_analysis_config_by_event_types_concurrent_token", "concurrent_token > 0");
                    table.ForeignKey(
                        name: "FK_analysis_config_by_event_type_event_types_event_type_id",
                        column: x => x.event_type_id,
                        principalSchema: "v2_hrim_analytics",
                        principalTable: "event_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Configuration of an analysis that will be made around events of a particular event-type");

            migrationBuilder.CreateTable(
                name: "duration_events",
                schema: "v2_hrim_analytics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    started_on = table.Column<DateOnly>(type: "date", nullable: false, comment: "Date when an event started"),
                    started_at = table.Column<DateTimeOffset>(type: "timetz", nullable: false, comment: "Time with end-user timezone when an event started"),
                    finished_on = table.Column<DateOnly>(type: "date", nullable: true, comment: "Date when an event finished"),
                    finished_at = table.Column<DateTimeOffset>(type: "timetz", nullable: true, comment: "Time with end-user timezone when an event finished"),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time of entity instance creation"),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true, comment: "Date and UTC time of entity instance last update "),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    concurrent_token = table.Column<long>(type: "bigint", nullable: false, comment: "Update is possible only when this token equals to the token in the storage"),
                    event_type_id = table.Column<long>(type: "bigint", nullable: false, comment: "Event type on which current event is based."),
                    created_by = table.Column<long>(type: "bigint", nullable: false, comment: "A user who created an instance of this event type"),
                    props = table.Column<string>(type: "jsonb", nullable: true, comment: "Some additional values associated with this event")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_duration_events", x => x.id);
                    table.CheckConstraint("CK_db_duration_events_concurrent_token", "concurrent_token > 0");
                    table.ForeignKey(
                        name: "FK_duration_events_event_types_event_type_id",
                        column: x => x.event_type_id,
                        principalSchema: "v2_hrim_analytics",
                        principalTable: "event_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_duration_events_hrim_users_created_by",
                        column: x => x.created_by,
                        principalSchema: "v2_hrim_analytics",
                        principalTable: "hrim_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "When it is important to register an event that has start time and end time this system_event_type can be used.\nThis kind of events may occur several times a day and can cross each other.");

            migrationBuilder.CreateTable(
                name: "occurrence_events",
                schema: "v2_hrim_analytics",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    occurred_on = table.Column<DateOnly>(type: "date", nullable: false, comment: "Date when an event occurred"),
                    occurred_at = table.Column<DateTimeOffset>(type: "timetz", nullable: false, comment: "Time with end-user timezone when an event occurred"),
                    created_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time of entity instance creation"),
                    updated_at = table.Column<DateTime>(type: "timestamptz", nullable: true, comment: "Date and UTC time of entity instance last update "),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: true),
                    concurrent_token = table.Column<long>(type: "bigint", nullable: false, comment: "Update is possible only when this token equals to the token in the storage"),
                    event_type_id = table.Column<long>(type: "bigint", nullable: false, comment: "Event type on which current event is based."),
                    created_by = table.Column<long>(type: "bigint", nullable: false, comment: "A user who created an instance of this event type"),
                    props = table.Column<string>(type: "jsonb", nullable: true, comment: "Some additional values associated with this event")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_occurrence_events", x => x.id);
                    table.CheckConstraint("CK_db_occurrence_events_concurrent_token", "concurrent_token > 0");
                    table.ForeignKey(
                        name: "FK_occurrence_events_event_types_event_type_id",
                        column: x => x.event_type_id,
                        principalSchema: "v2_hrim_analytics",
                        principalTable: "event_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_occurrence_events_hrim_users_created_by",
                        column: x => x.created_by,
                        principalSchema: "v2_hrim_analytics",
                        principalTable: "hrim_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "When the main importance is the fact that an event occurred.\nThis kind of events may occur several times a day.");

            migrationBuilder.CreateTable(
                name: "statistics_for_event_types",
                schema: "v2_analysis",
                columns: table => new
                {
                    entity_id = table.Column<long>(type: "bigint", nullable: false, comment: "refers to an event type for which this calculation was made."),
                    analysis_code = table.Column<string>(type: "text", nullable: false, comment: "Code of analysis such as count, gap, etc"),
                    result = table.Column<string>(type: "text", nullable: true),
                    started_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time when an analysis has been started."),
                    finished_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time when an analysis has been finished."),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "The last run correlation id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statistics_for_event_types", x => new { x.entity_id, x.analysis_code });
                    table.ForeignKey(
                        name: "FK_statistics_for_event_types_event_types_entity_id",
                        column: x => x.entity_id,
                        principalSchema: "v2_hrim_analytics",
                        principalTable: "event_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Stores results of calculation analysis for event types");

            migrationBuilder.CreateTable(
                name: "statistics_for_events",
                schema: "v2_analysis",
                columns: table => new
                {
                    entity_id = table.Column<long>(type: "bigint", nullable: false, comment: "refers to an occurrence/duration event for which this calculation was made."),
                    analysis_code = table.Column<string>(type: "text", nullable: false, comment: "Code of analysis such as count, gap, etc"),
                    result = table.Column<string>(type: "text", nullable: true),
                    started_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time when an analysis has been started."),
                    finished_at = table.Column<DateTime>(type: "timestamptz", nullable: false, comment: "Date and UTC time when an analysis has been finished."),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false, comment: "The last run correlation id")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_statistics_for_events", x => new { x.entity_id, x.analysis_code });
                    table.ForeignKey(
                        name: "FK_statistics_for_events_duration_events_entity_id",
                        column: x => x.entity_id,
                        principalSchema: "v2_hrim_analytics",
                        principalTable: "duration_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_statistics_for_events_occurrence_events_entity_id",
                        column: x => x.entity_id,
                        principalSchema: "v2_hrim_analytics",
                        principalTable: "occurrence_events",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                },
                comment: "Stores results of calculation analysis for event types");

            migrationBuilder.InsertData(
                schema: "v2_hrim_analytics",
                table: "hrim_features",
                columns: new[] { "id", "code", "concurrent_token", "created_at", "description", "feature_type", "is_deleted", "is_on", "updated_at", "variable_name" },
                values: new object[,]
                {
                    { new Guid("023f9105-6f6d-4ef7-b53f-c3bfa8a1a2e2"), "count", 1L, new DateTime(2023, 7, 8, 20, 18, 52, 864, DateTimeKind.Utc), "Calculates number of events and provides some calculation on duration lengths", "Analysis", null, true, new DateTime(2023, 7, 8, 20, 18, 52, 864, DateTimeKind.Utc), "FEAT_COUNT_ANALYSIS" },
                    { new Guid("2f1e83aa-a0f2-492f-af76-c6de43ad277b"), "gap", 1L, new DateTime(2023, 6, 21, 21, 2, 52, 864, DateTimeKind.Utc), "Calculates gaps between events of a specific event type", "Analysis", null, true, new DateTime(2023, 6, 21, 21, 2, 52, 864, DateTimeKind.Utc), "FEAT_GAP_ANALYSIS" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_duration_events_created_by_started_on",
                schema: "v2_hrim_analytics",
                table: "duration_events",
                columns: new[] { "created_by", "started_on" })
                .Annotation("Npgsql:IndexInclude", new[] { "event_type_id", "started_at", "finished_on", "finished_at", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "IX_duration_events_event_type_id",
                schema: "v2_hrim_analytics",
                table: "duration_events",
                column: "event_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_event_types_created_by_name",
                schema: "v2_hrim_analytics",
                table: "event_types",
                columns: new[] { "created_by", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_event_types_parent_id",
                schema: "v2_hrim_analytics",
                table: "event_types",
                column: "parent_id");

            migrationBuilder.CreateIndex(
                name: "IX_external_user_profiles_user_id",
                schema: "v2_hrim_analytics",
                table: "external_user_profiles",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_hrim_tags_created_by",
                schema: "v2_hrim_analytics",
                table: "hrim_tags",
                column: "created_by")
                .Annotation("Npgsql:IndexInclude", new[] { "tag" });

            migrationBuilder.CreateIndex(
                name: "IX_occurrence_events_created_by_occurred_at",
                schema: "v2_hrim_analytics",
                table: "occurrence_events",
                columns: new[] { "created_by", "occurred_at" })
                .Annotation("Npgsql:IndexInclude", new[] { "event_type_id", "is_deleted" });

            migrationBuilder.CreateIndex(
                name: "IX_occurrence_events_event_type_id",
                schema: "v2_hrim_analytics",
                table: "occurrence_events",
                column: "event_type_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "analysis_config_by_event_type",
                schema: "v2_analysis");

            migrationBuilder.DropTable(
                name: "external_user_profiles",
                schema: "v2_hrim_analytics");

            migrationBuilder.DropTable(
                name: "hrim_features",
                schema: "v2_hrim_analytics");

            migrationBuilder.DropTable(
                name: "hrim_tags",
                schema: "v2_hrim_analytics");

            migrationBuilder.DropTable(
                name: "statistics_for_event_types",
                schema: "v2_analysis");

            migrationBuilder.DropTable(
                name: "statistics_for_events",
                schema: "v2_analysis");

            migrationBuilder.DropTable(
                name: "duration_events",
                schema: "v2_hrim_analytics");

            migrationBuilder.DropTable(
                name: "occurrence_events",
                schema: "v2_hrim_analytics");

            migrationBuilder.DropTable(
                name: "event_types",
                schema: "v2_hrim_analytics");

            migrationBuilder.DropTable(
                name: "hrim_users",
                schema: "v2_hrim_analytics");
        }
    }
}
