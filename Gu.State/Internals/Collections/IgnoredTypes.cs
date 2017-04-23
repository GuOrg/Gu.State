namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    internal class IgnoredTypes
    {
        private static readonly IReadOnlyList<Type> DefaultIgnoredTypes = new[]
                                                                              {
                                                                                  typeof(List<>),
                                                                                  typeof(Array),
                                                                                  typeof(Dictionary<,>),
                                                                                  typeof(ObservableCollection<>),
                                                                                  typeof(Collection<>),
                                                                                  typeof(HashSet<>)
                                                                              };

        private static readonly IgnoredTypes Default = new IgnoredTypes(null);

        private readonly ConcurrentDictionary<Type, bool> ignoredTypes = new ConcurrentDictionary<Type, bool>();

        private IgnoredTypes(IEnumerable<Type> ignoredTypes)
        {
            if (ignoredTypes != null)
            {
                foreach (var ignoredType in ignoredTypes)
                {
                    this.ignoredTypes[ignoredType] = true;
                }
            }

            foreach (var ignoredType in DefaultIgnoredTypes)
            {
                this.ignoredTypes[ignoredType] = true;
            }
        }

        public static IgnoredTypes Create(IEnumerable<Type> ignoredTypes)
        {
            if (ignoredTypes == null || !ignoredTypes.Any())
            {
                return Default;
            }

            return new IgnoredTypes(ignoredTypes);
        }

        public bool IsIgnoringType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (this.ignoredTypes.TryGetValue(type, out bool isIgnoring))
            {
                return isIgnoring;
            }

            if (type.IsArray ||
                type.IsImmutableList() ||
                type.IsImmutableArray() ||
                type.IsImmutableHashSet() ||
                type.IsImmutableDictionary())
            {
                this.ignoredTypes.TryAdd(type, true);
                return true;
            }

            if (!type.IsGenericType)
            {
                this.ignoredTypes.TryAdd(type, false);
                return false;
            }

            return this.IsIgnoredGeneric(type);
        }

        private bool IsIgnoredGeneric(Type type)
        {
            var genericDef = type.GetGenericTypeDefinition();
            if (this.ignoredTypes.TryGetValue(genericDef, out bool isIgnoring))
            {
                this.ignoredTypes.TryAdd(type, isIgnoring);
                return isIgnoring;
            }

            this.ignoredTypes.TryAdd(type, false);
            return false;
        }
    }
}
