namespace Gu.State
{
    using System;
    using System.Reflection;

    internal sealed class UpdatingDiffBuilder : IDisposable
    {
        private readonly Disposer<DiffBuilder> diffBuilderDisposer;

        public UpdatingDiffBuilder(object x, object y)
        {
            this.diffBuilderDisposer = DiffBuilder.Create(x, y);
            this.ValueDiff = this.diffBuilderDisposer.Value.CreateValueDiff();
        }

        public ValueDiff ValueDiff { get; }

        public void Dispose()
        {
            this.diffBuilderDisposer.Dispose();
        }

        public void Update(PropertyInfo propertyInfo)
        {
            throw new NotImplementedException();
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
