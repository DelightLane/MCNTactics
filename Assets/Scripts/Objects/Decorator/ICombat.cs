using UnityEngine;
using System;

public delegate void ICombatCallback(eCombatState state);

public interface ICombat
{
    int Hp { get; }
    int Sp { get; }

    int Atk { get; }
    int Def { get; }

    int MaxHp { get; }
    int MaxSp { get; }

    void Damaged(AttackActor actor, ICombatCallback callback);
}