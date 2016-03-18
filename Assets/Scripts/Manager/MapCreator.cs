using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapCreator
{
    public void CreateTilemap(Vector2 mapSize, ref Dictionary<Vector2, Tile> resultTilemap)
    {
        RemoveTilemap(ref resultTilemap);

        CreateTiles(mapSize, ref resultTilemap);
    }

    public GameObject GetRoot()
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

    private void CreateTiles(Vector2 mapSize, ref Dictionary<Vector2, Tile> tilemap)
    {
        if (mapSize.x <= 0 && mapSize.y <= 0)
        {
            return;
        }

        if (tilemap == null)
        {
            tilemap = new Dictionary<Vector2, Tile>();
        }

        var offset = 0.03f;

        for (int i = 0; i < mapSize.x; ++i)
        {
            for (int j = 0; j < mapSize.y; ++j)
            {
                var tilePosition = new Vector2(i, j);

                var tile = Tile.CreateTile(tilePosition);

                tile.transform.position = new Vector3(i * (Tile.TILE_SIZE + offset), 0, j * (Tile.TILE_SIZE + offset));

                tile.transform.parent = GetRoot().transform;

                tilemap[tilePosition] = tile.GetComponent<Tile>();
                tilemap[tilePosition].SetPosition(tilePosition);
            }
        }
    }
}
