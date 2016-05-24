using System;
using UnityEngine;
using System.Linq;

public enum eCombatState
{
    ALIVE,
    DEAD
}

public enum eCombatTeam
{
    WHITE,
    BLUE,
    RED
}

public class UnitObject : ActionObject, ICombat
{
    public eCombatTeam Team { get; set; }

    private ICombat _impl;

    public int No { get; private set; }

    public string UnitName { get; private set; }

    public int Hp { get { return _impl.Hp; } }
    public int Sp { get { return _impl.Sp; } }
    
    public int Atk { get { return _impl.Atk; } }
    public int Def { get { return _impl.Def; } }

    public int MaxHp { get { return _impl.MaxHp; } }
    public int MaxSp { get { return _impl.MaxSp; } }

    public eCombatState CombatState { get { return _impl.CombatState; } }

    // 유닛 데이터 디버깅용
#if UNITY_EDITOR
    public UnitData _debugStatus;
#endif

    public static UnitObject Create(string unitName, eCombatTeam team)
    {
        var unitData = DataManager.Instance.GetData<UnitDataList>(DataManager.DataType.UNIT);

        foreach (var unit in unitData.unit)
        {
            if (unit.name == unitName)
            {
                return Create(unit, team);
            }
        }

        return null;
    }

    public static UnitObject Create(int unitNo, eCombatTeam team)
    {
        var unitData = DataManager.Instance.GetData<UnitDataList>(DataManager.DataType.UNIT);

        var unit = unitData.unit.Find(u => u.no == unitNo);

        if (unit != null)
        {
            return Create(unit, team);
        }

        return null;
    }

    private static UnitObject Create(UnitData unit, eCombatTeam team)
    {
        var targetObj = GameObject.Instantiate(Resources.Load(string.Format("Prefabs/{0}", unit.prefabName), typeof(GameObject))) as GameObject;
        if (targetObj != null)
        {
            try
            {
                var unitObj = targetObj.AddComponent<UnitObject>();

                unitObj.Initialize(unit);

                unitObj.Team = team;

                return unitObj;
            }
            catch (Exception e)
            {
                throw new UnityException(e.Message);
            }
        }

        return null;
    }

    public override void Initialize(DataObject data)
    {
        base.Initialize(data);

        UnitData unitData = data as UnitData;

        if (unitData != null)
        {
            AddActor(unitData);

            this.No = unitData.no;
            this.UnitName = unitData.name;

            _impl = new CombatInstance(unitData);
        }

        DisplayDebugStatus();
    }

    private void AddActor(UnitData data)
    {
        var unitActorDataList = DataManager.Instance.GetData<UnitActorDataList>(DataManager.DataType.ATTACH_ACTOR);

        if (unitActorDataList != null)
        {
            var actorList = unitActorDataList.GetActorList(data.no);
            foreach (var info in actorList)
            {
                this.AddActor(info);
            }
        }
    }

    private void DisplayDebugStatus()
    {
        // 유닛 데이터 디버깅용
#if UNITY_EDITOR
        _debugStatus = new UnitData();

        _debugStatus.no = No;
        _debugStatus.name = UnitName;

        _debugStatus.Hp = _impl.Hp;
        _debugStatus.Sp = _impl.Sp;
        _debugStatus.Atk = _impl.Atk;
        _debugStatus.Def = _impl.Def;
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