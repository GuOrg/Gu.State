namespace Gu.State.Tests.Internals
{
    public class MemberPathTypes
    {
        public class With<T>
        {
            public T Value { get; }
        }

        public class WithSelfProp
        {
            public WithSelfProp Value { get; }
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
