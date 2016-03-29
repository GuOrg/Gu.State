using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gu.State
{
    public static partial class DiffBy
    {
        private static bool TryGetValueDiff(object x, object y, out ValueDiff diff)
        {
            if (ReferenceEquals(x, y))
            {
                diff = null;
                return true;
            }

            if (x == null || y == null)
            {
                diff = new ValueDiff(x, y);
                return true;
            }

            if (x.GetType() != y.GetType())
            {
                diff = new ValueDiff(x, y);
                return true;
            }

            if (x.GetType().IsEquatable())
            {
                diff = Equals(x, y)
                           ? null
                           : new ValueDiff(x, y);
                return true;
            }

            diff = null;
            return false;
        }
    }
}
