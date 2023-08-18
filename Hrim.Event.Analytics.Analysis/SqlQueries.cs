namespace Hrim.Event.Analytics.Analysis;

public static class SqlQueries
{
    public const string GET_EVENT_TYPE_PARENTS = @"
WITH RECURSIVE cte AS (
        select et1.id, et1.parent_id
        from hrim_analytics.event_types et1
        where et1.id = {0}
    UNION
        select et2.id, et2.parent_id
        from hrim_analytics.event_types et2
        join cte on et2.id = cte.parent_id
) SELECT id FROM cte";
    
}