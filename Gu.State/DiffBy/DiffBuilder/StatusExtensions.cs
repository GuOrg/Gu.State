namespace Gu.State
{
    internal static class StatusExtensions
    {
        public static bool IsFlagSet(this Status value, Status flag)
        {
            return (value & flag) != 0;
        }

        public static Status SetFlag(this Status value, Status flag, bool set)
        {
            return set
                ? value.SetFlag(flag)
                : value.ClearFlag(flag);
        }

        internal static Status SetFlag(this Status value, Status flag)
        {
            return value | flag;
        }

        internal static Status ClearFlag(this Status value, Status flag)
        {
            return value & ~flag;
        }
    }
}