using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MCN.MonoSingletone<MapManager> {
    [SerializeField]
    private Vector2 _mapSize;

    [SerializeField]
    private List<PlaceInfo> _placeObjInfos;

    [System.Serializable]
    public class PlaceInfo
    {
        public Vector2 pos;
        public string prefabName;
    }

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

    private Dictionary<Vector2, Tile> _tilemaps;

    protected override string CreatedObjectName()
    {
        return "MapManager";
    }

    void Awake()
    {
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
        _debug = new DebugMapManager();
        _debug.CreateTilemap(this);
#endif
    }

    public void CreateTilemap()
    {
        if (_mapCreator != null)
        {
            _mapCreator.CreateTilemap(_mapSize, ref _tilemaps);
        }
    }

    public void RemoveTilemap()
    {
        if (_mapCreator != null)
        {
            _mapCreator.RemoveTilemap(ref _tilemaps);
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
                var targetObj = Instantiate(Resources.Load(string.Format("Prefabs/{0}", objInfo.prefabName), typeof(GameObject))) as GameObject;
                if (targetObj != null)
                {
                    var placeableObj = targetObj.GetComponent<PlaceableObject>() as PlaceableObject;

                    if (placeableObj != null)
                    {
                        this.AttachObject(objInfo.pos, placeableObj);
                    }
                    else
                    {
                        Debug.LogWarning(string.Format("Prefab {0} don't have 'PlaceableObject'", objInfo.prefabName));
                    }
                }
            }
        }
    }

    public bool IsExistMap()
    {
        return _tilemaps == null ? false : true;
    }

    public Tile GetTile(Vector2 position)
    {
        if (IsExistMap())
        {
            return _tilemaps[position];
        }

        throw new UnityException("Tilemap is not exist.");
    }

    public void AttachObject(Vector2 pos, PlaceableObject obj)
    {
        if (IsExistMap())
        {
            if (IsInMapSize(pos))
            {
                _tilemaps[pos].AttachObject(obj);
            }
            else
            {
                throw new UnityException("Tilemap is smaller than position.");
            }
        }
        else
        {
            throw new UnityException("Tilemap is not exist.");
        }
    }
}
