using UnityEngine;
using System;

public interface ICombat
{
    int Hp { get; }
    int Sp { get; }

    int Atk { get; }
    int Def { get; }

    int MaxHp { get; }
    int MaxSp { get; }

    eCombatState CombatState { get; }

    void Damaged(AttackActor actor);
}