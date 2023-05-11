using Hrim.Event.Analytics.Abstractions.Entities;
using Hrimsoft.StringCases;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hrim.Event.Analytics.EfCore.DbConfigurations;

public class HrimTagDbConfig: IEntityTypeConfiguration<HrimTag>
{
    public void Configure(EntityTypeBuilder<HrimTag> builder) {
        builder.ToTable(name: "hrim_tags",
                        t => t.HasComment(comment: "A tag that could be linked to an instance of any entity"));

        builder.HasIndex(x => x.CreatedById)
               .IncludeProperties(x => x.Tag);

        builder.AddEntityProperties();

        builder.Property(p => p.Tag)
               .HasColumnName(nameof(HrimTag.Tag).ToSnakeCase())
               .IsRequired();

        builder.Property(p => p.CreatedById)
               .HasColumnName(nameof(HrimTag.CreatedBy).ToSnakeCase())
               .HasComment(comment: "A user id who created a tag")
               .IsRequired();
        builder.HasOne(x => x.CreatedBy);
    }
}