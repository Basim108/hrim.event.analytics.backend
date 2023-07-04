using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrimsoft.Data.PostgreSql.ValueConverters;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

// ReSharper disable once InconsistentNaming
public static class _StatisticsForEntityDbConfigExtension
{
    public static void AddStatisticsForEntityConfiguration<TEntity>(this EntityTypeBuilder<TEntity> builder, string entityIdComment)
        where TEntity : StatisticsForEntity {

        builder.HasKey(x => new {x.EntityId, x.AnalysisCode});
        
        builder.Property(p => p.EntityId)
               .HasColumnName(name: nameof(StatisticsForEntity.EntityId).ToSnakeCase())
               .HasComment(comment: entityIdComment)
               .IsRequired();
        
        builder.Property(p => p.AnalysisCode)
               .HasColumnName(name: nameof(StatisticsForEntity.AnalysisCode).ToSnakeCase())
               .HasComment(comment: "Code of analysis such as count, gap, etc")
               .IsRequired();

        builder.Property(p => p.StartedAt)
               .HasColumnName(nameof(StatisticsForEntity.StartedAt).ToSnakeCase())
               .HasConversion(UtcDateTimeConverter.Get())
               .HasComment(comment: "Date and UTC time when an analysis has been started.")
               .HasColumnType(typeName: "timestamptz")
               .IsRequired();

        builder.Property(p => p.FinishedAt)
               .HasColumnName(nameof(StatisticsForEntity.FinishedAt).ToSnakeCase())
               .HasConversion(UtcDateTimeConverter.Get())
               .HasComment(comment: "Date and UTC time when an analysis has been finished.")
               .HasColumnType(typeName: "timestamptz");

        builder.Property(p => p.ResultJson)
               .HasColumnName("result");
        
        builder.Property(p => p.CorrelationId)
               .HasColumnName(name: nameof(StatisticsForEntity.CorrelationId).ToSnakeCase())
               .HasComment(comment: "The last run correlation id")
               .IsRequired();
    }
}