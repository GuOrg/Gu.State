namespace Gu.State
{
    public interface IRootChangeEventArgs
    {
        /// <summary>Gets the source instance that changed.</summary>
        object Source { get; }
    }
}