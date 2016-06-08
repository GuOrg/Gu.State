namespace Gu.State
{
    using System;
    using System.Collections.Concurrent;
    using System.Runtime.CompilerServices;

    internal static class ErrorCache
    {
        private static readonly ConditionalWeakTable<IMemberSettings, ConcurrentDictionary<Type, TypeErrors>> SettingEqualByErrorsMap = new ConditionalWeakTable<IMemberSettings, ConcurrentDictionary<Type, TypeErrors>>();

        internal static ConcurrentDictionary<Type, TypeErrors> EqualByErrors(this IMemberSettings settings)
        {
            return SettingEqualByErrorsMap.GetOrCreateValue(settings);
        }
    }
}
