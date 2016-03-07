using UnityEngine;
using System.Collections;

public class MapCreator
{
    private static MapCreator _instance;

    public static MapCreator Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = new MapCreator();
            }

            return _instance;
        }
    }

    private MapCreator() { }

    private GameObject CreateTile()
    {
        var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);

        tile.transform.localScale = new Vector3(1, 0.05f, 1);

        return tile;
    }

    public void CreateTilemap(Vector2 mapSize)
    {
        var offset = 1.01f;

        for (int i = 0; i < mapSize.x; ++i)
        {
            for(int j = 0; j < mapSize.y; ++j)
            {
                var tile = CreateTile();

                tile.transform.position = new Vector3(i * offset, -1, j * offset);
            }
        }
    }
}
