using UnityEngine;
using System;
using System.Collections.Generic;

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

        throw new UnityException("Image name is not available.");
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
