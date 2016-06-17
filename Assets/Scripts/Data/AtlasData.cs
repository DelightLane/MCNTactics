using UnityEngine;
using System;
using System.Collections.Generic;

// 아틀라스 데이터들의 타입을 클래스 타입으로 정의
public abstract class AtlasType { }
public class AtlasType_Tile : AtlasType { }

// 아틀라스 데이터별로 다른 AtlasDataList의 구현을 가진다.
// ex) AtlasDataList<AtlasType_Tile> 클래스는 현재 맵의 타일 데이터를 가진다.
public class AtlasDataList<T> : AtlasDataList where T : AtlasType { }

[System.Serializable]
public class AtlasDataList : DataObject
{
    public string name;
    public AtlasData[] infos;

    public Material GetMaterial()
    {
        return Resources.Load(string.Format("Materials/{0}", name), typeof(Material)) as Material;
    }

    public AtlasData GetImageData(string imageName)
    {
        for(int i = 0; i < infos.Length; ++i)
        {
            if(infos[i].imageName == imageName)
            {
                return infos[i];
            }
        }

        throw new UnityException(string.Format("Image name {0} is not available.", imageName));
    }

    public string GetName()
    {
        return name;
    }

    public AtlasData[] GetInfos()
    {
        return infos;
    }
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
