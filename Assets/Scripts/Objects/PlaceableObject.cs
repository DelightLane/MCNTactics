using UnityEngine;
using System;
using System.Collections;

public class PlaceableObject : TacticsObject, IDisposable, ITileActive
{
    protected Tile _attachedTile;
    
    // 타일에 놓는 건 타일의 AttachedObject 메소드를 사용할 것.
    // 이걸 사용할 수 없게 막을 방법은 없을까?
    public void Attach(Tile tile)
    {
        // 순환 참조 적용. 레퍼런스 관리에 신경 쓸 것
        _attachedTile = tile;

        Place(tile);
    }

    public void Dispose()
    {
        _attachedTile = null;

        GameObject.Destroy(gameObject);
    }

    private void Place(Tile tile)
    {
        transform.parent = tile.transform;
        transform.localPosition = new Vector3(0, transform.localScale.y / 2, 0);

        tile.AttachObject(this);
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
