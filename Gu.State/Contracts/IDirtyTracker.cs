namespace Gu.State
{
    using System;
    using System.ComponentModel;

    /// <summary>A tracker that tracks the difference between tow instances.</summary>
    public interface IDirtyTracker : INotifyPropertyChanged, IDisposable
    {
        /// <summary>Gets the settings specifying how tracking and equality is performed.</summary>
        PropertiesSettings Settings { get; }

        /// <summary>Gets a value indicating whether there is a difference between x and y.</summary>
        bool IsDirty { get; }

        /// <summary>Gets the difference between x and y. This is a mutable value.</summary>
        ValueDiff Diff { get; }
    }
}
