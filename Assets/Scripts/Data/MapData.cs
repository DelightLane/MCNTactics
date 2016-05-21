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
