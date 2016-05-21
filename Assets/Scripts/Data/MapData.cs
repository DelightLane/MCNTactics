using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class MapData : DataObject
{
    public int x;
    public int y;
    public List<string> tileTextureNames;
    public List<MapObjectData> objects;
}

[Serializable]
public class MapObjectData : DataObject
{
    public string type;
    public int no;
    public string team;
    public int x;
    public int y;
}

public enum eObjType
{
    UNIT,
}

[System.Serializable]
public class PlaceInfo
{
    public Vector2 pos;
    public eCombatTeam team;
    public eObjType type;
    public int no;

    public PlaceInfo(MapObjectData data)
    {
        pos = new Vector2(data.x, data.y);
        team = (eCombatTeam)Enum.Parse(typeof(eCombatTeam), data.team);
        no = data.no;
        type = (eObjType)Enum.Parse(typeof(eObjType), data.type);
    }
}

