namespace Gu.State
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    [DebuggerDisplay("{GetType().Name} Loop: {Path.PathString()}")]
    internal sealed class ReferenceLoop : Error, INotSupported, IWithErrors
    {
        internal ReferenceLoop(MemberPath path)
        {
            this.Path = path;
            var errors = new List<Error>();
            var typeErrors = path.OfType<ITypedNode>()
                                   .Select(x => x.Type)
                                   .Append(path.RootType)
                                   .Distinct()
                                   .Select(t => new TypeErrors(t));
            errors.AddRange(typeErrors);

            var memberErrors = path.OfType<IMemberItem>()
                                     .Select(x => x.Member)
                                     .Distinct()
                                     .Select(m => new MemberErrors(m));
            errors.AddRange(memberErrors);
            this.Errors = errors;
        }

        internal MemberPath Path { get; }

        public IReadOnlyList<Error> Errors { get; }

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            if (this.Path.LastMember is FieldInfo fieldInfo)
            {
                return errorBuilder.AppendLine($"The field {fieldInfo.DeclaringType.PrettyName()}.{fieldInfo.Name} of type {fieldInfo.FieldType.PrettyName()} is in a reference loop.")
                                   .AppendLine($"  - The loop is {this.Path.PathString()}...");
            }

            if (this.Path.LastMember is PropertyInfo propertyInfo)
            {
                return errorBuilder.AppendLine($"The property {propertyInfo.DeclaringType.PrettyName()}.{propertyInfo.Name} of type {propertyInfo.PropertyType.PrettyName()} is in a reference loop.")
                                   .AppendLine($"  - The loop is {this.Path.PathString()}...");
            }

            throw Throw.ExpectedParameterOfTypes<FieldInfo, PropertyInfo>(nameof(this.Path));
        }
    }
}
