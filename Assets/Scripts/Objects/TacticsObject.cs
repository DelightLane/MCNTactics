using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 본 게임 내부의 모든 오브젝트 컨트롤 클래스들의 최상위 부모 클래스
public abstract class TacticsObject : MonoBehaviour
{
    // Actor의 큐. 만약 큐에 Actor가 있다면 그 Actor는 사용될 준비가 된 것이다.
    private LinkedList<MCN.Actor> _actorQueue = new LinkedList<MCN.Actor>();

    // Actor의 큐를 디버깅하기 위해 Inspector에 노출시키기 위한 리스트
#if UNITY_EDITOR
    [SerializeField]
    private List<string> _actorDebugQueue = new List<string>();
#endif

    // 해당 TacticsObject가 행동을 취할 수 있는 Actor들
    private Dictionary<string, MCN.Actor> _actors = new Dictionary<string, MCN.Actor>();

    public void AddActor(MCN.Actor actor)
    {
        if (actor.CheckAbsoluteWeightKey())
        {
            _actors.Add(actor.GetType().ToString(), actor);
        }
    }

    public void RunActor(System.Type actorType)
    {
        if(!actorType.IsSubclassOf(typeof(MCN.Actor)) ||
           !_actors.ContainsKey(actorType.ToString()))
        {
            throw new UnityException("Actor's type is not correct.");
        }

        _actorQueue.AddLast(_actors[actorType.ToString()]);
#if UNITY_EDITOR
        _actorDebugQueue.Add(actorType.ToString());
#endif
    }

    public void FinishActor()
    {
        if (_actorQueue.Count > 0)
        {
            _actorQueue.RemoveFirst();
#if UNITY_EDITOR
            _actorDebugQueue.RemoveAt(0);
#endif
        }
    }

    public virtual void Interactive(TacticsObject interactTarget)
    {
        if(_actorQueue.Count > 0)
        {
            var actor = _actorQueue.First.Value;

            actor.Interactive(interactTarget);
        }
    }

    public virtual bool OnTouchEvent(eTouchEvent touch)
    {
        if (_actorQueue.Count > 0)
        {
            var actor = _actorQueue.First.Value;

            actor.OnTouchEvent(touch);
        }

        return true;
    }
}
