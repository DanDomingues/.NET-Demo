namespace Northstar.Utility
{
    public static class ViewUtility
    {
        public static string GetNavigationEnabledClass<T>(
            T element, 
            IEnumerable<T> collection,
            Func<T, int> getIndex,
            int navigationDirection,
            string enabledClass = "",
            string disabledClass = "disabled") where T : class
        {
            var index = getIndex(element);
            if(navigationDirection < 0)
            {
                return index == 0 ? disabledClass : enabledClass;
            }
            else if(navigationDirection > 0)
            {
                return index == collection.Count() - 1 ? disabledClass : enabledClass;
            }
            else
            {
                throw new InvalidCastException("Invalid navigation direction");
            }
        }
    }
}