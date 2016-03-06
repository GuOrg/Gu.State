namespace Gu.ChangeTracking
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    internal sealed class MemberTypeErrors : MemberError, IErrors, IFixWithEquatable, IFixWithImmutable, IExcludableType, IExcludableMember, INotSupported
    {
        public MemberTypeErrors(MemberInfo member, MemberPath path, Error error)
            : base(member, path)
        {
            this.Error = error;
        }

        public Error Error { get; }

        Type IFixWithEquatable.Type => this.SourceType();

        Type IFixWithImmutable.Type => this.SourceType();

        Type IExcludableType.Type => this.MemberInfo.MemberType();

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            var fieldInfo = this.MemberInfo as FieldInfo;
            if (fieldInfo != null)
            {
                return errorBuilder.AppendLine($"The field {this.SourceType()?.PrettyName()}.{fieldInfo.Name} of type {fieldInfo.FieldType.PrettyName()} is not supported.");
            }

            var propertyInfo = this.MemberInfo as PropertyInfo;
            if (propertyInfo != null)
            {
                return errorBuilder.AppendLine($"The property {this.SourceType()?.PrettyName()}.{propertyInfo.Name} of type {propertyInfo.PropertyType.PrettyName()} is not supported.");
            }

            throw Throw.ExpectedParameterOfTypes<FieldInfo, PropertyInfo>(nameof(this.MemberInfo));
        }

        StringBuilder IExcludableMember.AppendSuggestExcludeMember(StringBuilder errorBuilder)
        {
            return AppendSuggestExcludeMember(errorBuilder, this.SourceType(), this.MemberInfo);
        }

        public IEnumerator<Error> GetEnumerator()
        {
            yield return this;
            var errors = this.Error as IErrors;
            if (errors != null)
            {
                foreach (var error in errors)
                {
                    yield return error;
                }
            }
            else
            {
                yield return this.Error;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}