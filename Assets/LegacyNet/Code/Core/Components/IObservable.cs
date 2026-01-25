using Riptide;

namespace LegacyNetworking
{
    public interface IObservable
    {
        public void OnSerializeView(ref Message stream, bool isWriting);
    }
}
