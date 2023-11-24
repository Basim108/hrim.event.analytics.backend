using Hrim.Event.Analytics.Abstractions.Entities.Analysis;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class StatisticsForEventDbConfig: IEntityTypeConfiguration<StatisticsForEvent>
{
    public void Configure(EntityTypeBuilder<StatisticsForEvent> builder) {
        builder.ToTable(name: $"{nameof(StatisticsForEvent).ToSnakeCase()}s", 
                        schema: "v2_analysis",  
                        t => {
                            t.HasComment(comment: "Stores results of calculation analysis for event types");
                        });
        builder.AddStatisticsForEntityConfiguration("refers to an occurrence/duration event for which this calculation was made.");
    }
}