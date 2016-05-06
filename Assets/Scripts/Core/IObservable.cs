using UnityEngine;

namespace FZ
{
    /**
    *@brief 옵저버 관리자 인터페이스
    *@details 옵저버 패턴에서 관찰자들을 관리하는 관리자 인터페이스.
    *@author Delight
    */
    public interface IObservable<T>
    {
        /**
        *@brief 옵저버 등록 메소드
        *@details 구현시에는 리스트 등을 활용하여 observer를 등록해둔다.
        */
        void Subscribe(IObserver<T> observer);

        /**
        *@brief 옵저버 등록 해제 메소드
        *@details 등록된 리스트 등에서 해당 observer를 해제한다.
        */
        void Unsubscribe(IObserver<T> observer);
    }

    /**
    *@brief 옵저버 인터페이스
    *@details 옵저버 패턴에서 관찰자 인터페이스
    *@author Delight
    */
    public interface IObserver<T>
    {
        /**
        *@brief 변경 통지 메소드
        *@details subject의 변경이 통지되는 메소드. subject의 정보가 매개변수로 전달된다.
        */
        void OnNext(T data);
    }
}
