namespace Gu.ChangeTracking
{
    internal class SourceAndTargetValue
    {
        internal readonly object Source;
        internal readonly object SourceValue;
        internal readonly object Target;
        internal readonly object TargeteValue;

        public SourceAndTargetValue(object source, object sourceValue, object target, object targeteValue)
        {
            this.Source = source;
            this.SourceValue = sourceValue;
            this.Target = target;
            this.TargeteValue = targeteValue;
        }
    }
}
