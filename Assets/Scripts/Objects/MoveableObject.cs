using UnityEngine;
using System;
using System.Collections;

public enum eMoveableType
{
    NORMAL,
    MOVE
}

public class MoveableObject : PlaceableObject, MCN.IObserver<eTouchEvent>
{
    [SerializeField]
    private int moveRange = 0;

    #region state
    private MCN.StateMachine<MoveableState> _moveableStateMachine = new MCN.StateMachine<MoveableState>();

    private abstract class MoveableState : MCN.State
    {
        public MoveableState(TacticsObject target) : base(target)
        {
            var moveable = target as MoveableObject;

            if(moveable != null && moveable._moveableStateMachine != null)
            {
                moveable._moveableStateMachine.StorageState(GetCurrentType().ToString(), this);
            }
        }

        public abstract eMoveableType GetCurrentType();

        public abstract void OnTouchEvent();
    }

    private class MoveableState_Move : MoveableState
    {
        public MoveableState_Move(TacticsObject target) : base(target) { }

        public override eMoveableType GetCurrentType()
        {
            return eMoveableType.MOVE;
        }

        public override void OnTouchEvent()
        {
            var moveable = Target as MoveableObject;

            if (moveable != null)
            {
                moveable.ChangeMoveableState(eMoveableType.NORMAL);
            }
        }

        public override void Run()
        {
            var moveable = Target as MoveableObject;

            if (moveable != null)
            {
                if (moveable._attachedTile != null)
                {
                    MapManager.Instance.ChangeAllTileState(eTileType.DEACTIVE);

                    moveable._attachedTile.ShowChainActiveTile(moveable.moveRange);
                }
            }
        }
    }

    private class MoveableState_Normal : MoveableState
    {
        public MoveableState_Normal(TacticsObject target) : base(target) { }

        public override eMoveableType GetCurrentType()
        {
            return eMoveableType.NORMAL;
        }

        public override void OnTouchEvent()
        {
            var moveable = Target as MoveableObject;

            if (moveable != null)
            {
                moveable.ChangeMoveableState(eMoveableType.MOVE);
            }
        }

        public override void Run()
        {
            var moveable = Target as MoveableObject;

            if (moveable != null)
            {
                MapManager.Instance.ChangeAllTileState(eTileType.NORMAL);
            }
        }
    }
    #endregion

    void Awake()
    {
        TouchManager.Instance.Subscribe(this);

        StorageStates();

        ChangeMoveableState(eMoveableType.NORMAL);
    }

    void Destroy()
    {
        TouchManager.Instance.Unsubscribe(this);
    }

    private void StorageStates()
    {
        new MoveableState_Normal(this);
        new MoveableState_Move(this);
    }

    public void OnNext(eTouchEvent touchEvent)
    {
        if (touchEvent == eTouchEvent.TOUCH)
        {
            var state = GetCurrentMoveableState();

            if (state != null)
            {
                state.OnTouchEvent();
            }
        }
    }

    private MoveableState GetCurrentMoveableState()
    {
        var state = _moveableStateMachine.GetCurrentState();
        if(state != null && state is MoveableState)
        {
            return state as MoveableState;
        }

        throw new UnityException("don't have moveable state.");
    }

    private eMoveableType GetMoveableCurrentStateType()
    {
        var state = GetCurrentMoveableState();

        if(state != null)
        {
            return state.GetCurrentType();
        }

        throw new UnityException("don't have moveable state.");
    }

    private void ChangeMoveableState(eMoveableType type)
    {
        if(_moveableStateMachine != null)
        {
            _moveableStateMachine.ChangeState(type.ToString());
        }
    }
}
