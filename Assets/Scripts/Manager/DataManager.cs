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
        _datas.Add(factory.GetName(), factory.LoadData());
    }
}

#region DataFactory

public abstract class DataFactory
{
    public abstract DataObject LoadData();

    public abstract string GetName();
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
    public override DataObject LoadData()
    {
        // TODO : 작동 안됨. 나은 JSON 파서로 변경
        return JsonUtility.FromJson<UnitData>(GetJsonString());
    }

    public override string GetName()
    {
        return "unitData";
    }
}
#endregion
