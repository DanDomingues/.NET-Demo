using System.Globalization;

namespace Northstar.Utility
{
    public static class GenericUtility
    {
        public static string FormatAsUSD(this double amount)
        {
            return amount.ToString("c", CultureInfo.GetCultureInfo("en-US"));
        }

        public static bool EqualsAny(this string s, params string[] values)
        {
            return values.Any(s.Equals);
        }

        public static TOut MoveInList<TModel, TKey, TOut>(
            TModel? element,
            List<TModel> list,
            Func<int, int> getNewIndex,
            Func<TModel, TModel, bool> compare,
            Func<TModel, TKey> getKey,
            Func<TOut> onSuccess,
            Func<string, TOut> onFail,
            Action<TModel[]> onNewList) where TKey : IComparable
        {
            if(element == null)
            {
                return onFail("Element cannot be null");
            }

            var inList = list.FirstOrDefault(e => compare(e, element));

            if(inList == null)
            {
                return onFail("Element not present in list");
            }

            var index = list.IndexOf(inList);
            var newIndex = getNewIndex(index);

            if(index < 0 || index >= list.Count || newIndex < 0 || newIndex >= list.Count)
            {
                return onFail("Invalid move operation");
            }

            var categoriesDict = list.ToDictionary(c => getKey(c));
            list.RemoveAt(index);
            list.Insert(newIndex, element);
            onNewList([.. list]);
            return onSuccess();
        }
    }
}