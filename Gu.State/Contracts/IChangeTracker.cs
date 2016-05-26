namespace Gu.State
{
    using System;
    using System.ComponentModel;

    /// <summary>A change tracker for an object instance. </summary>
    public interface IChangeTracker : INotifyPropertyChanged, IDisposable
    {
        /// <summary>This event is raised when a change on the tracked instance is detected.</summary>
        event EventHandler Changed;

        /// <summary>Gets a value that is incremented each time a change is detected in the tracked instance.</summary>
        int Changes { get; }

        /// <summary>Gets the settings that specifies how tracking is performed.</summary>
        PropertiesSettings Settings { get; }
    }
}