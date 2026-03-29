namespace Demo.Utility
{
    public static class ValidationUtility
    {
        public static void IfIdValid<T>(Func<int, T> getById, Action<T> action, int id)
        {
            var obj = getById(id);
            if(obj == null)
            {
                return;
            }
            
            action.Invoke(obj);
        }

        public static void IfIdAndArgsValid<T>(Func<int, T> getById, Action<T> action, int id, params string[] args)
        {
            if(args?.Any(a => string.IsNullOrEmpty(a)) != false)
            {
                return;
            }
            IfIdValid(getById, action, id);
        }

        //TODO-4: Rework this into a more composite, additive style 
        /*
        protected void IfArgsValid(Action<OrderHeader> action, int id, params string[] args)
        {
            if (args?.Any(a => string.IsNullOrEmpty(a)) != false)
            {
                return;
            }
            action?.Invoke(GetById(id));
        }
        */
    }
}