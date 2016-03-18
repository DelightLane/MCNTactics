using UnityEngine;
using System;
using System.Collections;

public enum eMoveableType
{
    NORMAL,
    MOVE
}

public class MoveableObject : PlaceableObject
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
        StorageStates();

        ChangeMoveableState(eMoveableType.NORMAL);
    }

    private void StorageStates()
    {
        new MoveableState_Normal(this);
        new MoveableState_Move(this);
    }

    void Update()
    {
        OnTouchEvent(() =>
        {
            var state = GetCurrentMoveableState();

            if (state != null)
            {
                state.OnTouchEvent();
            }
        });
    }

    private void OnTouchEvent(Action callback)
    {
        // TODO : 마우스가 아니라 실제로 터치 이벤트에 대해 동작하게 수정
        if (Input.GetMouseButtonUp(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == transform)
                {
                    callback();
                }
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
