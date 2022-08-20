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
                .HasAnnotation("ProductVersion", "6.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.HasPostgresExtension(modelBuilder, "uuid-ossp");
            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.EventTypes.SystemEventType", b =>
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

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("EventType")
                        .HasColumnType("integer");

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

                    b.ToTable("event_types", "hrim_analytics");

                    b.HasDiscriminator<string>("Discriminator").HasValue("SystemEventType");

                    b.HasComment("User defined event types.\nhttps://hrimsoft.atlassian.net/wiki/spaces/HRIMCALEND/pages/65566/System+Event+Types");

                    b.HasCheckConstraint("CK_system_event_types_concurrent_token", "concurrent_token > 0");
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

                    b.ToTable("hrim_tags", "hrim_analytics");

                    b.HasComment("A tag that could be linked to an instance of any entity");

                    b.HasCheckConstraint("CK_hrim_tags_concurrent_token", "concurrent_token > 0");
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.HrimUser", b =>
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
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<DateTime?>("UpdatedAt")
                        .HasColumnType("timestamptz")
                        .HasColumnName("updated_at")
                        .HasComment("Date and UTC time of entity instance last update ");

                    b.HasKey("Id");

                    b.ToTable("hrim_users", "hrim_analytics");

                    b.HasComment("An authorized user");

                    b.HasCheckConstraint("CK_hrim_users_concurrent_token", "concurrent_token > 0");
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
                        .HasColumnType("uuid");

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

                    b.Property<bool>("IsPublic")
                        .HasColumnType("boolean")
                        .HasColumnName("is_public")
                        .HasComment("An owner who created this event_type could share it with other end-users.\nWill override IsPublic value of an event_type instance");

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

                    b.HasIndex("CreatedById", "StartedOn");

                    NpgsqlIndexBuilderExtensions.IncludeProperties(b.HasIndex("CreatedById", "StartedOn"), new[] { "StartedAt", "FinishedOn", "FinishedAt", "IsPublic" });

                    b.ToTable("duration_event_types", "hrim_analytics");

                    b.HasComment("When it is important to register an event that has start time and end time this system_event_type can be used.\nThis kind of events may occur several times a day and can cross each other.");

                    b.HasCheckConstraint("CK_db_duration_events_concurrent_token", "concurrent_token > 0");
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
                        .HasColumnType("uuid");

                    b.Property<bool?>("IsDeleted")
                        .HasColumnType("boolean")
                        .HasColumnName("is_deleted");

                    b.Property<bool>("IsPublic")
                        .HasColumnType("boolean")
                        .HasColumnName("is_public")
                        .HasComment("An owner who created this event_type could share it with other end-users.\nWill override IsPublic value of an event_type instance");

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

                    b.HasIndex("CreatedById", "OccurredAt");

                    NpgsqlIndexBuilderExtensions.IncludeProperties(b.HasIndex("CreatedById", "OccurredAt"), new[] { "IsPublic" });

                    b.ToTable("occurrence_events", "hrim_analytics");

                    b.HasComment("When the main importance is the fact that an event occurred.\nThis kind of events may occur several times a day.");

                    b.HasCheckConstraint("CK_db_occurrence_events_concurrent_token", "concurrent_token > 0");
                });

            modelBuilder.Entity("Hrim.Event.Analytics.EfCore.DbEntities.EventTypes.DbDurationEventType", b =>
                {
                    b.HasBaseType("Hrim.Event.Analytics.Abstractions.Entities.EventTypes.SystemEventType");

                    b.Property<DateTimeOffset?>("FinishedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateOnly?>("FinishedOn")
                        .HasColumnType("date");

                    b.Property<DateTimeOffset>("StartedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateOnly>("StartedOn")
                        .HasColumnType("date");

                    b.HasDiscriminator().HasValue("DbDurationEventType");

                    b.HasCheckConstraint("CK_system_event_types_concurrent_token", "concurrent_token > 0");
                });

            modelBuilder.Entity("Hrim.Event.Analytics.EfCore.DbEntities.EventTypes.DbOccurrenceEventType", b =>
                {
                    b.HasBaseType("Hrim.Event.Analytics.Abstractions.Entities.EventTypes.SystemEventType");

                    b.Property<DateTimeOffset>("OccurredAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateOnly>("OccurredOn")
                        .HasColumnType("date");

                    b.HasDiscriminator().HasValue("DbOccurrenceEventType");

                    b.HasCheckConstraint("CK_system_event_types_concurrent_token", "concurrent_token > 0");
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.EventTypes.SystemEventType", b =>
                {
                    b.HasOne("Hrim.Event.Analytics.Abstractions.Entities.HrimUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("Hrim.Event.Analytics.Abstractions.Entities.HrimTag", b =>
                {
                    b.HasOne("Hrim.Event.Analytics.Abstractions.Entities.HrimUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("Hrim.Event.Analytics.EfCore.DbEntities.Events.DbDurationEvent", b =>
                {
                    b.HasOne("Hrim.Event.Analytics.Abstractions.Entities.HrimUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");
                });

            modelBuilder.Entity("Hrim.Event.Analytics.EfCore.DbEntities.Events.DbOccurrenceEvent", b =>
                {
                    b.HasOne("Hrim.Event.Analytics.Abstractions.Entities.HrimUser", "CreatedBy")
                        .WithMany()
                        .HasForeignKey("CreatedById")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("CreatedBy");
                });
#pragma warning restore 612, 618
        }
    }
}
