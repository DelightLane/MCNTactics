using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class AtlasDataList : DataObject
{
    public AtlasData[] infos;
}

[System.Serializable]
public class AtlasData : DataObject
{
    public string imageName;
    public float offsetX;
    public float offsetY;
    public float scaleX;
    public float scaleY;
}
