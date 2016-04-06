namespace Gu.State
{
    internal interface IGetterAndSetter
    {
        void SetValue(object source, object value);

        object GetValue(object source);
    }
}
