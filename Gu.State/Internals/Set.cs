namespace Gu.State
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal static class Set
    {
        internal static IEnumerable<object> ElementsOrderedByHashCode(IEnumerable set)
        {
            var comparer = set.GetType()
                              .GetProperty("Comparer", Constants.DefaultPropertyBindingFlags)
                              ?.GetValue(set);
            if (comparer == null)
            {
                return set.Cast<object>()
                          .OrderBy(i => i.GetHashCode());
            }

            var getHashcodeMethod = comparer.GetType()
                                            .GetMethod("GetHashCode", new Type[] { set.GetType().GetItemType() });
            return set.Cast<object>()
                      .OrderBy(i => getHashcodeMethod.Invoke(comparer, new[] { i }));
        }
    }
}