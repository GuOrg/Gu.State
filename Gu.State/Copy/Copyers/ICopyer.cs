namespace Gu.State
{
    public interface ICopyer
    {
        void Copy<TSettings>(
            object source,
            object target,
            TSettings settings,
            ReferencePairCollection referencePairs)
            where TSettings : class, IMemberSettings;
    }
}