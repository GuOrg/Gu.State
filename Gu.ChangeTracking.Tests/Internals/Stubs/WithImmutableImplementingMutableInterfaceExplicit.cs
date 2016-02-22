namespace Gu.ChangeTracking.Tests.Internals.Stubs
{
    public class WithImmutableImplementingMutableInterfaceExplicit : IMutableInterface
    {
        public readonly int ImmutableValue;
        int IMutableInterface.MutableValue { get; set; }
    }
}