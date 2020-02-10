namespace Voxar
{
    public interface IReceiver<T>
    {
        void ReceiveData(T data);
    }

    public interface IImageReceiver<T> : IReceiver<T>
    {
        void SetDataResolution(int x, int y);
    }
}