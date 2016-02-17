namespace Gu.ChangeTracking.Tests.PropertySynchronizerStubs
{
    using System;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    using JetBrains.Annotations;

    public class WithSimpleProperties : INotifyPropertyChanged
    {
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
    }
}
