using Hrim.Event.Analytics.Abstractions.Entities.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class HrimUserDbConfig: IEntityTypeConfiguration<HrimUser>
{
    public void Configure(EntityTypeBuilder<HrimUser> builder) {
        builder.ToTable(name: "hrim_users", t => t.HasComment(comment: "An authorized user"));

        builder.AddEntityProperties();

        builder.HasMany(x => x.ExternalProfiles)
               .WithOne(x => x.HrimUser);
    }
}