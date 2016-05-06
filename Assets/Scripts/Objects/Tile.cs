using UnityEngine;
using System.Collections;
using System;

public enum eTileDirect
{
    UP,
    DOWN,
    RIGHT,
    LEFT
}

public enum eTileType
{
    NORMAL,
    ACTIVE,
    DEACTIVE
}

public class Tile : TacticsObject, IDisposable, FZ.IObserver<eTouchEvent>
{
    private Vector2 _position;
    private PlaceObject _attached;

    public static readonly float TILE_SIZE = 1;

    #region Tile State
    private FZ.StateMachine<TileState> _stateMachine = new FZ.StateMachine<TileState>();

    private abstract class TileState : FZ.State<Tile>
    {
        public TileState(Tile target) : base(target)
        {
            var tile = Target as Tile;
            if (tile != null)
            {
                tile._stateMachine.StorageState(GetCurrentType().ToString(), this);
            }
        }

        public abstract eTileType GetCurrentType();

        // 타일에 붙은 오브젝트에 터치 이벤트를 넘기고 싶지 않을 때 false를 리턴한다.
        public virtual bool OnTouchEvent(eTouchEvent touch) { return true; }
    }

    private class TileState_Normal : TileState
    {
        public TileState_Normal(Tile target) : base(target) { }

        public override void Run()
        {
            SetTileColor();
        }

        public override eTileType GetCurrentType()
        {
            return eTileType.NORMAL;
        }

        private void SetTileColor()
        {
            if (Target != null)
            {
                var renderer = Target.transform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.white;
                }
            }
        }
    }

    private class TileState_Active : TileState
    {
        public TileState_Active(Tile target) : base(target) { }

        public override void Run()
        {
            SetTileColor();
        }

        public override eTileType GetCurrentType()
        {
            return eTileType.ACTIVE;
        }

        private void SetTileColor()
        {
            if (Target != null)
            {
                var renderer = Target.transform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.red;
                }
            }
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

    private class TileState_Deactive : TileState
    {
        public TileState_Deactive(Tile target) : base(target) { }

        public override void Run()
        {
            SetTileColor();
        }

        public override eTileType GetCurrentType()
        {
            return eTileType.DEACTIVE;
        }

        private void SetTileColor()
        {
            if (Target != null)
            {
                var renderer = Target.transform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.black;
                }
            }
        }
    }
    #endregion

    private Tile() { }

    public static GameObject CreateTile(Vector2 pos)
    {
        var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
        tile.name = String.Format("{0}_{1}", pos.x, pos.y);

        var material = Resources.Load("Material/Notthing", typeof(Material)) as Material;

        var renderer = tile.GetComponent<Renderer>();
        renderer.material = material;

        tile.transform.localScale = new Vector3(TILE_SIZE, 0.05f, TILE_SIZE);

        var tileComp = tile.AddComponent<Tile>();
        TouchManager.Instance.Subscribe(tileComp);

        return tile;
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

        ChangeState(eTileType.NORMAL);
    }

    private void StorageStates()
    {
        new TileState_Normal(this);
        new TileState_Active(this);
        new TileState_Deactive(this);
    }

    public void SetPosition(Vector2 pos)
    {
        _position = pos;
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

    public void ShowChainActiveTile(int range, Func<Tile, bool> ignoreCondition)
    {
        if (_attached != null)
        {
            ShowChainActiveTile(range, _attached, ignoreCondition);
        }
    }

    public void ShowChainActiveTile(int range, TacticsObject startedObj, Func<Tile, bool> ignoreCondition)
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

                closedTile.ChangeState(eTileType.ACTIVE);
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

    public void ChangeState(eTileType interaction)
    {
        if (_stateMachine != null)
        {
            _stateMachine.ChangeState(interaction.ToString());
        }
    }

    private TileState GetCurrentState()
    {
        if(_stateMachine != null)
        {
            var tileState = _stateMachine.GetCurrentState() as TileState;

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