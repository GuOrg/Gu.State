namespace Gu.State
{
    internal interface IInitialize<out T>
    {
        T Initialize();
    }
}