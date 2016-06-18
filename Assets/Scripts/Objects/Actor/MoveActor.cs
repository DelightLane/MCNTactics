using UnityEngine;
using System;
using System.Collections.Generic;

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

            var chainInfo = new Tile.ChainInfo((Tile tile) =>
            {
                return tile.GetAttachObject() != null;
            });
            chainInfo.Cost = new Tile.ObjectCost(ActTarget);

            placedTile.ActiveChain(Range, chainInfo);
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
