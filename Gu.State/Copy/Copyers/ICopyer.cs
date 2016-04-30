namespace Gu.State
{
    using System;

    public interface ICopyer
    {
        void CopyItems<TSettings>(
            object source,
            object target,
            Action<object, object, TSettings, ReferencePairCollection> syncItem,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings;
    }
}