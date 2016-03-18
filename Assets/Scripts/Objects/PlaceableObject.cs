using UnityEngine;
using System;
using System.Collections;

public class PlaceableObject : TacticsObject, IDisposable
{
    protected Tile _attachedTile;
    
    public void Attach(Tile tile)
    {
        // 순환 참조 적용. 레퍼런스 관리에 신경 쓸 것
        _attachedTile = tile;

        Place(_attachedTile);
    }

    public void Dispose()
    {
        _attachedTile = null;
    }

    private void Place(Tile tile)
    {
        transform.parent = tile.transform;
        transform.localPosition = new Vector3(0, transform.localScale.y / 2, 0);
    }
}
