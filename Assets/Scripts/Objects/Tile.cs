﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum eTileDirect
{
    UP,
    DOWN,
    RIGHT,
    LEFT
}

public class Tile : TacticsObject, IDisposable, FZ.IObserver<eTouchEvent>
{
    private Vector2 _position;
    private PlaceObject _attached;

    private string _tileTextureName;

    public static readonly float TILE_SIZE = 1;

    #region Tile State
    private FZ.StateMachine<State> _stateMachine = new FZ.StateMachine<State>();

    public abstract class State : FZ.State<Tile>
    {
        public State(Tile target) : base(target)
        {
            var tile = Target as Tile;
            if (tile != null)
            {
                tile._stateMachine.StorageState(this);
            }
        }

        // 타일에 붙은 오브젝트에 터치 이벤트를 넘기고 싶지 않을 때 false를 리턴한다.
        public virtual bool OnTouchEvent(eTouchEvent touch) { return true; }

        protected void SetTileImage(string imageName)
        {
            if (Target != null)
            {
                var tileDatas = DataManager.Instance.GetData<AtlasDataList>(DataManager.DataType.ATLAS_TILE);

                Target.SetCubeTexture(tileDatas, imageName);
            }
        }

        private string _forceTileImageName = string.Empty;

        protected string TileImageName
        {
            get
            {
                return _forceTileImageName == string.Empty ? GetBaseTileImageName() : _forceTileImageName;
            }
        }

        protected abstract string GetBaseTileImageName();

        public void SetForceTileImageName(string name)
        {
            _forceTileImageName = name;
        }

        public override void Run()
        {
            SetTileImage(TileImageName);
        }
    }

    public class State_Normal : State
    {
        public State_Normal(Tile target) : base(target) { }

        protected override string GetBaseTileImageName()
        {
            return Target._tileTextureName;
        }
    }

    public class State_Active : State
    {
        public State_Active(Tile target) : base(target) { }

        protected override string GetBaseTileImageName()
        {
            // TODO : 맞는 타일 이미지로 변경
            return "active";
        }

        public override bool OnTouchEvent(eTouchEvent touch)
        {
            if (touch == eTouchEvent.TOUCH)
            {
                var selected = GameManager.Instance.SelectedObj;

                if (selected != null)
                {
                    selected.Interactive(Target);

                    return false;
                }
            }

            return true;
        }
    }

    public class State_Deactive : State
    {
        public State_Deactive(Tile target) : base(target) { }

        protected override string GetBaseTileImageName()
        {
            // TODO : 맞는 타일 이미지로 변경
            return "deactive";
        }
    }
    #endregion

    private Tile() { }

    public static Tile CreateTile()
    {
        var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        
        tile.transform.localScale = new Vector3(TILE_SIZE, TILE_SIZE, TILE_SIZE);

        var tileComp = tile.AddComponent<Tile>();
        TouchManager.Instance.Subscribe(tileComp);

        return tileComp;
    }

    public void Dispose()
    {
        if (_attached != null)
        {
            var placeable = _attached.GetComponent<PlaceObject>();

            if (placeable != null)
            {
                placeable.Dispose();
            }

            _attached = null;
        }

        if(_stateMachine != null)
        {
            _stateMachine.Dispose();

            _stateMachine = null;
        }

        GameObject.Destroy(gameObject);
    }

    void Awake()
    {
        StorageStates();
    }

    private void StorageStates()
    {
        new State_Normal(this);
        new State_Active(this);
        new State_Deactive(this);
    }

    public void Initialize(Vector2 pos, string tileTextureName)
    {
        SetName(pos);

        SetPosition(pos);

        _tileTextureName = tileTextureName;

        ChangeState<State_Normal>();
    }

    public void SetName(Vector2 pos)
    {
        this.name = String.Format("{0}_{1}", pos.x, pos.y);
    }

    public void SetPosition(Vector2 pos)
    {
        _position = pos;

        var offset = 0.03f;
        this.transform.position = new Vector3(pos.x * (Tile.TILE_SIZE + offset), 0, -pos.y * (Tile.TILE_SIZE + offset));
        this.transform.parent = MapCreator.GetRoot().transform;
    }

    public PlaceObject GetAttachObject()
    {
        if (_attached != null)
        {
            return _attached;
        }

        return null;
    }

    public bool AttachObject(PlaceObject obj)
    {
        if (_attached == null)
        {
            var prevTile = obj.GetPlacedTile();

            if(prevTile != null)
            {
                prevTile.DetachObject();
            }

            // 순환 참조 적용. 레퍼런스 관리에 신경 쓸 것
            _attached = obj;

            obj.Attach(this);

            return true;
        }

        return false;
    }

    public void DetachObject()
    {
        if(_attached != null)
        {
            _attached.Detach();

            _attached = null;
        }
    }

    public Tile[] GetClosedTiles()
    {
        Tile[] closedTiles = new Tile[4];

        closedTiles[0] = GetClosedTile(eTileDirect.UP);
        closedTiles[1] = GetClosedTile(eTileDirect.DOWN);
        closedTiles[2] = GetClosedTile(eTileDirect.LEFT);
        closedTiles[3] = GetClosedTile(eTileDirect.RIGHT);

        return closedTiles;
    }

    public Tile GetClosedTile(eTileDirect direct)
    {
        Vector2 closedTilePos = this._position;

        switch (direct)
        {
            case eTileDirect.UP:
                closedTilePos += new Vector2(0, 1);
                break;
            case eTileDirect.DOWN:
                closedTilePos += new Vector2(0, -1);
                break;
            case eTileDirect.RIGHT:
                closedTilePos += new Vector2(1, 0);
                break;
            case eTileDirect.LEFT:
                closedTilePos += new Vector2(-1, 0);
                break;
        }

        if (closedTilePos.x >= 0 && closedTilePos.y >= 0)
        {
            if (MapManager.Instance.IsExistMap())
            {
                return MapManager.Instance.GetTile(closedTilePos);
            }
        }

        return null;
    }

    public delegate bool TileActiveIgnoreCond(Tile checkedTile);

    public void ShowChainActiveTile(int range, TileActiveIgnoreCond ignoreCondition)
    {
        if (_attached != null)
        {
            ShowChainActiveTile(range, _attached, ignoreCondition);
        }
    }

    public void ShowChainActiveTile(int range, TacticsObject startedObj, TileActiveIgnoreCond ignoreCondition)
    {
        if (range <= 0)
        {
            return;
        }

        var closedTiles = this.GetClosedTiles();

        int obstacleCost = GetChainActiveTileCost(startedObj);

        foreach (var closedTile in closedTiles)
        {
            if (closedTile != null)
            {
                if (ignoreCondition != null && ignoreCondition(closedTile))
                {
                    continue;
                }

                closedTile.ChangeState<State_Active>();
                closedTile.ShowChainActiveTile(range - 1 - obstacleCost, startedObj, ignoreCondition);
            }
        }
    }

    private int GetChainActiveTileCost(TacticsObject startedObj)
    {
        var closedTiles = this.GetClosedTiles();

        int obstacleCost = 0;

        if (this.GetAttachObject() != startedObj)
        {
            foreach (var closedTile in closedTiles)
            {
                if (closedTile != null)
                {
                    if (closedTile.GetAttachObject() != null && closedTile.GetAttachObject() != startedObj)
                    {
                        ++obstacleCost;
                    }
                }
            }
        }

        return obstacleCost;
    }

    public void ChangeState<T>(string forceTileImageName = "") where T : Tile.State
    {
        if (_stateMachine != null)
        {
            _stateMachine.GetState<T>().SetForceTileImageName(forceTileImageName);
            _stateMachine.ChangeState<T>();
        }
    }

    private State GetCurrentState()
    {
        if(_stateMachine != null)
        {
            var tileState = _stateMachine.GetCurrentState() as State;

            if(tileState != null)
            {
                return tileState;
            }

            throw new UnityException("tileState isn't.");
        }

        throw new UnityException("_stateMachine is null.");
    }
    
    // 모든 터치 이벤트는 타일이 기준.
    // 그러므로 모든 터치 이벤트는 이곳을 기점으로 시작된다.
    public void OnNext(eTouchEvent touch)
    {
        if (GetCurrentState().OnTouchEvent(touch))
        {
            if (GetAttachObject() != null)
            {
                GetAttachObject().OnTouchEvent(touch);
            }
        }
    }
}