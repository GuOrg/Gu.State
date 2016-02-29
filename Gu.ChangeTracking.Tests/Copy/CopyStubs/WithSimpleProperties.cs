namespace Gu.ChangeTracking.Tests.CopyStubs
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class WithSimpleProperties : INotifyPropertyChanged
    {
        internal static readonly TestComparer Comparer = new TestComparer();

        private int intValue;
        private int? nullableIntValue;
        private string stringValue;
        private StringSplitOptions enumValue;

        public WithSimpleProperties()
        {
        }

        public WithSimpleProperties(int intValue, int? nullableIntValue, string stringValue, StringSplitOptions enumValue)
        {
            this.intValue = intValue;
            this.nullableIntValue = nullableIntValue;
            this.stringValue = stringValue;
            this.enumValue = enumValue;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public int IntValue
        {
            get { return this.intValue; }
            set
            {
                if (value == this.intValue) return;
                this.intValue = value;
                this.OnPropertyChanged();
            }
        }

        public int? NullableIntValue
        {
            get { return this.nullableIntValue; }
            set
            {
                if (value == this.nullableIntValue) return;
                this.nullableIntValue = value;
                this.OnPropertyChanged();
            }
        }

        public string StringValue
        {
            get { return this.stringValue; }
            set
            {
                if (value == this.stringValue) return;
                this.stringValue = value;
                this.OnPropertyChanged();
            }
        }

        public StringSplitOptions EnumValue
        {
            get { return this.enumValue; }
            set
            {
                if (value == this.enumValue) return;
                this.enumValue = value;
                this.OnPropertyChanged();
            }
        }
        public void SetFields(int intValue, string stringValue)
        {
            this.intValue = intValue;
            this.stringValue = stringValue;
        }

        [NotifyPropertyChangedInvocator]
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal sealed class TestComparer : IEqualityComparer<WithSimpleProperties>, IComparer
        {
            public bool Equals(WithSimpleProperties x, WithSimpleProperties y)
            {
                if (ReferenceEquals(x, y))
                {
                    return true;
                }
                if (ReferenceEquals(x, null))
                {
                    return false;
                }
                if (ReferenceEquals(y, null))
                {
                    return false;
                }
                if (x.GetType() != y.GetType())
                {
                    return false;
                }
                return x.IntValue == y.IntValue && x.NullableIntValue == y.NullableIntValue && string.Equals(x.StringValue, y.StringValue) && x.EnumValue == y.EnumValue;
            }

            public int GetHashCode(WithSimpleProperties obj)
            {
                unchecked
                {
                    var hashCode = obj.IntValue;
                    hashCode = (hashCode * 397) ^ obj.NullableIntValue.GetHashCode();
                    hashCode = (hashCode * 397) ^ (obj.StringValue != null
                                                       ? obj.StringValue.GetHashCode()
                                                       : 0);
                    hashCode = (hashCode * 397) ^ (int)obj.EnumValue;
                    return hashCode;
                }
            }

            int IComparer.Compare(object x, object y)
            {
                return this.Equals((WithSimpleProperties)x, (WithSimpleProperties)y)
                           ? 0
                           : -1;
            }
        }
    }
}
