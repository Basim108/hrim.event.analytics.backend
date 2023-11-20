using Hrim.Event.Analytics.Abstractions.Entities;

#pragma warning disable CS1591
namespace Hrim.Event.Analytics.Abstractions.Extensions;

public static class HrimEntityExtensions
{
    /// <summary> Copies each item in the list </summary>
    public static IList<TItem> CopyListTo<TItem, TKey>(this IList<TItem> list)
        where TItem : HrimEntity<TKey>, new()
        where TKey : struct {
        var anotherList = new List<TItem>(capacity: list.Count);
        foreach (var item in list) {
            var another = new TItem();
            item.CopyTo(another: another);
            anotherList.Add(item: another);
        }

        return anotherList;
    }
}