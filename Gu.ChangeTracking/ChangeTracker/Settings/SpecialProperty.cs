namespace Gu.ChangeTracking
{
    using System;
    using System.Diagnostics;
    using System.Reflection;

    [DebuggerDisplay("Name: {Name} TrackAs: {TrackAs}")]
    [Serializable]
    public class SpecialProperty
    {
        private SpecialProperty() // for serialization
        {
        }

        public SpecialProperty(string declaringTypeName, string name, TrackAs trackAs)
        {
            Ensure.NotNullOrEmpty(declaringTypeName, nameof(declaringTypeName));
            Ensure.NotNullOrEmpty(name, nameof(name));
            Ensure.NotEqual(trackAs, TrackAs.Unknown, nameof(trackAs));

            this.DeclaringTypeName = declaringTypeName;
            this.Name = name;
            this.TrackAs = trackAs;
        }

        public SpecialProperty(PropertyInfo property, TrackAs trackAs)
        {
            Ensure.NotNull(property, nameof(property));
            Ensure.NotEqual(trackAs, TrackAs.Unknown, nameof(trackAs));
            this.DeclaringTypeName = property.DeclaringType?.FullName;
            this.Name = property.Name;
            this.TrackAs = trackAs;
        }

        /// <summary>
        /// The name of the declaring type
        /// </summary>
        public string DeclaringTypeName { get; }

        /// <summary>
        /// Gets or sets the full name of the type
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The reason <see cref="Name"/> is excluded from tracking.
        /// </summary>
        public TrackAs TrackAs { get; }

        public PropertyInfo AsProperty()
        {
            return Type.GetType(this.DeclaringTypeName, true).GetProperty(this.Name);
        }
    }
}