using Riptide;

namespace LegacyNetworking
{
    public interface IObservable
    {
        public void OnSerialize(Message stream);
        public void OnDeserialize(Message stream);
    }
}
