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
                        AddActor(ref placeableObj, unit.actor);
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

    public static void AddActor(ref PlaceableObject obj, List<ActorInfo> info)
    {
        if (obj != null)
        {
            foreach (var actorInfo in info)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                try
                {
                    Type actorType = assembly.GetType(actorInfo.name);

                    MCN.Actor actor = (MCN.Actor)Activator.CreateInstance(actorType);

                    if (actor != null)
                    {
                        actor.Initialize(obj, actorInfo.weightName, actorInfo.weightValue);
                    }

                    obj.AddActor(actor);

#if UNITY_EDITOR
                    // 이건 디버깅용으로.. 무조껀 큐에 삽입한다.
                    obj.EnqueueActor(actor.GetType());
#endif
                }
                catch (Exception e)
                {
                    throw new UnityException(e.ToString());
                }
            }
        }
    }
}

