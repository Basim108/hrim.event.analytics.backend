using Hrim.Event.Analytics.Abstractions.Entities;
using Hrimsoft.Data.PostgreSql.ValueConverters;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

// ReSharper disable once InconsistentNaming
public static class _EntityDbConfigExtension
{
    private const string POSTGRES_GUID_GENERATOR = "uuid_generate_v4()";

    public static void AddEntityProperties<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : HrimEntity {
        builder.HasKey(x => x.Id);

        builder.Property(p => p.Id)
               .HasColumnName(nameof(HrimEntity.Id).ToSnakeCase())
               .HasDefaultValueSql(sql: POSTGRES_GUID_GENERATOR);

        builder.Property(p => p.CreatedAt)
               .HasColumnName(nameof(HrimEntity.CreatedAt).ToSnakeCase())
               .HasConversion(UtcDateTimeConverter.Get())
               .HasComment(comment: "Date and UTC time of entity instance creation")
               .HasColumnType(typeName: "timestamptz")
               .IsRequired();

        builder.Property(p => p.UpdatedAt)
               .HasColumnName(nameof(HrimEntity.UpdatedAt).ToSnakeCase())
               .HasConversion(UtcDateTimeConverter.Get())
               .HasComment(comment: "Date and UTC time of entity instance last update ")
               .HasColumnType(typeName: "timestamptz");

        builder.Property(p => p.IsDeleted)
               .HasColumnName(nameof(HrimEntity.IsDeleted).ToSnakeCase());

        var concurrentTokenColumn = nameof(HrimEntity.ConcurrentToken).ToSnakeCase();
        builder.Property(p => p.ConcurrentToken)
               .HasColumnName(name: concurrentTokenColumn)
               .HasComment(comment: "Update is possible only when this token equals to the token in the storage")
               .IsConcurrencyToken()
               .IsRequired();
        var checkConstraintName = $"CK_{typeof(TEntity).Name.ToSnakeCase()}s_{concurrentTokenColumn}";
        builder.HasCheckConstraint(name: checkConstraintName, $"{concurrentTokenColumn} > 0");
    }
}