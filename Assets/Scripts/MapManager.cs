﻿using UnityEngine;
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

        CreateTilemap();

        PlaceObjacts();
    }

    public void CreateTilemap()
    {
        if (_mapCreator != null)
        {
            _mapCreator.CreateTilemap(_mapSize, out _tilemaps);
        }
    }

    public void RemoveTtilemap()
    {
        if (_mapCreator != null)
        {
            _mapCreator.RemoveTilemap(ref _tilemaps);
        }
    }

    public void PlaceObjacts()
    {
        foreach (var objInfo in _placeObjInfos)
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
            _tilemaps[pos].AttachObject(obj);
        }
        else
        {
            throw new UnityException("Tilemap is not exist.");
        }
    }
}
