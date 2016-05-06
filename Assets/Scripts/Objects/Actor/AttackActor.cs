using UnityEngine;
using System.Collections;
using System;

public enum eAttackActType
{
    NORMAL,
    ATTACK,
    DONE
}

public class AttackActor : MCN.UnitObjActor
{
    #region weight
    public int Range
    {
        get
        {
            return GetWeight("range");
        }
    }

    public int Damage
    {
        get
        {
            int atk = ActTarget.Atk;
            
            return atk + GetWeight("damage");
        }
    }

    protected override string[] AbsoluteWeightKey()
    {
        return new string[] { "range", "damage" };
    }
    #endregion   

    #region state
    private MCN.StateMachine<AttackActState> _stateMachine = new MCN.StateMachine<AttackActState>();

    private abstract class AttackActState : MCN.State<AttackActor>
    {
        public AttackActState(AttackActor target) : base(target)
        {
            if (Target != null && Target._stateMachine != null)
            {
                Target._stateMachine.StorageState(GetCurrentType().ToString(), this);
            }
        }

        public virtual void Interactive(Tile activeTile) { }

        public abstract eAttackActType GetCurrentType();

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

    private class AttackActState_Normal : AttackActState
    {
        public AttackActState_Normal(AttackActor target) : base(target) { }

        public override eAttackActType GetCurrentType()
        {
            return eAttackActType.NORMAL;
        }

        public override bool OnTouchEvent()
        {
            if (Target != null)
            {
                if (Target.GetCurrentStateType() != eAttackActType.DONE)
                {
                    if (GameManager.Instance.SelectedObj == null)
                    {
                        Target.ChangeState(eAttackActType.ATTACK);
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

    private class AttackActState_Attack : AttackActState
    {
        public AttackActState_Attack(AttackActor target) : base(target) { }

        public override eAttackActType GetCurrentType()
        {
            return eAttackActType.ATTACK;
        }

        public override bool OnTouchEvent()
        {
            if (Target != null)
            {
                if (Target.ActTarget.IsSelected())
                {
                    Target.ChangeState(eAttackActType.NORMAL);
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

                    Func<Tile, bool> tileDeactiveCond = (Tile tile) =>
                    {
                        return (tile.GetAttachObject() != null && !(tile.GetAttachObject() is UnitObject)) ||
                               (tile.GetAttachObject() is UnitObject && (tile.GetAttachObject() as UnitObject).Team == Target.ActTarget.Team);
                    };

                    placedTile.ShowChainActiveTile(Target.Range, tileDeactiveCond);
                }
            }
        }

        public override void Interactive(Tile activeTile)
        {
            if (Target != null)
            {
                var damagedTarget = activeTile.GetAttachObject() as UnitObject;

                if (damagedTarget != null)
                {
                    damagedTarget.Damaged(Target);

                    Target.ChangeState(eAttackActType.DONE);
                }
            }
        }
    }

    private class AttackActState_Done : AttackActState
    {
        public AttackActState_Done(AttackActor target) : base(target) { }

        public override eAttackActType GetCurrentType()
        {
            return eAttackActType.DONE;
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

        ChangeState(eAttackActType.NORMAL);
    }

    private void StorageStates()
    {
        new AttackActState_Normal(this);
        new AttackActState_Attack(this);
        new AttackActState_Done(this);
    }

    private AttackActState GetCurrentState()
    {
        var state = _stateMachine.GetCurrentState();
        if (state != null && state is AttackActState)
        {
            return state as AttackActState;
        }

        throw new UnityException("don't have attackAct state.");
    }

    private eAttackActType GetCurrentStateType()
    {
        var state = GetCurrentState();

        if (state != null)
        {
            return state.GetCurrentType();
        }

        throw new UnityException("don't have attackAct state.");
    }

    private void ChangeState(eAttackActType type)
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

        throw new UnityException("don't have attackAct state.");
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

        throw new UnityException("don't have attackAct state.");
    }
}
