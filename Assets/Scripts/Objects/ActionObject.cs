using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using FZ;

public class ActionObject : PlaceObject, FZ.IActorQueue
{
    private ActorMachine _actorMachine = new ActorMachine();

    // Actor의 큐를 디버깅하기 위해 Inspector에 노출시키기 위한 리스트
#if UNITY_EDITOR
    [SerializeField]
    private List<string> _actorDebugQueue = new List<string>();
#endif

    public override void Initialize(DataObject data)
    {
        base.Initialize(data);
    }

    public void AddActor(Actor actor)
    {
        _actorMachine.AddActor(actor);
    }

    public void AddActor(ActorInfo info)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        try
        {
            Type actorType = assembly.GetType(info.name);

            FZ.Actor actor = (FZ.Actor)Activator.CreateInstance(actorType);

            if (actor != null)
            {
                actor.Initialize(this, info.weightName, info.weightValue);
            }

            this.AddActor(actor);
        }
        catch (Exception e)
        {
            throw new UnityException(e.ToString());
        }
    }

    // 액터를 사용하기 전에 체크할 조건들을 위한 메소드
    protected virtual bool DoPreReserveActor(FZ.Actor checkedActor)
    {
        return true;
    }

    protected virtual bool DoPreStartActor(FZ.Actor checkedActor)
    {
        return true;
    }

    protected virtual bool DoPreEndActor(FZ.Actor checkedActor)
    {
        return true;
    }

    protected virtual bool DoPreCancelActor(FZ.Actor checkedActor)
    {
        return true;
    }

    public void ReserveActor(Type actorType)
    {
        if (DoPreReserveActor(_actorMachine.GetUsableActor(actorType)))
        {
            _actorMachine.ReserveActor(actorType);
            Debug_EnqueueActor(actorType.ToString());
        }
    }

    public void StartActor()
    {
        var headActor = _actorMachine.GetHeadActor();

        if (headActor != null)
        {
            if (DoPreStartActor(headActor))
            {
                _actorMachine.StartActor();
            }
        }
    }

    public void EndActor()
    {
        if (DoPreEndActor(_actorMachine.GetHeadActor()))
        {
            _actorMachine.EndActor();
            Debug_DequeueActor();
        }

        StartActor();
    }

    public void CancelActor()
    {
        if (DoPreCancelActor(_actorMachine.GetHeadActor()))
        {
            _actorMachine.CancelActor();
            Debug_DequeueActor();
        }
    }

    public bool HasActor()
    {
        return _actorMachine.GetHeadActor() != null;
    }

    protected bool IsLastActor()
    {
        return _actorMachine.IsLastActor();
    }

    private void Debug_EnqueueActor(string actorType)
    {
#if UNITY_EDITOR
        _actorDebugQueue.Add(actorType.ToString());
#endif
    }
    
    private void Debug_DequeueActor()
    {
#if UNITY_EDITOR
        if (_actorDebugQueue.Count > 0)
        {
            _actorDebugQueue.RemoveAt(0);
        }
#endif
    }

    public IEnumerable<string> GetQueueActorNames()
    {
        return _actorMachine.GetQueueActorNames();
    }

    public override void Interactive(TacticsObject interactTarget)
    {
        var activeActor = _actorMachine.GetHeadActor();
        if (activeActor != null)
        {
            activeActor.Interactive(interactTarget);
        }
    }

    public override bool OnTouchEvent(eTouchEvent touch)
    {
        var activeActor = _actorMachine.GetHeadActor();
        if (activeActor != null)
        {
            activeActor.OnTouchEvent(touch);
        }

        return true;
    }
}
