namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    internal class KnownTypes
    {
        private static readonly IReadOnlyList<Type> DefaultKnownTypes = new[]
                                                                              {
                                                                                  typeof(List<>),
                                                                                  typeof(Array),
                                                                                  typeof(Dictionary<,>),
                                                                                  typeof(ObservableCollection<>),
                                                                                  typeof(Collection<>),
                                                                                  typeof(HashSet<>)
                                                                              };

        private static readonly KnownTypes Default = new KnownTypes(null);

        private readonly ConcurrentDictionary<Type, bool> knownTypes = new ConcurrentDictionary<Type, bool>();

        private KnownTypes(IEnumerable<Type> ignoredTypes)
        {
            if (ignoredTypes != null)
            {
                foreach (var ignoredType in ignoredTypes)
                {
                    this.knownTypes[ignoredType] = true;
                }
            }

            foreach (var ignoredType in DefaultKnownTypes)
            {
                this.knownTypes[ignoredType] = true;
            }
        }

        public static KnownTypes Create(IEnumerable<Type> ignoredTypes)
        {
            if (ignoredTypes == null || !ignoredTypes.Any())
            {
                return Default;
            }

            return new KnownTypes(ignoredTypes);
        }

        public bool IsKnownType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (this.knownTypes.TryGetValue(type, out var isIgnoring))
            {
                return isIgnoring;
            }

            if (type.IsArray ||
                type.IsInSystemCollections())
            {
                this.knownTypes.TryAdd(type, true);
                return true;
            }

            if (!type.IsGenericType)
            {
                this.knownTypes.TryAdd(type, false);
                return false;
            }

            return this.IsKnownGeneric(type);
        }

        private bool IsKnownGeneric(Type type)
        {
            var genericDef = type.GetGenericTypeDefinition();
            if (this.knownTypes.TryGetValue(genericDef, out var isIgnoring))
            {
                this.knownTypes.TryAdd(type, isIgnoring);
                return isIgnoring;
            }

            this.knownTypes.TryAdd(type, false);
            return false;
        }
    }
}
