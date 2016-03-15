using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapController : MCN.MonoSingletone {
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

    private MapCreator _mapCreator;

    private Dictionary<Vector2, Tile> _tilemaps;

    protected override string CreatedObjectName()
    {
        return "MapController";
    }

    void Awake()
    {
        if(_mapCreator == null)
        {
            _mapCreator = new MapCreator();
        }
        _mapCreator.CreateTilemap(_mapSize, out _tilemaps);

        if(_placeObjInfos == null)
        {
            _placeObjInfos = new List<PlaceInfo>();
        }
        foreach(var objInfo in _placeObjInfos)
        {
            var targetObj = Instantiate(Resources.Load(string.Format("Prefabs/{0}", objInfo.prefabName), typeof(GameObject))) as GameObject;
            if(targetObj != null)
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

    public Tile GetTile(Vector2 position)
    {
        return _tilemaps[position];
    }

    public void AttachObject(Vector2 pos, PlaceableObject obj)
    {
        _tilemaps[pos].AttachObject(obj);
    }
}
