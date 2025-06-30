namespace Utilities.FastConditions
{
    public static class Any
    {
        public static bool IsNull(params object[] objects)
        {
            foreach (var obj in objects)
            {
                if (obj is null)
                    return true;
            }

            return false;
        }

        public static bool IsNotNull(params object[] objects)
        {
            foreach (var obj in objects)
            {
                if (obj is not null)
                    return true;
            }

            return false;
        }
    }
}