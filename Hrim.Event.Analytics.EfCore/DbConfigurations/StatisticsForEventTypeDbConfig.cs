using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class StatisticsForEventTypeDbConfig: IEntityTypeConfiguration<StatisticsForEventType>
{
    public void Configure(EntityTypeBuilder<StatisticsForEventType> builder) {
        builder.ToTable(name:  $"{nameof(StatisticsForEventType).ToSnakeCase()}s", 
                        schema: "v2_analysis",  
                        t => {
                            t.HasComment(comment: "Stores results of calculation analysis for event types");
                        });
        builder.AddStatisticsForEntityConfiguration("refers to an event type for which this calculation was made.");
    }
}