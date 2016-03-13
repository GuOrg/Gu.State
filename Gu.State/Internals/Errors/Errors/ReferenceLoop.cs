namespace Gu.State
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    [DebuggerDisplay("{GetType().Name} Loop: {Path.PathString}")]
    internal sealed class ReferenceLoop : Error, INotSupported, IWithErrors
    {
        public ReferenceLoop(MemberPath path)
        {
            this.Path = path;
            var errors = new List<Error> { new TypeErrors(path.RootType) };
            foreach (var node in path.Path.OfType<ITypedNode>())
            {
                var memberItem = node as IMemberItem;
                if (memberItem != null)
                {
                    errors.Add(new MemberErrors(memberItem.Member));
                }

                errors.Add(new TypeErrors(node.Type));
            }

            this.Errors = errors;
        }

        public MemberPath Path { get; }

        public IReadOnlyCollection<Error> Errors { get; }

        public StringBuilder AppendNotSupported(StringBuilder errorBuilder)
        {
            var fieldInfo = this.Path.LastMember as FieldInfo;
            if (fieldInfo != null)
            {
                return errorBuilder.AppendLine($"The field {fieldInfo.DeclaringType.PrettyName()}.{fieldInfo.Name} of type {fieldInfo.FieldType.PrettyName()} is in a reference loop.")
                                   .AppendLine($"  - The loop is {this.Path.PathString()}...");
            }

            var propertyInfo = this.Path.LastMember as PropertyInfo;
            if (propertyInfo != null)
            {
                return errorBuilder.AppendLine($"The property {propertyInfo.DeclaringType.PrettyName()}.{propertyInfo.Name} of type {propertyInfo.PropertyType.PrettyName()} is in a reference loop.")
                                   .AppendLine($"  - The loop is {this.Path.PathString()}...");
            }

            throw Throw.ExpectedParameterOfTypes<FieldInfo, PropertyInfo>(nameof(this.Path));
        }
    }
}