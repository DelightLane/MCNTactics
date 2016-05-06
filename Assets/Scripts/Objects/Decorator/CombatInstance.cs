using System;
using UnityEngine;

[Serializable]
public struct Status
{
    public int Hp;
    public int Sp;
    public int Atk;
    public int Def;
}

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
        Initialize(status);
    }

    public CombatInstance(UnitData unitData)
    {
        Status status;
        status.Hp = unitData.Hp;
        status.Sp = unitData.Sp;
        status.Atk = unitData.Atk;
        status.Def = unitData.Def;

        Initialize(status);
    }

    private void Initialize(Status status)
    {
        _combatState = eCombatState.ALIVE;

        _hp = status.Hp;
        _sp = status.Sp;

        _maxHp = status.Hp;
        _maxSp = status.Sp;

        _atk = status.Atk;
        _def = status.Def;
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