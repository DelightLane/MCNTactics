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

    public MapData()
    {
        tileTextureNames = new List<string>();
        objects = new List<MapObjectData>();
    }
}

[Serializable]
public class MapObjectData : DataObject
{
    public string type;
    public int no;
    public string team;
    public int x;
    public int y;

    public PlaceInfo CreatePlaceInfo()
    {
        return new PlaceInfo(this);
    }
}

public enum eObjType
{
    UNIT,
}

// 실질적으로 맵에 배치할 때 사용하는 데이터
[System.Serializable]
public class PlaceInfo : ICloneable
{
    public Vector2 pos;
    public eCombatTeam team;
    public eObjType type;
    public int no;

    public PlaceInfo() { }

    public PlaceInfo(MapObjectData data)
    {
        pos = new Vector2(data.x, data.y);
        team = (eCombatTeam)Enum.Parse(typeof(eCombatTeam), data.team);
        no = data.no;
        type = (eObjType)Enum.Parse(typeof(eObjType), data.type);
    }

    public object Clone()
    {
        PlaceInfo clone = new PlaceInfo();
        clone.pos = pos;
        clone.team = team;
        clone.type = type;
        clone.no = no;

        return clone;
    }
}

