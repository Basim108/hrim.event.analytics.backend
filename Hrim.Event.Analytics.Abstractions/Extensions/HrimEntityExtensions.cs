using Hrim.Event.Analytics.Abstractions.Entities;

#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions.Extensions;

public static class HrimEntityExtensions
{
    /// <summary> Copies each item in the list </summary>
    public static IList<TItem> CopyListTo<TItem>(this IList<TItem> list)
        where TItem : HrimEntity, new()
    {
        var anotherList = new List<TItem>(list.Count);
        foreach (var item in list)
        {
            var another = new TItem();
            item.CopyTo(another);
            anotherList.Add(another);
        }

        return anotherList;
    }
}