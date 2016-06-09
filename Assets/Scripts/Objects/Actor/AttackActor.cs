using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AttackActor : FZ.UnitObjActor
{
    #region weight
    public int Range
    {
        get
        {
            return GetWeight("range");
        }
    }

    public int Damage
    {
        get
        {
            int atk = ActTarget.Atk;
            
            return atk + GetWeight("damage");
        }
    }

    protected override List<string> AbsoluteWeightKey()
    {
        var absoluteWeightList = base.AbsoluteWeightKey();
        absoluteWeightList.Add("range");
        absoluteWeightList.Add("damage");

        return absoluteWeightList;
    }
    #endregion   

    public override void Run()
    {
        base.Run();

        if (ActTarget.GetPlacedTile() != null)
        {
            MapManager.Instance.ChangeAllTileState(eTileType.DEACTIVE);

            var placedTile = ActTarget.GetPlacedTile();

            Func<Tile, bool> tileDeactiveCond = (Tile tile) =>
            {
                return (tile.GetAttachObject() != null && !(tile.GetAttachObject() is UnitObject)) ||
                       (tile.GetAttachObject() is UnitObject && (tile.GetAttachObject() as UnitObject).Team == ActTarget.Team);
            };

            placedTile.ShowChainActiveTile(Range, tileDeactiveCond);
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
            var damagedTarget = activeTile.GetAttachObject() as UnitObject;

            if (damagedTarget != null)
            {
                damagedTarget.Damaged(this);

                this.RequestFinish();
            }
        }
    }
}
