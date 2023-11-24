using AutoMapper;
using Hrim.Event.Analytics.EfCore.AutoMapper;

namespace Hrim.Event.Analytics.Api.Tests.Infrastructure;

public class MapperFactory
{
    private static IMapper? _mapper;

    public static IMapper GetMapper()
        => _mapper ??= new MapperConfiguration(cfg => {
            cfg.AddProfile<DbDurationEventProfile>();
            cfg.AddProfile<DbOccurrenceEventProfile>();
            cfg.AddProfile<DbEventTypeProfile>();
            cfg.AddProfile<DbAnalysisConfigByEventTypeProfile>();
        }).CreateMapper();
}