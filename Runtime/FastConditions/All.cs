namespace Utilities.FastConditions
{
    public static class All
    {
        public static bool IsNull(params object[] objects)
        {
            foreach (var obj in objects)
            {
                if (obj is not null)
                    return false;
            }

            return true;
        }

        public static bool IsNotNull(params object[] objects)
        {
            foreach (var obj in objects)
            {
                if (obj is null)
                    return false;
            }

            return true;
        }
    }
}