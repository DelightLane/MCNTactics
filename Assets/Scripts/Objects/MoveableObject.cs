using UnityEngine;
using System;
using System.Collections;

public class MoveableObject : PlaceableObject
{
    [SerializeField]
    private int moveRange;

    void Update()
    {
        OnTouchEvent(() =>
        {
            if (_attachedTile != null)
            {
                _attachedTile.ShowChainActiveTile(moveRange);
            }
        });
    }

    private void OnTouchEvent(Action callback)
    {
        // TODO : 마우스가 아니라 실제로 터치 이벤트에 대해 동작하게 수정
        if (Input.GetMouseButtonUp(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                callback();
            }
        }
    }
}
