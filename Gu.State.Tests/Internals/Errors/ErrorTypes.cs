namespace Gu.State.Tests.Internals.Errors
{
    using System.ComponentModel;

    public static class ErrorTypes
    {
        public class With<T>
        {
            public T Value { get; set; }
        }

        public class Notifying<T> : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            public T Value { get; set; }
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

        public class WithIndexer
        {
            // ReSharper disable once UnusedParameter.Global
            public int this[int index]
            {
                get { return 0; }
                set { }
            }
        }

        public class ComplexType
        {
            public string Name { get; set; }

            public int Value { get; set; }
        }
    }
}
