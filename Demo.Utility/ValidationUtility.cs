namespace Demo.Utility
{
    public static class ValidationUtility
    {
        public static bool IfIdValid<T>(Func<int, T>? getById = null, Action<T>? action = null, int id = 0)
        {
            var obj = getById != null ? getById(id) : default;
            if(obj == null)
            {
                return false;
            }
            action?.Invoke(obj);
            return true;
        }

        public static bool IfArgsValid<T>(Func<int, T>? getById = null, Action<T>? action = null, int id = 0, params string[] args)
        {   
            if (args?.Any(a => string.IsNullOrEmpty(a)) != false)
            {
                return false;
            }
            
            var obj = getById != null ? getById(id) : default;

            if(obj != null)
            {
                action?.Invoke(obj);
            }
            
            return true;
        }

        public static bool IfIdAndArgsValid<T>(Func<int, T>? getById = null, Action<T>? action = null, int id = 0, params string[] args)
        {
            return IfIdValid<T>(id: id) && IfArgsValid(getById, action, id, args); 
        }
    }
}