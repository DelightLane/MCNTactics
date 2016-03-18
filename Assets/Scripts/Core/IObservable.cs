using UnityEngine;

namespace MCN
{
    public interface IObservable<T>
    {
        void Subscribe(IObserver<T> observer);
        void Unsubscribe(IObserver<T> observer);
    }

    public interface IObserver<T>
    {
        void OnNext(T data);
    }
}
