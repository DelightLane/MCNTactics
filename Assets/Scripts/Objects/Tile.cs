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

public class Tile : TacticsObject
{
    private Vector2 _position;
    private PlaceableObject _attached;

    #region Tile State
    private MCN.StateMachine<TileState> _stateMachine = new MCN.StateMachine<TileState>();

    private abstract class TileState : MCN.State
    {
        public TileState(TacticsObject target) : base(target)
        {
            var tile = Target as Tile;
            if(tile != null)
            {
                tile._stateMachine.StorageState(GetCurrentType().ToString(), this);
            }
        }

        public abstract eTileType GetCurrentType();
    }

    private class TileState_Normal : TileState
    {
        public TileState_Normal(TacticsObject target) : base(target) { }

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
            var tile = Target as Tile;

            if(tile != null)
            {
                var renderer = tile.transform.GetComponent<Renderer>();
                if(renderer != null)
                {
                    renderer.material.color = Color.white;
                }
            }
        }
    }

    private class TileState_Active : TileState
    {
        public TileState_Active(TacticsObject target) : base(target) { }

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
            var tile = Target as Tile;

            if (tile != null)
            {
                var renderer = tile.transform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.red;
                }
            }
        }
    }

    private class TileState_Deactive : TileState
    {
        public TileState_Deactive(TacticsObject target) : base(target) { }

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
            var tile = Target as Tile;

            if (tile != null)
            {
                var renderer = tile.transform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = Color.black;
                }
            }
        }
    }
    #endregion

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

    public TacticsObject GetAttachObject()
    {
        return _attached;
    }

    public void AttachObject(PlaceableObject obj)
    {
        // 순환 참조 적용. 레퍼런스 관리에 신경 쓸 것
        _attached = obj;

        obj.Attach(this);
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

    public void ShowChainActiveTile(int range)
    {
        if(range <= 0)
        {
            return;
        }

        this.ChangeState(eTileType.ACTIVE);
        
        var closedTiles = this.GetClosedTiles();

        foreach (var closedTile in closedTiles)
        {
            if (closedTile != null)
            {
                closedTile.ChangeState(eTileType.ACTIVE);
                closedTile.ShowChainActiveTile(range - 1);
            }
        }
    }

    public void ChangeState(eTileType interaction)
    {
        _stateMachine.ChangeState(interaction.ToString());
    }
}
