using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using FZ;

// TODO : 더 많은 터치 이벤트들을 구현해야 함
public enum eTouchEvent
{
    TOUCH
}

public class TouchManager : MonoSingletone<TouchManager>, IObservable<eTouchEvent>
{
    private List<IObserver<eTouchEvent>> _observers;

    void Awake()
    {
        _observers = new List<IObserver<eTouchEvent>>();
    }

    void Update()
    {
        // TODO : 마우스가 아니라 실제로 터치 이벤트에 대해 동작하게 수정
        if (Input.GetMouseButtonUp(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(Camera.main.transform.position, ray.direction * 1000, Color.red, 100);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            if (hits.Length > 0)
            {
                foreach (var hit in hits)
                {
                    var gameObjObserver = _observers.Find((IObserver<eTouchEvent> ob) => ob is TacticsObject && (ob as TacticsObject).transform == hit.transform);

                    if (gameObjObserver != null)
                    {
                        if (((TacticsObject)gameObjObserver).transform == hit.transform)
                        {
                            gameObjObserver.OnNext(eTouchEvent.TOUCH);
                            return;
                        }
                    }
                }
            }
        }
    }

    public void Subscribe(IObserver<eTouchEvent> observer)
    {
        if (_observers != null)
        {
            _observers.Add(observer);
        }
    }

    public void Unsubscribe(IObserver<eTouchEvent> observer)
    {
        if (_observers != null)
        {
            _observers.Remove(observer);
        }
    }

    protected override string CreatedObjectName()
    {
        return "TouchManager";
    }

}
