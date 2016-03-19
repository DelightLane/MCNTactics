using UnityEngine;
using System;
using System.Collections;

public class PlaceableObject : TacticsObject, IDisposable, ITileActive
{
    protected Tile _placedTile;

    // 타일에 놓는 건 타일의 AttachObject 메소드를 사용할 것.
    // 이걸 사용할 수 없게 막을 방법은 없을까?
    public void Attach(Tile tile)
    {
        // 순환 참조 적용. 레퍼런스 관리에 신경 쓸 것
        _placedTile = tile;

        Place(tile);
    }

    // 타일에 떼는 건 타일의 DetachObject 메소드를 사용할 것.
    // 이걸 사용할 수 없게 막을 방법은 없을까?
    public void Detach()
    {
        _placedTile = null;
    }

    public Tile GetPlacedTile()
    {
        return _placedTile;
    }

    public void Dispose()
    {
        _placedTile = null;

        GameObject.Destroy(gameObject);
    }

    private void Place(Tile tile)
    {
        if (tile != null)
        {
            transform.parent = tile.transform;
            transform.localPosition = new Vector3(0, transform.localScale.y / 2, 0);

            tile.AttachObject(this);
        }
    }

    public bool IsSelected()
    {
        return GameManager.Instance.SelectedObj == this;
    }

    public void Select()
    {
        GameManager.Instance.SelectedObj = this;
    }

    public void Deselect()
    {
        GameManager.Instance.SelectedObj = null;
    }

    public virtual void TileTouchAction(Tile activeTile) { }
}
