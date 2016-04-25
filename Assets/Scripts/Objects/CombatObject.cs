using System;
using UnityEngine;

[Serializable]
public struct Status
{
    public int hp;
    public int sp;
    public int atk;
    public int def;
}

public enum eCombatState
{
    ALIVE,
    DEAD
}

public enum eCombatTeam
{
    BLUE,
    RED
}

public class CombatObject : PlaceableObject, ICombat
{
    public Status _initialStatus;

    public eCombatTeam Team { get; set; }

    private ICombat _impl;

    public int Hp { get { return _impl.Hp; } }
    public int Sp { get { return _impl.Sp; } }
    
    public int Atk { get { return _impl.Atk; } }
    public int Def { get { return _impl.Def; } }

    public int MaxHp { get { return _impl.MaxHp; } }
    public int MaxSp { get { return _impl.MaxSp; } }

    public eCombatState CombatState { get { return _impl.CombatState; } }

    // 데코레이터 디버깅용
#if UNITY_EDITOR
    public Status _debugStatus;
#endif

    void Awake()
    {
        _impl = new CombatInstance(_initialStatus);

        DisplayDebugStatus();
    }

    private void DisplayDebugStatus()
    {
        // 데코레이터 디버깅용
#if UNITY_EDITOR
        _debugStatus.hp = _impl.Hp;
        _debugStatus.sp = _impl.Sp;
        _debugStatus.atk = _impl.Atk;
        _debugStatus.def = _impl.Def;
#endif
    }

    public void Damaged(AttackActor actor)
    {
        _impl.Damaged(actor);

        DisplayDebugStatus();
    }

    public void AddStatus(Status status)
    {
        _impl = new CombatAddedDeco(_impl, status);

        DisplayDebugStatus();
    }
}