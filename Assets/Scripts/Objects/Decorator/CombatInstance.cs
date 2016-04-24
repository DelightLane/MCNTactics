using System;
using UnityEngine;

public class CombatInstance : ICombat
{
    private int _hp;
    private int _sp;

    private int _maxHp;
    private int _maxSp;

    private int _atk;
    private int _def;

    private eCombatState _combatState;
    public eCombatState CombatState { get { return _combatState; } }

    public int Hp
    {
        get
        {
            if (CombatState == eCombatState.ALIVE)
            {
                return _hp;
            }

            return 0;
        }
    }
    public int Sp { get { return _sp; } }

    public int Atk { get { return _atk; } }
    public int Def { get { return _def; } }

    public int MaxHp { get { return _maxHp; } }
    public int MaxSp { get { return _maxSp; } }

    public CombatInstance(Status status)
    {
        _combatState = eCombatState.ALIVE;

        _hp = status.hp;
        _sp = status.sp;

        _maxHp = status.hp;
        _maxSp = status.sp;

        _atk = status.atk;
        _def = status.def;
    }

    public void Damaged(AttackActor actor)
    {
        _hp -= actor.Damage;

        if(Hp < 0)
        {
            _combatState = eCombatState.DEAD;
        }
    }
}