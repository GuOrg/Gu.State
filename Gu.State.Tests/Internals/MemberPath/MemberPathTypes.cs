namespace Gu.State.Tests.Internals
{
    public static class MemberPathTypes
    {
        public class With<T>
        {
            public T Value { get; }
        }

        public class Parent
        {
            public Child Child { get; }
        }

        public class Child
        {
            public Parent Parent { get; }
        }
    }
}
