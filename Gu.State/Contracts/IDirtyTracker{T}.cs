namespace Gu.State
{
    using System;
    using System.ComponentModel;

    public interface IDirtyTracker : INotifyPropertyChanged, IDisposable
    {
        /// <summary>The settings specifying how tracking and equality is performed.</summary>
        PropertiesSettings Settings { get; }

        /// <summary>True if there is a difference between x and y.</summary>
        bool IsDirty { get; }

        /// <summary>The difference between x and y. This is a mutable value.</summary>
        ValueDiff Diff { get; }
    }
}