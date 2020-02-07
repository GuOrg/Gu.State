namespace Gu.State
{
    internal class SourceAndTargetValue
    {
        internal readonly object Source;
        internal readonly object SourceValue;
        internal readonly object Target;
        internal readonly object TargetValue;

        internal SourceAndTargetValue(object source, object sourceValue, object target, object targetValue)
        {
            this.Source = source;
            this.SourceValue = sourceValue;
            this.Target = target;
            this.TargetValue = targetValue;
        }
    }
}
