namespace Gu.ChangeTracking
{
    using System;
    using System.Diagnostics;

    [DebuggerDisplay("Name: {Name} TrackAs: {TrackAs}")]
    [Serializable]
    public class SpecialType 
    {
        private SpecialType() // for serialization
        {
        }

        public SpecialType(string fullTypeName, TrackAs trackAs)
        {
            Ensure.NotNullOrEmpty(fullTypeName, nameof(fullTypeName));
            Ensure.NotEqual(trackAs, TrackAs.Unknown, nameof(trackAs));
            Name = fullTypeName;
            TrackAs = trackAs;
        }

        public SpecialType(Type type, TrackAs trackAs)
        {
            Ensure.NotNull(type, nameof(type));
            Ensure.NotEqual(trackAs, TrackAs.Unknown, nameof(trackAs));
            Name = type.FullName;
            TrackAs = trackAs;
        }

        /// <summary>
        /// Gets or sets the full name of the type
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The reason <see cref="Name"/> is excluded from tracking.
        /// </summary>
        public TrackAs TrackAs { get; }
    }
}