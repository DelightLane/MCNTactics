using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FZ;

public class MapManager : FZ.MonoSingletone<MapManager> {
    [SerializeField]
    private Vector2 _mapSize;

    [SerializeField]
    private List<PlaceInfo> _objInfos;

#if UNITY_EDITOR
    private class DebugMapManager
    {
        private Vector2 _savedMapSize;

        public void CreateTilemap(MapManager manager)
        {
            if (_savedMapSize != manager._mapSize)
            {
                manager.CreateTilemap();

                manager.PlaceObjs(eObjType.UNIT);

                _savedMapSize = manager._mapSize;
            }
        }
    }

    private DebugMapManager _debug;
#endif

    private MapCreator _mapCreator;

    private Dictionary<Vector2, Tile> _tilemap;

    protected override string CreatedObjectName()
    {
        return "MapManager";
    }

    void Awake()
    {
        // TODO : 이 곳은 임시 위치. 추후 로딩 씬에서 부를 수 있게 수정.
        DataManager.Instance.LoadDatas();

        if (_mapCreator == null)
        {
            _mapCreator = new MapCreator();
        }

        LoadMapData();

#if UNITY_EDITOR
        _debug = new DebugMapManager();
        _debug.CreateTilemap(this);
#else
        CreateTilemap();

        PlaceObjs(eObjType.UNIT);
#endif
    }

    void Update()
    {
#if UNITY_EDITOR
        _debug.CreateTilemap(this);
#endif
    }

    private void LoadMapData()
    {
        _objInfos = new List<PlaceInfo>();

        var mapData = DataManager.Instance.GetData<MapData>(DataManager.DataType.MAP);

        _mapSize.x = mapData.x;
        _mapSize.y = mapData.y;
        
        foreach(var obj in mapData.objects)
        {
            _objInfos.Add(new PlaceInfo(obj));
        }
    }

    public void CreateTilemap()
    {
        if (_mapCreator != null)
        {
            _mapCreator.CreateTilemap(_mapSize, ref _tilemap);
        }
    }

    public void RemoveTilemap()
    {
        if (_mapCreator != null)
        {
            _mapCreator.RemoveTilemap(ref _tilemap);
        }
    }

    public bool IsInMapSize(Vector2 pos)
    {
        return _mapSize.x > pos.x && _mapSize.y > pos.y;
    }

    public void PlaceObjs(eObjType type)
    {
        var unitList = _objInfos.FindAll(obj => obj.type == type);

        foreach (var objInfo in unitList)
        {
            if (IsInMapSize(objInfo.pos))
            {
                var unitObj = UnitObject.Create(objInfo.no, objInfo.team);
                if (unitObj != null)
                {
                    this.AttachObject(objInfo.pos, unitObj);
                }
            }
        }
    }

    public bool IsExistMap()
    {
        return _tilemap == null ? false : true;
    }

    public Tile GetTile(Vector2 position)
    {
        if (IsExistMap())
        {
            if (IsInMapSize(position))
            {
                return _tilemap[position];
            }

            return null;
        }
        else
        {
            throw new UnityException("Tilemap is not exist.");
        }
    }

    public void AttachObject(Vector2 pos, PlaceObject obj)
    {
        var tile = GetTile(pos);

        if (tile != null)
        {
            tile.AttachObject(obj);
        }
    }

    public void ChangeAllTileState(eTileType state)
    {
        foreach(var tile in _tilemap)
        {
            tile.Value.ChangeState(state);
        }
    }
}
