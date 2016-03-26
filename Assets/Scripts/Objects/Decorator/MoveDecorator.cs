using UnityEngine;
using System;
using System.Collections.Generic;

public enum eMoveableType
{
    NORMAL,
    MOVE,
    DONE
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

        public abstract bool OnTouchEvent();

        protected void AllTileToNormal()
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

    private class MoveableState_Move : MoveableState
    {
        public MoveableState_Move(TacticsObject target) : base(target) { }

        public override eMoveableType GetCurrentType()
        {
            return eMoveableType.MOVE;
        }

        public override bool OnTouchEvent()
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

            return false;
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
                            moveable.ChangeState(eMoveableType.DONE);
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

        public override bool OnTouchEvent()
        {
            var moveable = Target as MoveDecorator;

            if (moveable != null)
            {
                if (moveable.GetCurrentStateType() != eMoveableType.DONE)
                {
                    if (GameManager.Instance.SelectedObj == null)
                    {
                        moveable.ChangeState(eMoveableType.MOVE);
                    }
                }
            }

            return false;
        }

        public override void Run()
        {
            AllTileToNormal();
        }
    }

    private class MoveableState_Done : MoveableState
    {
        public MoveableState_Done(TacticsObject target) : base(target) { }

        public override eMoveableType GetCurrentType()
        {
            return eMoveableType.DONE;
        }

        public override bool OnTouchEvent()
        {
            return true;
        }

        public override void Run()
        {
            AllTileToNormal();
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
        new MoveableState_Done(this);
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

    protected override bool DecoOnTouchEvent(eTouchEvent touch)
    {
        var state = GetCurrentState();

        if (state != null)
        {
            return state.OnTouchEvent();
        }

        throw new UnityException("don't have moveable state.");
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
