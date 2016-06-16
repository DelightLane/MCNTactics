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
            MapManager.Instance.ChangeAllTileState<Tile.State_Deactive>();

            var placedTile = ActTarget.GetPlacedTile();

            placedTile.ActiveChain(Range, new Tile.ChainInfo((Tile tile) =>
            {
                if(tile.GetAttachObject() != null)
                {
                    if (tile.GetAttachObject() is UnitObject)
                    {
                        if ((tile.GetAttachObject() as UnitObject).Team == ActTarget.Team)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }

                return false;
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
            var damagedTarget = activeTile.GetAttachObject() as UnitObject;

            if (damagedTarget != null)
            {
                damagedTarget.Damaged(this);

                this.RequestFinish();
            }
        }
    }
}
