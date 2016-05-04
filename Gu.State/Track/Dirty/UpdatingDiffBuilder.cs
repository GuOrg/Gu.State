namespace Gu.State
{
    using System;
    using System.Reflection;

    internal sealed class UpdatingDiffBuilder : IDisposable
    {
        private readonly object x;
        private readonly object y;
        private readonly PropertiesSettings settings;
        private readonly Disposer<DiffBuilder> diffBuilderDisposer;

        public UpdatingDiffBuilder(object x, object y, PropertiesSettings settings)
        {
            this.x = x;
            this.y = y;
            this.settings = settings;
            this.diffBuilderDisposer = DiffBuilder.Create(x, y);
        }

        public ValueDiff ValueDiff => this.Builder.CreateValueDiff();

        private DiffBuilder Builder => this.diffBuilderDisposer.Value;

        public void Dispose()
        {
            this.diffBuilderDisposer.Dispose();
        }

        public void Update(PropertyInfo propertyInfo)
        {
            DiffBy.UpdateMemberDiff(this.x, this.y, propertyInfo, this.settings, this.Builder);
        }

        public void Update(RemoveEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Update(int index)
        {
            throw new NotImplementedException();
        }
    }
}
