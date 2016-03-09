using UnityEngine;
using System.Collections;

public enum eTileDirect
{
    UP,
    DOWN,
    RIGHT,
    LEFT
}

public class Tile : TacticsObject
{
    private Vector2 _position;
    private TacticsObject _attached;

    public void SetPosition(Vector2 pos)
    {
        _position = pos;
    }

    public TacticsObject GetAttachObject()
    {
        return _attached;
    }

    public void AttachObject(PlaceableObject obj)
    {
        // 순환 참조 적용. 레퍼런스 관리에 신경 쓸 것
        _attached = obj;

        obj.Attach(this);
    }

    public Tile GetClosedTile(eTileDirect direct)
    {
        Vector2 closedTilePos = this._position;

        switch (direct)
        {
            case eTileDirect.UP:
                closedTilePos += new Vector2(0, 1);
                break;
            case eTileDirect.DOWN:
                closedTilePos += new Vector2(0, -1);
                break;
            case eTileDirect.RIGHT:
                closedTilePos += new Vector2(1, 0);
                break;
            case eTileDirect.LEFT:
                closedTilePos += new Vector2(-1, 0);
                break;
        }

        if (closedTilePos.x >= 0 && closedTilePos.y >= 0)
        {
            var mapController = GameObject.FindObjectOfType<MapController>();

            if (mapController != null)
            {
                return mapController.GetTile(closedTilePos);
            }
        }

        return null;
    }
}
