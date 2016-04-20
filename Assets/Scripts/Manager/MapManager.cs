using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using MCN;

public class MapManager : MCN.MonoSingletone<MapManager> {
    [SerializeField]
    private Vector2 _mapSize;

    [SerializeField]
    private List<PlaceInfo> _placeObjInfos;

#if UNITY_EDITOR
    private class DebugMapManager
    {
        private Vector2 _savedMapSize;

        public void CreateTilemap(MapManager manager)
        {
            if (_savedMapSize != manager._mapSize)
            {
                manager.CreateTilemap();

                manager.PlaceObjacts();

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
        // TODO : 이 곳은 임시 위치. 모든 Awake보다 먼저 불리는 곳에서 데이터를 불러야 할텐데 방법이 없을까?
        DataManager.Instance.LoadDatas();

        if (_mapCreator == null)
        {
            _mapCreator = new MapCreator();
        }

        if (_placeObjInfos == null)
        {
            _placeObjInfos = new List<PlaceInfo>();
        }

#if UNITY_EDITOR
        _debug = new DebugMapManager();
        _debug.CreateTilemap(this);
#else
        CreateTilemap();

        PlaceObjacts();
#endif
    }

    void Update()
    {
#if UNITY_EDITOR
        _debug.CreateTilemap(this);
#endif
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

    public void PlaceObjacts()
    {
        foreach (var objInfo in _placeObjInfos)
        {
            if (IsInMapSize(objInfo.pos))
            {
                var placeableObj = PlaceableCreator.Create(objInfo.unitName);
                if (placeableObj != null)
                {
                    this.AttachObject(objInfo.pos, placeableObj);
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

    public void AttachObject(Vector2 pos, PlaceableObject obj)
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
