using UnityEngine;
using System;
using System.Collections.Generic;

public enum eMoveActType
{
    NORMAL,
    MOVE,
    DONE
}

public class MoveActor : MCN.ActObjActor
{
    #region weight
    public int Range
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
    private MCN.StateMachine<MoveActState> _stateMachine = new MCN.StateMachine<MoveActState>();

    private abstract class MoveActState : MCN.State<MoveActor>
    {
        public MoveActState(MoveActor target) : base(target)
        {
            if (Target != null && Target._stateMachine != null)
            {
                Target._stateMachine.StorageState(GetCurrentType().ToString(), this);
            }
        }

        public virtual void Interactive(Tile activeTile) { }

        public abstract eMoveActType GetCurrentType();

        public abstract bool OnTouchEvent();

        protected void AllTileToNormal()
        {
            if (Target != null)
            {
                Target.ActTarget.Deselect();

                MapManager.Instance.ChangeAllTileState(eTileType.NORMAL);
            }
        }
    }

    private class MoveActState_Normal : MoveActState
    {
        public MoveActState_Normal(MoveActor target) : base(target) { }

        public override eMoveActType GetCurrentType()
        {
            return eMoveActType.NORMAL;
        }

        public override bool OnTouchEvent()
        {
            if (Target != null)
            {
                if (Target.GetCurrentStateType() != eMoveActType.DONE)
                {
                    if (GameManager.Instance.SelectedObj == null)
                    {
                        Target.ChangeState(eMoveActType.MOVE);
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

    private class MoveActState_Move : MoveActState
    {
        public MoveActState_Move(MoveActor target) : base(target) { }

        public override eMoveActType GetCurrentType()
        {
            return eMoveActType.MOVE;
        }

        public override bool OnTouchEvent()
        {
            if (Target != null)
            {
                if (Target.ActTarget.IsSelected())
                {
                    Target.ChangeState(eMoveActType.NORMAL);
                }
            }

            return false;
        }

        public override void Run()
        {
            if (Target != null)
            {
                Target.ActTarget.Select();

                if (Target.ActTarget.GetPlacedTile() != null)
                {
                    MapManager.Instance.ChangeAllTileState(eTileType.DEACTIVE);

                    var placedTile = Target.ActTarget.GetPlacedTile();

                    placedTile.ShowChainActiveTile(Target.Range, (Tile tile) => { return tile.GetAttachObject() != null; });
                }
            }
        }

        public override void Interactive(Tile activeTile)
        {
            if (Target != null)
            {
                bool isSuccess = activeTile.AttachObject(Target.ActTarget);

                if (isSuccess)
                {
                    if (Target != null)
                    {
                        Target.ChangeState(eMoveActType.DONE);
                    }
                }
            }
        }
    }

    private class MoveActState_Done : MoveActState
    {
        public MoveActState_Done(MoveActor target) : base(target) { }

        public override eMoveActType GetCurrentType()
        {
            return eMoveActType.DONE;
        }

        public override bool OnTouchEvent()
        {
            return true;
        }

        public override void Run()
        {
            if (Target != null)
            {
                AllTileToNormal();

                Target.FinishActor();
            }
        }
    }
    #endregion

    protected override void Initialize()
    {
        base.Initialize();

        StorageStates();

        ChangeState(eMoveActType.NORMAL);
    }

    private void StorageStates()
    {
        new MoveActState_Normal(this);
        new MoveActState_Move(this);
        new MoveActState_Done(this);
    }

    private MoveActState GetCurrentState()
    {
        var state = _stateMachine.GetCurrentState();
        if (state != null && state is MoveActState)
        {
            return state as MoveActState;
        }

        throw new UnityException("don't have moveAct state.");
    }

    private eMoveActType GetCurrentStateType()
    {
        var state = GetCurrentState();

        if (state != null)
        {
            return state.GetCurrentType();
        }

        throw new UnityException("don't have moveAct state.");
    }

    private void ChangeState(eMoveActType type)
    {
        if (_stateMachine != null)
        {
            _stateMachine.ChangeState(type.ToString());
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
