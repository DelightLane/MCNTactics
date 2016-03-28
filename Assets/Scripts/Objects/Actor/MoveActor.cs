using UnityEngine;
using System;
using System.Collections.Generic;

public enum eMoveableType
{
    NORMAL,
    MOVE,
    DONE
}

public class MoveActor : MCN.Actor
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

    private abstract class MoveableState : MCN.State<MoveActor>
    {
        public MoveableState(MoveActor target) : base(target)
        {
            if (Target != null && Target._moveableStateMachine != null)
            {
                Target._moveableStateMachine.StorageState(GetCurrentType().ToString(), this);
            }
        }

        public virtual void Interactive(Tile activeTile) { }

        public abstract eMoveableType GetCurrentType();

        public abstract bool OnTouchEvent();

        protected void AllTileToNormal()
        {
            if (Target != null)
            {
                var placeable = Target.ActTarget as PlaceableObject;

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
        public MoveableState_Move(MoveActor target) : base(target) { }

        public override eMoveableType GetCurrentType()
        {
            return eMoveableType.MOVE;
        }

        public override bool OnTouchEvent()
        {
            if (Target != null)
            {
                var placeable = Target.ActTarget as PlaceableObject;

                if (placeable != null)
                {
                    if (placeable.IsSelected())
                    {
                        Target.ChangeState(eMoveableType.NORMAL);
                    }
                }
            }

            return false;
        }

        public override void Run()
        {
            if (Target != null)
            {
                var placeable = Target.ActTarget as PlaceableObject;

                if (placeable != null)
                {
                    placeable.Select();

                    if (placeable.GetPlacedTile() != null)
                    {
                        MapManager.Instance.ChangeAllTileState(eTileType.DEACTIVE);

                        var placedTile = placeable.GetPlacedTile();

                        placedTile.ShowChainActiveTile(Target.Range);
                    }
                }
            }
        }

        public override void Interactive(Tile activeTile)
        {
            if (Target != null)
            {
                var placeable = Target.ActTarget as PlaceableObject;

                if (placeable != null)
                {
                    bool isSuccess = activeTile.AttachObject(placeable);

                    if (isSuccess)
                    {
                        if (Target != null)
                        {
                            Target.ChangeState(eMoveableType.DONE);
                        }
                    }
                }
            }
        }
    }

    private class MoveableState_Normal : MoveableState
    {
        public MoveableState_Normal(MoveActor target) : base(target) { }

        public override eMoveableType GetCurrentType()
        {
            return eMoveableType.NORMAL;
        }

        public override bool OnTouchEvent()
        {
            if (Target != null)
            {
                if (Target.GetCurrentStateType() != eMoveableType.DONE)
                {
                    if (GameManager.Instance.SelectedObj == null)
                    {
                        Target.ChangeState(eMoveableType.MOVE);
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
        public MoveableState_Done(MoveActor target) : base(target) { }

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

            Target.FinishActor();
        }
    }
    #endregion

    protected override void Initialize()
    {
        base.Initialize();

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

        throw new UnityException("don't have moveAct state.");
    }

    private eMoveableType GetCurrentStateType()
    {
        var state = GetCurrentState();

        if (state != null)
        {
            return state.GetCurrentType();
        }

        throw new UnityException("don't have moveAct state.");
    }

    private void ChangeState(eMoveableType type)
    {
        if (_moveableStateMachine != null)
        {
            _moveableStateMachine.ChangeState(type.ToString());
        }
    }

    public override bool OnTouchEvent(eTouchEvent touch)
    {
        base.OnTouchEvent(touch);

        var state = GetCurrentState();

        if (state != null)
        {
            return state.OnTouchEvent();
        }

        throw new UnityException("don't have moveAct state.");
    }

    public override void Interactive(TacticsObject interactTarget)
    {
        base.Interactive(interactTarget);

        var tile = interactTarget as Tile;

        if (tile != null)
        {
            var state = GetCurrentState();

            if (state != null)
            {
                state.Interactive(tile);

                return;
            }
        }

        throw new UnityException("don't have moveAct state.");
    }
}
