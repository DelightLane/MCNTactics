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

    private List<string> _tileTextureNames;

    [SerializeField]
    private List<PlaceInfo> _objInfos;

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

        CreateTilemap();

        PlaceAllObjs();

        // TODO : 이 곳은 임시 위치. 추후 로딩 씬에서 부를 수 있게 수정.(유닛이 모두 셋팅되고 시작팀이 선택되어야 함)
        GameManager.Get<GameManager.Turn>().SelectCurrectTurnTeam();
    }

    private void LoadMapData()
    {
        _objInfos = new List<PlaceInfo>();

        var mapData = DataManager.Instance.GetData<MapData>(DataManager.DataType.MAP);

        _mapSize.x = mapData.x;
        _mapSize.y = mapData.y;

        _tileTextureNames = mapData.tileTextureNames;


        foreach (var obj in mapData.objects)
        {
            _objInfos.Add(obj.CreatePlaceInfo());
        }
    }

    public void CreateTilemap()
    {
        if (_mapCreator != null)
        {
            _mapCreator.CreateTilemap(_mapSize, _tileTextureNames, ref _tilemap);
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

    public void PlaceAllObjs()
    {
        foreach(eObjType type in Enum.GetValues(typeof(eObjType)))
        {
            InitObjs(type);
        }
    }

    public void InitObjs(eObjType type)
    {
        GameManager.Get<GameManager.Turn>().ResetRegisterTeams();

        var unitList = _objInfos.FindAll(obj => obj.type == type);

        foreach (var objInfo in unitList)
        {
            if (IsInMapSize(objInfo.pos))
            {
                GameManager.Get<GameManager.Turn>().RegisterTeam(objInfo.team);

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

    public void ChangeAllTileState<T>() where T : Tile.State
    {
        foreach(var tile in _tilemap)
        {
            tile.Value.ChangeState<T>();
        }
    }
}
