using Hrim.Event.Analytics.Abstractions.Entities;
using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations; 

public class HrimUserDbConfig: IEntityTypeConfiguration<HrimUser> {
    public void Configure(EntityTypeBuilder<HrimUser> builder) {
        builder.ToTable("hrim_users", t => t.HasComment("An authorized user"));

        builder.AddEntityProperties();

        builder.HasMany(x => x.ExternalProfiles)
               .WithOne(x => x.HrimUser);
    }
}