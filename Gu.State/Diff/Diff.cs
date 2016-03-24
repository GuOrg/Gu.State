namespace Gu.State
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    public class Diff : IDiff
    {
        public static readonly Diff Empty = new Diff(new IDiff[0]);

        internal Diff(IReadOnlyCollection<IDiff> diffs)
        {
            this.Diffs = diffs;
        }

        public IReadOnlyCollection<IDiff> Diffs { get; }

        public bool IsEmpty => this.Diffs.Count == 0;

        public Diff Without(PropertyInfo propertyInfo)
        {
            if (this.Diffs.OfType<IPropertyDiff>().Any(x => x.PropertyInfo == propertyInfo))
            {
                return new Diff(this.Diffs.Where(x => (x as IPropertyDiff)?.PropertyInfo != propertyInfo).ToArray());
            }

            return this;
        }

        public Diff With(PropertyInfo propertyInfo, object xValue, object yValue)
        {
            throw new System.NotImplementedException();
        }

        ///// <summary>
        ///// Compares x and y for equality using property values.
        ///// If a type implements IList the items of the list are compared
        ///// </summary>
        ///// <typeparam name="T">The type to compare</typeparam>
        ///// <param name="x">The first instance</param>
        ///// <param name="y">The second instance</param>
        ///// <param name="referenceHandling">
        ///// If Structural is used a deep equals is performed.
        ///// Default value is Throw
        ///// </param>
        ///// <param name="bindingFlags">The binding flags to use when getting properties</param>
        ///// <returns>True if <paramref name="x"/> and <paramref name="y"/> are equal</returns>
        //public static Diff PropertyValues<T>(
        //    T x,
        //    T y,
        //    ReferenceHandling referenceHandling = ReferenceHandling.Throw,
        //    BindingFlags bindingFlags = Constants.DefaultPropertyBindingFlags)
        //{
        //    var settings = PropertiesSettings.GetOrCreate(bindingFlags, referenceHandling);
        //    return PropertyValues(x, y, settings);
        //}

        ///// <summary>
        ///// Compares x and y for equality using property values.
        ///// If a type implements IList the items of the list are compared
        ///// </summary>
        ///// <typeparam name="T">The type of <paramref name="x"/> and <paramref name="y"/></typeparam>
        ///// <param name="x">The first instance</param>
        ///// <param name="y">The second instance</param>
        ///// <param name="settings">Specifies how equality is performed.</param>
        ///// <returns>True if <paramref name="x"/> and <paramref name="y"/> are equal</returns>
        //public static Diff PropertyValues<T>(T x, T y, PropertiesSettings settings)
        //{
        //    EqualBy.Verify.CanEqualByPropertyValues(x, y, settings);

        //    if (settings.ReferenceHandling == ReferenceHandling.StructuralWithReferenceLoops)
        //    {
        //        var referencePairs = new ReferencePairCollection();
        //        return PropertiesValuesEquals(x, y, settings, referencePairs);
        //    }

        //    return PropertiesValuesEquals(x, y, settings, null);
        //}
    }
}
