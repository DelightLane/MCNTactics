using System;
using UnityEngine;

public class CombatObject : PlaceableObject
{
    [SerializeField]
    private int _hp;
    [SerializeField]
    private int _sp;

    [SerializeField]
    private int _atk;
    [SerializeField]
    private int _def;

    public int Hp { get { return _hp; } }
    public int Sp { get { return _sp; } }
    
    public int Atk { get { return _atk; } }
    public int Def { get { return _def; } }

    public void Damaged(AttackActor actor)
    {
        _hp -= actor.Damage;

        if(_hp < 0)
        {
            _hp = 0;
        }
    }
}