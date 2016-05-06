using System;

public class CombatAddedDeco : MCN.Decorator<ICombat>, ICombat
{
    private int _addedAtk;
    private int _addedDef;
    private int _addedHp;
    private int _addedSp;

    public CombatAddedDeco(ICombat target, Status addedStatus) : base(target)
    {
        this._addedAtk = addedStatus.Atk;
        this._addedDef = addedStatus.Def;
        this._addedHp = addedStatus.Hp;
        this._addedSp = addedStatus.Sp;
    }

    public int Atk { get { return _decoTarget.Atk + _addedAtk; } }

    public int Def {  get { return _decoTarget.Def + _addedDef; } }

    public int Hp {  get { return _decoTarget.Hp + _addedHp; } }

    public int Sp { get { return _decoTarget.Sp + _addedSp; } }

    public int MaxHp {  get { return _decoTarget.MaxHp + _addedHp; } }

    public int MaxSp { get { return _decoTarget.MaxSp + _addedSp; } }

    public eCombatState CombatState { get { return _decoTarget.CombatState; } }

    public void Damaged(AttackActor actor)
    {
        _decoTarget.Damaged(actor);
    }
}
