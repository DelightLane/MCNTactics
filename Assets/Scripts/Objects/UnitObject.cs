using System;
using UnityEngine;
using System.Linq;
using FZ;

public enum eCombatState
{
    ALIVE,
    DEAD
}

public enum eCombatTeam
{
    UNSELECT,
    WHITE,
    BLUE,
    RED
}

public class UnitObject : ActionObject, ICombat, FZ.IObserver<eCombatTeam>
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

    public int ActRange {  get { return _impl.ActRange; } }

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

                unitObj.ShapeNormal();

                GameManager.Get<GameManager.Turn>().Subscribe(unitObj);

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

        Debug_DisplayStatus();
    }

    protected override void ShapeNormal()
    {
        // TODO : 정상적인 선택 취소 표시를 적용해줄 것
        Color matColor = Color.white;

        if(Team == eCombatTeam.RED)
        {
            matColor = Color.red;
        }
        else if(Team == eCombatTeam.BLUE)
        {
            matColor = Color.blue;
        }
        else if(Team == eCombatTeam.WHITE)
        {
            matColor = Color.white;
        }

        this.GetComponent<Renderer>().material.color = matColor;
    }

    private void ShapeDead()
    {
        // TODO : 유닛이 죽을 때 어떤 형식을 취할 것인지? 일단은 바로 삭제.
        GameObject.Destroy(this.gameObject);
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

    private void Debug_DisplayStatus()
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
        _debugStatus.ActRange = _impl.ActRange;
#endif
    }

    public void Damaged(AttackActor actor)
    {
        Damaged(actor, (eCombatState state) =>
        {
            if(state == eCombatState.DEAD)
            {
                ShapeDead();
            }
        });
    }

    public void Damaged(AttackActor actor, ICombatCallback callback)
    {
        _impl.Damaged(actor, callback);

        Debug_DisplayStatus();
    }

    public void AddStatus(Status status)
    {
        _impl = new CombatAddedDeco(_impl, status);

        Debug_DisplayStatus();
    }

    protected override bool DoPreReserveActor(FZ.Actor checkedActor)
    {
        var checkedUnitActor = checkedActor as IUnitActor;
        if (checkedUnitActor != null)
        {
            bool isSuccess = GameManager.Get<GameManager.Turn>().DoTurn(checkedUnitActor);

            return isSuccess;
        }

        return false;
    }

    protected override bool DoPreEndActor(Actor checkedActor)
    {
        if(IsLastActor())
        {
            GameManager.Get<GameManager.Turn>().EndTurn();
        }

        return true;
    }

    protected override bool DoPreCancelActor(Actor checkedActor)
    {
        var checkedUnitActor = checkedActor as IUnitActor;
        if (checkedUnitActor != null)
        {
            GameManager.Get<GameManager.Turn>().UndoTurn(checkedUnitActor);
        }

        return true;
    }

    public override bool OnTouchEvent(eTouchEvent touch)
    {
        if (!HasActor())
        {
            if (IsSelectedEmpty() &&
                GameManager.Get<GameManager.Turn>().IsCurrentTeam(this.Team))
            {
                Select();

                ActiveActRange();
            }
        }

        return base.OnTouchEvent(touch);
    }

    private Tile _actRangeRoot;

    public void OnNext(eCombatTeam team)
    {
        if(this.Team == team)
        {
            _actRangeRoot = _placedTile;
        }
    }

    private void ActiveActRange()
    {
        if (_actRangeRoot != null)
        {
            var chainInfo = new Tile.ChainInfo((Tile tile) =>
            {
                if (tile.GetAttachObject() != null && tile.GetAttachObject() != this)
                {
                    return true;
                }

                return false;
            });
            // TODO : 실제 이미지 이름으로 변경
            chainInfo.ActiveTileImage = "actRange";
            _actRangeRoot.ActiveChain(ActRange, chainInfo);
        }
    }
}