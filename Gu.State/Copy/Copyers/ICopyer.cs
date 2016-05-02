namespace Gu.State
{
    using System;

    public interface ICopyer
    {
        void Copy<TSettings>(
            object source,
            object target,
            Func<object, object, TSettings, ReferencePairCollection, object> copyItem,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings;
    }
}