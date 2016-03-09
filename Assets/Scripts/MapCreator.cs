using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapCreator : Singletone<MapCreator>
{
    private readonly float TILE_SIZE = 1;

    private MapCreator() { }

    private GameObject CreateTile()
    {
        var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);

        tile.AddComponent<Tile>();
        tile.transform.localScale = new Vector3(TILE_SIZE, 0.05f, TILE_SIZE);

        return tile;
    }

    public void CreateTilemap(Vector2 mapSize, out Dictionary<Vector2, Tile> resultTilemap)
    {
        var root = GameObject.Find("Tilemap");
        if(root == null)
        {
            root = new GameObject("Tilemap");
            root.transform.position = Vector3.up * -1;
        }
        else
        {
            var rootChilds = root.GetComponentsInChildren<GameObject>();

            foreach(var child in rootChilds)
            {
                GameObject.Destroy(child);
            }
        }

        var tilemaps = new Dictionary<Vector2, Tile>();

        var offset = 0.03f;

        for (int i = 0; i < mapSize.x; ++i)
        {
            for(int j = 0; j < mapSize.y; ++j)
            {
                var tile = CreateTile();

                tile.transform.position = new Vector3(i * (TILE_SIZE + offset), 0, j * (TILE_SIZE + offset));

                tile.transform.parent = root.transform;

                var tilePosition = new Vector2(i, j);
                tilemaps[tilePosition] = tile.GetComponent<Tile>();
                tilemaps[tilePosition].SetPosition(tilePosition);
            }
        }

        resultTilemap = tilemaps;
    }
}
