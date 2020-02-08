namespace Gu.State
{
    internal struct Skip
    {
        internal Skip(int i)
        {
            this.I = i;
        }

        internal int I { get; }

        public override string ToString()
        {
            return $"Skip({this.I})";
        }
    }
}
