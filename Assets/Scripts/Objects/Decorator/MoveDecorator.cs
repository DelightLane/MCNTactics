using UnityEngine;
using System;
using System.Collections.Generic;

public enum eMoveableType
{
    NORMAL,
    MOVE
}

public class MoveDecorator : MCN.Decorator
{
    #region weight
    private int Range
    {
        get
        {
            return GetWeight("range");
        }
    }

    protected override string[] AbsoluteWeightKey()
    {
        return new string[] { "range" };
    }
    #endregion

    #region state
    private MCN.StateMachine<MoveableState> _moveableStateMachine = new MCN.StateMachine<MoveableState>();

    private abstract class MoveableState : MCN.State
    {
        public MoveableState(TacticsObject target) : base(target)
        {
            var moveable = target as MoveDecorator;

            if (moveable != null && moveable._moveableStateMachine != null)
            {
                moveable._moveableStateMachine.StorageState(GetCurrentType().ToString(), this);
            }
        }

        public virtual void Interactive(Tile activeTile) { }

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
            var moveable = Target as MoveDecorator;

            if (moveable != null)
            {
                var placeable = moveable.DecoTarget as PlaceableObject;

                if (placeable != null)
                {
                    if (placeable.IsSelected())
                    {
                        moveable.ChangeState(eMoveableType.NORMAL);
                    }
                }
            }
        }

        public override void Run()
        {
            var moveable = Target as MoveDecorator;

            if (moveable != null)
            {
                var placeable = moveable.DecoTarget as PlaceableObject;

                if (placeable != null)
                {
                    placeable.Select();

                    if (placeable.GetPlacedTile() != null)
                    {
                        MapManager.Instance.ChangeAllTileState(eTileType.DEACTIVE);

                        var placedTile = placeable.GetPlacedTile();

                        placedTile.ShowChainActiveTile(moveable.Range);
                    }
                }
            }
        }

        public override void Interactive(Tile activeTile)
        {
            var moveable = Target as MoveDecorator;

            if (moveable != null)
            {
                var placeable = moveable.DecoTarget as PlaceableObject;

                if (placeable != null)
                {
                    bool isSuccess = activeTile.AttachObject(placeable);

                    if (isSuccess)
                    {
                        if (moveable != null)
                        {
                            moveable.ChangeState(eMoveableType.NORMAL);
                        }
                    }
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
            var moveable = Target as MoveDecorator;

            if (moveable != null)
            {
                if (GameManager.Instance.SelectedObj == null)
                {
                    moveable.ChangeState(eMoveableType.MOVE);
                }
            }
        }

        public override void Run()
        {
            var moveable = Target as MoveDecorator;

            if (moveable != null)
            {
                var placeable = moveable.DecoTarget as PlaceableObject;

                if (placeable != null)
                {
                    placeable.Deselect();

                    MapManager.Instance.ChangeAllTileState(eTileType.NORMAL);
                }
            }
        }
    }
    #endregion

    void Awake()
    {
        StorageStates();

        ChangeState(eMoveableType.NORMAL);
    }

    private void StorageStates()
    {
        new MoveableState_Normal(this);
        new MoveableState_Move(this);
    }

    private MoveableState GetCurrentState()
    {
        var state = _moveableStateMachine.GetCurrentState();
        if (state != null && state is MoveableState)
        {
            return state as MoveableState;
        }

        throw new UnityException("don't have moveable state.");
    }

    private eMoveableType GetCurrentStateType()
    {
        var state = GetCurrentState();

        if (state != null)
        {
            return state.GetCurrentType();
        }

        throw new UnityException("don't have moveable state.");
    }

    private void ChangeState(eMoveableType type)
    {
        if (_moveableStateMachine != null)
        {
            _moveableStateMachine.ChangeState(type.ToString());
        }
    }

    protected override void DecoOnTouchEvent(eTouchEvent touch)
    {
        var state = GetCurrentState();

        if (state != null)
        {
            state.OnTouchEvent();
        }
    }

    protected override void DecoInteractive(TacticsObject interactTarget)
    {
        var tile = interactTarget as Tile;

        if (tile != null)
        {
            var state = GetCurrentState();

            if (state != null)
            {
                state.Interactive(tile);
            }
        }
    }
}
