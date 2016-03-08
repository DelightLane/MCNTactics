using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MapManager : MonoBehaviour {
    [SerializeField]
    private Vector2 _mapSize;

    private Dictionary<Vector2, Tile> _tilemaps;

	void Awake()
    {
        MapCreator.Instance.CreateTilemap(_mapSize, out _tilemaps);
    }

    public Tile GetTile(Vector2 position)
    {
        return _tilemaps[position];
    }
}
