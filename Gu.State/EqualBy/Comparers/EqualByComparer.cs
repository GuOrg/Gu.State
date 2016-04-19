namespace Gu.State
{
    using System;

    /// <summary>A comparer that compares by member or index.</summary>
    public abstract class EqualByComparer
    {
        /// <summary>
        /// Compare <paramref name="x"/> with <paramref name="y"/>
        /// </summary>
        /// <typeparam name="TSetting">The setting type.</typeparam>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="compareItem">The logic for how nested values are compared.</param>
        /// <param name="settings">The settings that specifies how comparison is performed.</param>
        /// <param name="referencePairs">
        /// The already compared items. This is used to stop infinite recursion when there are reference loops.
        /// </param>
        /// <returns>True if <paramref name="x"/> and <paramref name="y"/> are equal.</returns>
        public abstract bool Equals<TSetting>(
            object x,
            object y,
            Func<object, object, TSetting, ReferencePairCollection, bool> compareItem,
            TSetting settings,
            ReferencePairCollection referencePairs)
            where TSetting : class, IMemberSettings;

        /// <summary>Convenience method for checking equality if either or both <paramref name="x"/> and <paramref name="y"/> are null.</summary>
        /// <param name="x">The x value.</param>
        /// <param name="y">The y value.</param>
        /// <param name="result">The result.</param>
        /// <returns>True if any of <paramref name="x"/> and <paramref name="y"/> is null.</returns>
        protected static bool TryGetEitherNullEquals(object x, object y, out bool result)
        {
            if (x == null && y == null)
            {
                result = true;
                return true;
            }

            if (x == null || y == null)
            {
                result = false;
                return true;
            }

            result = false;
            return false;
        }
    }
}
