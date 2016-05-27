namespace Gu.State
{
    internal struct Skip
    {
        public Skip(int i)
        {
            this.I = i;
        }

        public int I { get; }

        public override string ToString()
        {
            return $"Skip({this.I})";
        }
    }
}