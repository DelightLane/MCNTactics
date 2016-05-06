using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MCN;

public class PlaceableObject : TacticsObject, IDisposable, MCN.IActorQueue
{
    private ActorMachine _actorMachine = new ActorMachine();

    // Actor의 큐를 디버깅하기 위해 Inspector에 노출시키기 위한 리스트
#if UNITY_EDITOR
    [SerializeField]
    private List<string> _actorDebugQueue = new List<string>();
#endif

    protected Tile _placedTile;

    // 타일에 놓는 건 타일의 AttachObject 메소드를 사용할 것.
    // 이걸 사용할 수 없게 막을 방법은 없을까?
    public void Attach(Tile tile)
    {
        // 순환 참조 적용. 레퍼런스 관리에 신경 쓸 것
        _placedTile = tile;

        Place(tile);
    }

    // 타일에 떼는 건 타일의 DetachObject 메소드를 사용할 것.
    // 이걸 사용할 수 없게 막을 방법은 없을까?
    public void Detach()
    {
        _placedTile = null;
    }

    public Tile GetPlacedTile()
    {
        return _placedTile;
    }

    public void Dispose()
    {
        _placedTile = null;

        GameObject.Destroy(gameObject);
    }

    private void Place(Tile tile)
    {
        if (tile != null)
        {
            transform.parent = tile.transform;
            transform.localPosition = new Vector3(0, transform.localScale.y / 2, 0);

            tile.AttachObject(this);
        }
    }

    public bool IsSelected()
    {
        return GameManager.Instance.SelectedObj == this;
    }

    public void Select()
    {
        GameManager.Instance.SelectedObj = this;
    }

    public void Deselect()
    {
        GameManager.Instance.SelectedObj = null;
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

            MCN.Actor actor = (MCN.Actor)Activator.CreateInstance(actorType);

            if (actor != null)
            {
                actor.Initialize(this, info.weightName, info.weightValue);
            }

            this.AddActor(actor);

#if UNITY_EDITOR
            // 이건 디버깅용으로.. 무조껀 큐에 삽입한다.
            this.EnqueueActor(actor.GetType());
#endif
        }
        catch (Exception e)
        {
            throw new UnityException(e.ToString());
        }
    }

    public void EnqueueActor(Type actorType)
    {
        _actorMachine.EnqueueActor(actorType);

#if UNITY_EDITOR
        _actorDebugQueue.Add(actorType.ToString());
#endif
    }

    public void DequeueActor()
    {
        _actorMachine.DequeueActor();

#if UNITY_EDITOR
        _actorDebugQueue.RemoveAt(0);
#endif
    }


    public override void Interactive(TacticsObject interactTarget)
    {
        var activeActor = _actorMachine.GetActiveActor();
        if (activeActor != null)
        {
            activeActor.Interactive(interactTarget);
        }
    }

    public override bool OnTouchEvent(eTouchEvent touch)
    {
        var activeActor = _actorMachine.GetActiveActor();
        if (activeActor != null)
        {
            activeActor.OnTouchEvent(touch);
        }

        return true;
    }
}
