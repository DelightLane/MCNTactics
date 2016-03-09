using UnityEngine;
using System.Collections;

public class PlaceableObject : TacticsObject
{
    protected Tile _attachedTile;

    public void Attach(Tile tile)
    {
        // 순환 참조 적용. 레퍼런스 관리에 신경 쓸 것
        _attachedTile = tile;
    }
}
