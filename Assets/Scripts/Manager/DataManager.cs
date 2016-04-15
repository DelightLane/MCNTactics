using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DataManager : MCN.Singletone<DataManager>
{
    private DataManager() { }

    private Dictionary<string, DataObject> _datas = new Dictionary<string, DataObject>();

    public void LoadData(DataFactory factory)
    {
        _datas.Add(factory.GetName(), factory.LoadDatas());
    }
}

#region DataFactory

public abstract class DataFactory
{
    protected class JsonParser<T>
    {
        public T LoadDatas(DataFactory factory)
        {
            T datas;
            try
            {
                datas = JsonUtility.FromJson<T>(factory.GetJsonString());
            }
            catch (Exception e)
            {
                throw new UnityException(typeof(T) + " : " + e.ToString());
            }

            return datas;
        }
    }

    public abstract string GetName();

    public abstract DataObject LoadDatas();

    protected string GetJsonString()
    {
        var textAsset = Resources.Load(string.Format("Data/{0}", GetName())) as TextAsset;
        if (textAsset != null)
        {
            return textAsset.text;
        }
        else
        {
            throw new UnityException("DataFactory : DataPath is not available.");
        }
    }
}

public class UnitDataFactory : DataFactory
{
    public override string GetName()
    {
        return "unitData";
    }

    public override DataObject LoadDatas()
    {
        return new UnitDataObject(new JsonParser<UnitDatas>().LoadDatas(this));
    }
}

#endregion
