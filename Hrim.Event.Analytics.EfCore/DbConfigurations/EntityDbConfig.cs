using Hrim.Event.Analytics.Abstractions.Entities;
using Hrimsoft.Data.PostgreSql.ValueConverters;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public static class EntityDbConfig {
    public const string POSTGRES_GUID_GENERATOR = "uuid_generate_v4()";

    public static void AddEntityProperties<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity {
        builder.HasKey(x => x.Id);
        builder.UseXminAsConcurrencyToken();

        builder.Property(p => p.Id)
               .HasColumnName(nameof(Entity.Id).ToSnakeCase())
               .HasDefaultValueSql(POSTGRES_GUID_GENERATOR);

        builder.Property(p => p.CreatedAt)
               .HasColumnName(nameof(Entity.CreatedAt).ToSnakeCase())
               .HasConversion(UtcDateTimeConverter.Get())
               .HasComment("Date and UTC time of entity instance creation")
               .HasColumnType("timestamptz")
               .IsRequired();

        builder.Property(p => p.UpdatedAt)
               .HasColumnName(nameof(Entity.UpdatedAt).ToSnakeCase())
               .HasConversion(UtcDateTimeConverter.Get())
               .HasComment("Date and UTC time of entity instance last update ")
               .HasColumnType("timestamptz");

        builder.Property(p => p.IsDeleted)
               .HasColumnName(nameof(Entity.IsDeleted).ToSnakeCase());

        builder.Property(p => p.ConcurrentToken)
               .HasColumnName("xmin");
    }
}