namespace Gu.State
{
    /// <summary>A custom copy implementation.</summary>
    public interface ICopyer
    {
        /// <summary>Copy <paramref name="source"/> to <paramref name="target"/> or create a new value if target is null.</summary>
        /// <typeparam name="TSettings">The setting type.</typeparam>
        /// <param name="source">The source value.</param>
        /// <param name="target">The target value.</param>
        /// <param name="settings">The settings.</param>
        /// <param name="referencePairs">A collection with previously copied values.</param>
        void Copy<TSettings>(
            object source,
            object target,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings;
    }
}