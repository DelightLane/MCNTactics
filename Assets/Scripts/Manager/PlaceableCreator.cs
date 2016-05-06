using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

public class PlaceableCreator
{
    private PlaceableCreator() { }

    public static PlaceableObject Create(string unitName)
    {
        var unitData = DataManager.Instance.GetData(DataManager.DataType.UNIT) as UnitDataObject;

        foreach (var unit in unitData.Data)
        {
            if (unit.name == unitName)
            {
                var targetObj = GameObject.Instantiate(Resources.Load(string.Format("Prefabs/{0}", unit.prefabName), typeof(GameObject))) as GameObject;
                if (targetObj != null)
                {
                    var placeableObj = targetObj.GetComponent<PlaceableObject>() as PlaceableObject;

                    if (placeableObj != null)
                    {
                        var attachActorData = DataManager.Instance.GetData(DataManager.DataType.ATTACH_ACTOR) as AttachActorDataObject;

                        if (attachActorData != null)
                        {
                            var actorList = attachActorData.GetActorList(unit.no);
                            foreach (var info in actorList)
                            {
                                placeableObj.AddActor(info);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogWarning(string.Format("Prefab {0} don't have 'PlaceableObject'", unit.prefabName));
                    }

                    return placeableObj;
                }
            }
        }

        return null;
    }
}

