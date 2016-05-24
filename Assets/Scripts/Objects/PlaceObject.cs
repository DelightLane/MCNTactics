using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using FZ;

public class PlaceObject : TacticsObject, IDisposable, FZ.IObserver<eTouchEvent>
{
    protected Tile _placedTile;

    public virtual void Initialize(DataObject data)
    {
        TouchManager.Instance.Subscribe(this);
    }

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
            transform.localPosition = new Vector3(0, tile.transform.localScale.y, 0);

            tile.AttachObject(this);
        }
    }

    public bool IsSelected()
    {
        return GameManager.Instance.SelectedObj == this;
    }

    public void Select()
    {
        // TODO : 정상적인 선택 표시를 적용해줄 것
        this.GetComponent<Renderer>().material.color = Color.red;

        GameManager.Instance.SelectedObj = this;
    }

    public void Deselect()
    {
        // TODO : 정상적인 선택 취소 표시를 적용해줄 것
        this.GetComponent<Renderer>().material.color = Color.white;

        GameManager.Instance.SelectedObj = null;
    }

    public void OnNext(eTouchEvent data)
    {
        if(_placedTile != null)
        {
            _placedTile.OnNext(data);
        }
    }
}
