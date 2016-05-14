using UnityEngine;
using System;
using System.Collections.Generic;

[System.Serializable]
public class AtlasDataList : DataObject
{
    private class MaterialManager : FZ.Singletone<MaterialManager>
    {
        private Dictionary<string, Material> _materialPool;

        private MaterialManager()
        {
            _materialPool = new Dictionary<string, Material>();
        }

        public Material GetMaterial(string imageName)
        {
            if (_materialPool.ContainsKey(imageName))
            {
                return _materialPool[imageName];
            }

            return null;
        }

        public void AddMaterial(string imageName, Material mainMaterial, AtlasData imageInfo)
        {
            if (!_materialPool.ContainsKey(imageName))
            {
                var material = Material.Instantiate(mainMaterial);

                material.mainTextureOffset = new Vector2(imageInfo.offsetX, imageInfo.offsetY);
                material.mainTextureScale = new Vector2(imageInfo.scaleX, imageInfo.scaleY);

                _materialPool.Add(imageName, material);
            }
        }
    }

    public string name;
    public AtlasData[] infos;

    public Material GetMaterial(string imageName)
    {
        var material = MaterialManager.Instance.GetMaterial(imageName);

        if(material == null)
        {
            MaterialManager.Instance.AddMaterial(imageName, GetMaterial(), GetImageData(imageName));
            material = MaterialManager.Instance.GetMaterial(imageName);
        }

        return material;
    }

    private Material GetMaterial()
    {
        return Resources.Load(string.Format("Materials/{0}", name), typeof(Material)) as Material;
    }

    private AtlasData GetImageData(string imageName)
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
