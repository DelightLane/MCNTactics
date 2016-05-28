using UnityEngine;
using System;
using System.Collections.Generic;

public class MoveActor : FZ.ActObjActor
{
    #region weight
    public int Range
    {
        get
        {
            return GetWeight("range");
        }
    }

    protected override string[] AbsoluteWeightKey()
    {
        return new string[] { "range" };
    }
    #endregion

    public override void Run()
    {
        base.Run();

        if (ActTarget.GetPlacedTile() != null)
        {
            MapManager.Instance.ChangeAllTileState(eTileType.DEACTIVE);

            var placedTile = ActTarget.GetPlacedTile();

            placedTile.ShowChainActiveTile(Range, (Tile tile) => { return tile.GetAttachObject() != null; });
        }
    }

    public override void Reset()
    {
        base.Reset();

        MapManager.Instance.ChangeAllTileState(eTileType.NORMAL);
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
                this.FinishActor();
            }
        }
    }
}
