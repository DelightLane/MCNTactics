using UnityEngine;
using System;
using System.Collections.Generic;

// TODO : 현재 이통을 할 수가 없음
// 유닛만이 턴 / 액션 가중치 제약을 받는데 그걸 받기 위해서는 UnitObjActor를 상속받는 액터여야 한다.
// 하지만 그 부모 클래스를 상속받으면 확장성에 문제가 생긴다.
// 내일 이 부분을 해결해보자.
public class MoveActor : FZ.ActObjActor, IUnitActor
{
    #region weight
    public int ActPoint
    {
        get
        {
            return GetWeight("actPoint");
        }
    }

    public int Range
    {
        get
        {
            return GetWeight("range");
        }
    }

    protected override List<string> AbsoluteWeightKey()
    {
        var absoluteWeightList = base.AbsoluteWeightKey();
        absoluteWeightList.Add("range");

        return absoluteWeightList;
    }
    #endregion

    public override void Run()
    {
        base.Run();

        if (ActTarget.GetPlacedTile() != null)
        {
            MapManager.Instance.ChangeAllTileState<Tile.State_Deactive>();

            var placedTile = ActTarget.GetPlacedTile();

            placedTile.ActiveChain(Range, new Tile.ChainInfo((Tile tile) => 
            {
                return tile.GetAttachObject() != null;
            }));
        }
    }

    public override void Reset()
    {
        base.Reset();

        MapManager.Instance.ChangeAllTileState<Tile.State_Normal>();
    }

    public override bool OnTouchEvent(eTouchEvent touch)
    {
        return base.OnTouchEvent(touch);
    }

    public override void Interactive(TacticsObject interactTarget)
    {
        base.Interactive(interactTarget);

        var activeTile = interactTarget as Tile;

        if (activeTile != null)
        {
            bool isSuccess = activeTile.AttachObject(ActTarget);

            if (isSuccess)
            {
                this.RequestFinish();
            }
        }
    }
}
