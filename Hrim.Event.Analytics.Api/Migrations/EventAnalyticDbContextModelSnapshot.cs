﻿// <auto-generated />
using System;
using Hrim.Event.Analytics.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Hrim.Event.Analytics.Api.Migrations
{
    [DbContext(typeof(EventAnalyticDbContext))]
    partial class EventAnalyticDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("hrim_analytics")
                .HasAnnotation("ProductVersion", "7.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.Account.ExternalUserProfile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<long>("ConcurrentToken")
                        .IsConcurrencyToken()
                        .HasColumnType("bigint")
                        .HasColumnName("concurrent_token")
                        .HasComment("Update is possible only when this token equals to the token in the storage");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("created_at")
                        .HasComment("Date and UTC time of entity instance creation");

                    b.Property<string>("Email")
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("ExternalUserId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("external_user_id")
                        .HasComment("A user id in external identity provider");

                    b.Property<string>("FirstName")
                        .HasColumnType("text")
                        .HasColumnName("first_name");

                    b.Property<string>("FullName")
                        .HasColumnType("text")
                        .HasColumnName("full_name");

                    b.Property<Guid>("HrimUserId")
                        .HasColumnType("uuid")
                        .HasColumnName("user_id")
                        .HasComment("A user id in current system to which this profile is linked to");

                    b.Property<string>("Idp")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("idp")
                        .HasComment("Identity provider that provided this profile");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime>("LastLogin")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("last_login")
                        .HasComment("If null then profile was linked but never used as a login");

                    b.Property<string>("LastName")
                        .HasColumnType("text")
                        .HasColumnName("last_name");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("updated_at")
                        .HasComment("Date and UTC time of entity instance last update ");

                    b.HasKey("Id");

                    b.HasIndex("HrimUserId");

                    b.ToTable("external_user_profiles", "hrim_analytics", t =>
                        {
                            t.HasComment("user profiles from a specific idp such as Google, Facebook, etc");

                            t.HasCheckConstraint("CK_external_user_profiles_concurrent_token", "concurrent_token > 0");
                        });
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.Account.HrimUser", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<long>("ConcurrentToken")
                        .IsConcurrencyToken()
                        .HasColumnType("bigint")
                        .HasColumnName("concurrent_token")
                        .HasComment("Update is possible only when this token equals to the token in the storage");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("created_at")
                        .HasComment("Date and UTC time of entity instance creation");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("updated_at")
                        .HasComment("Date and UTC time of entity instance last update ");

                    b.HasKey("Id");

                    b.ToTable("hrim_users", "hrim_analytics", t =>
                        {
                            t.HasComment("An authorized user");

                            t.HasCheckConstraint("CK_hrim_users_concurrent_token", "concurrent_token > 0");
                        });
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.Analysis.AnalysisByEventType", b =>
                {
                    b.Property<Guid>("EventTypeId")
                        .HasColumnType("uuid")
                        .HasColumnName("event_type_id")
                        .HasComment("Events of this event type will be analysed");

                    b.Property<string>("AnalysisCode")
                        .HasColumnType("text")
                        .HasColumnName("analysis_code");

                    b.Property<long>("ConcurrentToken")
                        .IsConcurrencyToken()
                        .HasColumnType("bigint")
                        .HasColumnName("concurrent_token")
                        .HasComment("Update is possible only when this token equals to the token in the storage");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("created_at")
                        .HasComment("Date and UTC time of entity instance creation");

                    b.Property<bool>("IsOn")
                        .HasColumnType("boolean")
                        .HasColumnName("is_on")
                        .HasComment("Enable/disable analysis for a particular event-type");

                    b.Property<string>("Settings")
                        .HasColumnType("jsonb")
                        .HasColumnName("settings");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("updated_at")
                        .HasComment("Date and UTC time of entity instance last update ");

                    b.HasKey("EventTypeId", "AnalysisCode");

                    b.ToTable("analysis_by_event_type", "analysis", t =>
                        {
                            t.HasComment("Analysis that is made around events of a particular event-type");

                            t.HasCheckConstraint("CK_analysis_by_event_types_concurrent_token", "concurrent_token > 0");
                        });
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.Analysis.StatisticsForEvent", b =>
                {
                    b.Property<Guid>("EntityId")
                        .HasColumnType("uuid")
                        .HasColumnName("entity_id")
                        .HasComment("refers to an occurrence/duration event for which this calculation was made.");

                    b.Property<string>("AnalysisCode")
                        .HasColumnType("text")
                        .HasColumnName("analysis_code")
                        .HasComment("Code of analysis such as count, gap, etc");

                    b.Property<Guid>("CorrelationId")
                        .HasColumnType("uuid")
                        .HasColumnName("correlation_id")
                        .HasComment("The last run correlation id");

                    b.Property<DateTime>("FinishedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("finished_at")
                        .HasComment("Date and UTC time when an analysis has been finished.");

                    b.Property<string>("ResultJson")
                        .HasColumnType("text")
                        .HasColumnName("result");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("started_at")
                        .HasComment("Date and UTC time when an analysis has been started.");

                    b.HasKey("EntityId", "AnalysisCode");

                    b.ToTable("statistics_for_events", "analysis", t =>
                        {
                            t.HasComment("Stores results of calculation analysis for event types");
                        });
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.Analysis.StatisticsForEventType", b =>
                {
                    b.Property<Guid>("EntityId")
                        .HasColumnType("uuid")
                        .HasColumnName("entity_id")
                        .HasComment("refers to an event type for which this calculation was made.");

                    b.Property<string>("AnalysisCode")
                        .HasColumnType("text")
                        .HasColumnName("analysis_code")
                        .HasComment("Code of analysis such as count, gap, etc");

                    b.Property<Guid>("CorrelationId")
                        .HasColumnType("uuid")
                        .HasColumnName("correlation_id")
                        .HasComment("The last run correlation id");

                    b.Property<DateTime>("FinishedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("finished_at")
                        .HasComment("Date and UTC time when an analysis has been finished.");

                    b.Property<string>("ResultJson")
                        .HasColumnType("text")
                        .HasColumnName("result");

                    b.Property<DateTime>("StartedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("started_at")
                        .HasComment("Date and UTC time when an analysis has been started.");

                    b.HasKey("EntityId", "AnalysisCode");

                    b.ToTable("statistics_for_event_types", "analysis", t =>
                        {
                            t.HasComment("Stores results of calculation analysis for event types");
                        });
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.EventTypes.UserEventType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("Color")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("color")
                        .HasComment("A color that events will be drawing with in a calendar. e.g. 'red', '#ff0000'");

                    b.Property<long>("ConcurrentToken")
                        .IsConcurrencyToken()
                        .HasColumnType("bigint")
                        .HasColumnName("concurrent_token")
                        .HasComment("Update is possible only when this token equals to the token in the storage");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("created_at")
                        .HasComment("Date and UTC time of entity instance creation");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid")
                        .HasColumnName("created_by")
                        .HasComment("A user who created an instance of this event type");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description")
                        .HasComment("Description given by user, when user_event_type based on this one will be created.");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("boolean")
                        .HasColumnName("is_public")
                        .HasComment(" An owner who created this event_type could share it with other end-users");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("name")
                        .HasComment("Event type name, e.g. 'nice mood', 'headache', etc");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("updated_at")
                        .HasComment("Date and UTC time of entity instance last update ");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById", "Name")
                        .IsUnique();

                    b.ToTable("event_types", "hrim_analytics", t =>
                        {
                            t.HasComment("User defined event types.\nhttps://hrimsoft.atlassian.net/wiki/spaces/HRIMCALEND/pages/65566/System+Event+Types");

                            t.HasCheckConstraint("CK_user_event_types_concurrent_token", "concurrent_token > 0");
                        });
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.HrimFeature", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("code")
                        .HasComment("Feature code");

                    b.Property<long>("ConcurrentToken")
                        .IsConcurrencyToken()
                        .HasColumnType("bigint")
                        .HasColumnName("concurrent_token")
                        .HasComment("Update is possible only when this token equals to the token in the storage");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("created_at")
                        .HasComment("Date and UTC time of entity instance creation");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("FeatureType")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("feature_type")
                        .HasComment("Could be one of:\nAnalysis");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<bool>("IsOn")
                        .HasColumnType("boolean")
                        .HasColumnName("is_on")
                        .HasComment("When a feature is off then its hangfire jobs, in case existed, should not be proceeded or scheduled.\nand in case feature represents an analysis (e.g. count, gap) this analysis should not appear in the list of available analysis.");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("updated_at")
                        .HasComment("Date and UTC time of entity instance last update ");

                    b.Property<string>("VariableName")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("variable_name")
                        .HasComment("Environment variable name that controls is this feature set on/off");

                    b.HasKey("Id");

                    b.ToTable("hrim_features", "hrim_analytics", t =>
                        {
                            t.HasComment("Features that might be on/off;\nfor example, analysis based on event-types, tags, events, etc");

                            t.HasCheckConstraint("CK_hrim_features_concurrent_token", "concurrent_token > 0");
                        });
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.HrimTag", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<long>("ConcurrentToken")
                        .IsConcurrencyToken()
                        .HasColumnType("bigint")
                        .HasColumnName("concurrent_token")
                        .HasComment("Update is possible only when this token equals to the token in the storage");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("created_at")
                        .HasComment("Date and UTC time of entity instance creation");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid")
                        .HasColumnName("created_by")
                        .HasComment("A user id who created a tag");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<string>("Tag")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("tag");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("updated_at")
                        .HasComment("Date and UTC time of entity instance last update ");

                    b.HasKey("Id");

                    b.HasIndex("CreatedById");

                    NpgsqlIndexBuilderExtensions.IncludeProperties(b.HasIndex("CreatedById"), new[] { "Tag" });

                    b.ToTable("hrim_tags", "hrim_analytics", t =>
                        {
                            t.HasComment("A tag that could be linked to an instance of any entity");

                            t.HasCheckConstraint("CK_hrim_tags_concurrent_token", "concurrent_token > 0");
                        });
                });

            modelBuilder.Entity("Hrim.Event.Analytics.EfCore.DbEntities.Events.DbDurationEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<long>("ConcurrentToken")
                        .IsConcurrencyToken()
                        .HasColumnType("bigint")
                        .HasColumnName("concurrent_token")
                        .HasComment("Update is possible only when this token equals to the token in the storage");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("created_at")
                        .HasComment("Date and UTC time of entity instance creation");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid")
                        .HasColumnName("created_by")
                        .HasComment("A user who created an instance of this event type");

                    b.Property<Guid>("EventTypeId")
                        .HasColumnType("uuid")
                        .HasColumnName("event_type_id")
                        .HasComment("Event type on which current event is based.");

                    b.Property<DateTimeOffset?>("FinishedAt")
                        .HasColumnType("timetz")
                        .HasColumnName("finished_at")
                        .HasComment("Time with end-user timezone when an event finished");

                    b.Property<DateOnly?>("FinishedOn")
                        .HasColumnType("date")
                        .HasColumnName("finished_on")
                        .HasComment("Date when an event finished");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTimeOffset>("StartedAt")
                        .HasColumnType("timetz")
                        .HasColumnName("started_at")
                        .HasComment("Time with end-user timezone when an event started");

                    b.Property<DateOnly>("StartedOn")
                        .HasColumnType("date")
                        .HasColumnName("started_on")
                        .HasComment("Date when an event started");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("updated_at")
                        .HasComment("Date and UTC time of entity instance last update ");

                    b.HasKey("Id");

                    b.HasIndex("EventTypeId");

                    b.HasIndex("CreatedById", "StartedOn");

                    NpgsqlIndexBuilderExtensions.IncludeProperties(b.HasIndex("CreatedById", "StartedOn"), new[] { "EventTypeId", "StartedAt", "FinishedOn", "FinishedAt", "IsDeleted" });

                    b.ToTable("duration_events", "hrim_analytics", t =>
                        {
                            t.HasComment("When it is important to register an event that has start time and end time this system_event_type can be used.\nThis kind of events may occur several times a day and can cross each other.");

                            t.HasCheckConstraint("CK_db_duration_events_concurrent_token", "concurrent_token > 0");
                        });
                });

            modelBuilder.Entity("Hrim.Event.Analytics.EfCore.DbEntities.Events.DbOccurrenceEvent", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasColumnName("id")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<long>("ConcurrentToken")
                        .IsConcurrencyToken()
                        .HasColumnType("bigint")
                        .HasColumnName("concurrent_token")
                        .HasComment("Update is possible only when this token equals to the token in the storage");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("created_at")
                        .HasComment("Date and UTC time of entity instance creation");

                    b.Property<Guid>("CreatedById")
                        .HasColumnType("uuid")
                        .HasColumnName("created_by")
                        .HasComment("A user who created an instance of this event type");

                    b.Property<Guid>("EventTypeId")
                        .HasColumnType("uuid")
                        .HasColumnName("event_type_id")
                        .HasComment("Event type on which current event is based.");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTimeOffset>("OccurredAt")
                        .HasColumnType("timetz")
                        .HasColumnName("occurred_at")
                        .HasComment("Time with end-user timezone when an event occurred");

                    b.Property<DateOnly>("OccurredOn")
                        .HasColumnType("date")
                        .HasColumnName("occurred_on")
                        .HasComment("Date when an event occurred");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("updated_at")
                        .HasComment("Date and UTC time of entity instance last update ");

                    b.HasKey("Id");

                    b.HasIndex("EventTypeId");

                    b.HasIndex("CreatedById", "OccurredAt");

                    NpgsqlIndexBuilderExtensions.IncludeProperties(b.HasIndex("CreatedById", "OccurredAt"), new[] { "EventTypeId", "IsDeleted" });

                    b.ToTable("occurrence_events", "hrim_analytics", t =>
                        {
                            t.HasComment("When the main importance is the fact that an event occurred.\nThis kind of events may occur several times a day.");

                            t.HasCheckConstraint("CK_db_occurrence_events_concurrent_token", "concurrent_token > 0");
                        });
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.Account.ExternalUserProfile", b =>
                {
                    b.HasOne("Hrim.Event.Analytics.Abstractions.Entities.Account.HrimUser", "HrimUser")
                        .WithMany("ExternalProfiles")
                        .HasForeignKey("HrimUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("HrimUser");
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.Analysis.AnalysisByEventType", b =>
                {
                    b.HasOne("Hrim.Event.Analytics.Abstractions.Entities.EventTypes.UserEventType", "EventType")
                        .WithMany("AnalysisSettings")
                        .HasForeignKey("EventTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EventType");
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.Analysis.StatisticsForEvent", b =>
                {
                    b.HasOne("Hrim.Event.Analytics.EfCore.DbEntities.Events.DbDurationEvent", null)
                        .WithMany("AnalysisResults")
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Hrim.Event.Analytics.EfCore.DbEntities.Events.DbOccurrenceEvent", null)
                        .WithMany("AnalysisResults")
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.Analysis.StatisticsForEventType", b =>
                {
                    b.HasOne("Hrim.Event.Analytics.Abstractions.Entities.EventTypes.UserEventType", null)
                        .WithMany("AnalysisResults")
                        .HasForeignKey("EntityId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.EventTypes.UserEventType", b =>
                {
                    b.HasOne("Hrim.Event.Analytics.Abstractions.Entities.Account.HrimUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.HrimTag", b =>
                {
                    b.HasOne("Hrim.Event.Analytics.Abstractions.Entities.Account.HrimUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("Hrim.Event.Analytics.EfCore.DbEntities.Events.DbDurationEvent", b =>
                {
                    b.HasOne("Hrim.Event.Analytics.Abstractions.Entities.Account.HrimUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Hrim.Event.Analytics.Abstractions.Entities.EventTypes.UserEventType", "EventType")
                        .WithMany()
                        .HasForeignKey("EventTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");

                    b.Navigation("EventType");
                });

            modelBuilder.Entity("Hrim.Event.Analytics.EfCore.DbEntities.Events.DbOccurrenceEvent", b =>
                {
                    b.HasOne("Hrim.Event.Analytics.Abstractions.Entities.Account.HrimUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Hrim.Event.Analytics.Abstractions.Entities.EventTypes.UserEventType", "EventType")
                        .WithMany()
                        .HasForeignKey("EventTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");

                    b.Navigation("EventType");
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.Account.HrimUser", b =>
                {
                    b.Navigation("ExternalProfiles");
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.EventTypes.UserEventType", b =>
                {
                    b.Navigation("AnalysisResults");

                    b.Navigation("AnalysisSettings");
                });

            modelBuilder.Entity("Hrim.Event.Analytics.EfCore.DbEntities.Events.DbDurationEvent", b =>
                {
                    b.Navigation("AnalysisResults");
                });

            modelBuilder.Entity("Hrim.Event.Analytics.EfCore.DbEntities.Events.DbOccurrenceEvent", b =>
                {
                    b.Navigation("AnalysisResults");
                });
#pragma warning restore 612, 618
        }
    }
}
