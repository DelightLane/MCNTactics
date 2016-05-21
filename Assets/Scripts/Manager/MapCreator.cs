using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapCreator
{
    public void CreateTilemap(Vector2 mapSize, List<string> tileNames, ref Dictionary<Vector2, Tile> resultTilemap)
    {
        RemoveTilemap(ref resultTilemap);

        CreateTiles(mapSize, tileNames, ref resultTilemap);
    }

    public static GameObject GetRoot()
    {
        var root = GameObject.Find("Tilemap");

        if (root == null)
        {
            root = new GameObject("Tilemap");
            root.transform.position = Vector3.up * -1;
        }

        return root;
    }

    public void RemoveTilemap(ref Dictionary<Vector2, Tile> tilemap)
    {
        if(tilemap == null)
        {
            return;
        }

        foreach (var tilePair in tilemap)
        {
            var tile = tilePair.Value.GetComponent<Tile>();

            if (tile != null)
            {
                tile.Dispose();
            }
        }

        tilemap.Clear();
    }

    private void CreateTiles(Vector2 mapSize, List<string> tileNames, ref Dictionary<Vector2, Tile> tilemap)
    {
        if (mapSize.x <= 0 && mapSize.y <= 0)
        {
            return;
        }

        if (tilemap == null)
        {
            tilemap = new Dictionary<Vector2, Tile>();
        }

        for (int i = 0; i < mapSize.x; ++i)
        {
            for (int j = 0; j < mapSize.y; ++j)
            {
                var tilePosition = new Vector2(i, j);
                int tilePositionIdx = i + (int)mapSize.x * j;

                var tile = Tile.CreateTile();
                tile.Initialize(tilePosition, tileNames[tilePositionIdx]);

                tilemap[tilePosition] = tile.GetComponent<Tile>();
            }
        }
    }
}
