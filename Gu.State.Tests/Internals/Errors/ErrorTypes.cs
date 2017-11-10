// ReSharper disable All
// ReSharper disable All
#pragma warning disable INPC003 // Notify when property changes.
#pragma warning disable INPC001 // Implement INotifyPropertyChanged.
#pragma warning disable 67
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

#pragma warning disable WPF1010 // Mutable public property should notify.
            public T Value { get; set; }
#pragma warning restore WPF1010 // Mutable public property should notify.
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
