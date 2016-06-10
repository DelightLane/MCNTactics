using System;
using UnityEngine;

[Serializable]
public struct Status
{
    public int Hp;
    public int Sp;
    public int Atk;
    public int Def;
    public int ActRange;
}

public class CombatInstance : ICombat
{
    private int _hp;
    private int _sp;

    private int _maxHp;
    private int _maxSp;

    private int _atk;
    private int _def;

    private int _actRange;

    private eCombatState _combatState;
    
    public int Hp
    {
        get
        {
            if (_combatState == eCombatState.ALIVE)
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

    public int ActRange { get { return _actRange; } }

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
        status.ActRange = unitData.ActRange;

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

        _actRange = status.ActRange;
    }

    public void Damaged(AttackActor actor, ICombatCallback callback)
    {
        _hp -= actor.Damage;

        if(Hp < 0)
        {
            _combatState = eCombatState.DEAD;
        }

        if (callback != null)
        {
            callback(_combatState);
        }
    }
}