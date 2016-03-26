using UnityEngine;
using System;
using System.Collections.Generic;

public enum eAttackableType
{
    ATTACK,
    NORMAL,
    DONE,
}

public class AttackDecorator : MCN.Decorator
{
    #region weight
    private int Range
    {
        get
        {
            return GetWeight("range");
        }
    }

    private int Power
    {
        get
        {
            return GetWeight("power");
        }
    }

    protected override string[] AbsoluteWeightKey()
    {
        return new string[] { "range", "power" };
    }
    #endregion

    #region state
    private MCN.StateMachine<AttackableState> _attackableStateMachine = new MCN.StateMachine<AttackableState>();

    private abstract class AttackableState : MCN.State
    {
        public AttackableState(TacticsObject target) : base(target)
        {
            var attackable = target as AttackDecorator;

            if (attackable != null && attackable._attackableStateMachine != null)
            {
                attackable._attackableStateMachine.StorageState(GetCurrentType().ToString(), this);
            }
        }

        public virtual void Interactive(PlaceableObject target) { }

        public abstract eAttackableType GetCurrentType();

        public abstract void OnTouchEvent();

        protected void AllTileToNormal()
        {
            var attackable = Target as AttackDecorator;

            if (attackable != null)
            {
                var placeable = attackable.DecoTarget as PlaceableObject;

                if (placeable != null)
                {
                    placeable.Deselect();

                    MapManager.Instance.ChangeAllTileState(eTileType.NORMAL);
                }
            }
        }

        // TODO : state들을 만든다.
    }
    #endregion

    void Awake()
    {
        StorageStates();
    }

    private void StorageStates()
    {
        // TODO : 스테이트들을 적재한다.
    }

    private AttackableState GetCurrentState()
    {
        var state = _attackableStateMachine.GetCurrentState();
        if (state != null && state is AttackableState)
        {
            return state as AttackableState;
        }

        throw new UnityException("don't have attackable state.");
    }

    private eAttackableType GetCurrentStateType()
    {
        var state = GetCurrentState();

        if (state != null)
        {
            return state.GetCurrentType();
        }

        throw new UnityException("don't have attackable state.");
    }

    private void ChangeState(eAttackableType type)
    {
        if (_attackableStateMachine != null)
        {
            _attackableStateMachine.ChangeState(type.ToString());
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
        var target = interactTarget as PlaceableObject;

        if (target != null)
        {
            var state = GetCurrentState();

            if (state != null)
            {
                state.Interactive(target);
            }
        }
    }
}
