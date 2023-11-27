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
    
    public static void AddEntityProperties<TEntity, TKey>(this EntityTypeBuilder<TEntity> builder)
        where TKey : struct
        where TEntity : HrimEntity<TKey> {
        builder.HasKey(x => x.Id);

        if (typeof(TKey) == typeof(Guid)) {
            builder.Property(p => p.Id)
                   .HasColumnName(nameof(HrimEntity<TKey>.Id).ToSnakeCase())
                   .HasDefaultValueSql(sql: POSTGRES_GUID_GENERATOR);
        }
        else {
            builder.Property(p => p.Id)
                   .HasColumnName(nameof(HrimEntity<TKey>.Id).ToSnakeCase())
                   .UseIdentityAlwaysColumn();
        }
        builder.Property(p => p.CreatedAt)
               .HasColumnName(nameof(HrimEntity<TKey>.CreatedAt).ToSnakeCase())
               .HasConversion(UtcDateTimeConverter.Get())
               .HasComment(comment: "Date and UTC time of entity instance creation")
               .HasColumnType(typeName: "timestamptz")
               .IsRequired();

        builder.Property(p => p.UpdatedAt)
               .HasColumnName(nameof(HrimEntity<TKey>.UpdatedAt).ToSnakeCase())
               .HasConversion(UtcDateTimeConverter.Get())
               .HasComment(comment: "Date and UTC time of entity instance last update ")
               .HasColumnType(typeName: "timestamptz");

        builder.Property(p => p.IsDeleted)
               .HasColumnName(nameof(HrimEntity<TKey>.IsDeleted).ToSnakeCase());

        var concurrentTokenColumn = nameof(HrimEntity<TKey>.ConcurrentToken).ToSnakeCase();
        builder.Property(p => p.ConcurrentToken)
               .HasColumnName(name: concurrentTokenColumn)
               .HasComment(comment: "Update is possible only when this token equals to the token in the storage")
               .IsConcurrencyToken()
               .IsRequired();
        builder.ToTable(t => {
            var checkConstraintName = $"CK_{typeof(TEntity).Name.ToSnakeCase()}s_{concurrentTokenColumn}";
            t.HasCheckConstraint(name: checkConstraintName, $"{concurrentTokenColumn} > 0");
        });
    }
}