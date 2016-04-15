using System;
using System.Collections.Generic;

[Serializable]
public class UnitDatas
{
    public List<UnitData> UnitData;
}

[Serializable]
public class UnitData
{
    public int no;
    public string name;
    public string prefabName;
    public List<ActorInfo> actor;
}

[Serializable]
public class ActorInfo
{
    public string name;
    public List<string> weightName;
    public List<int> weightValue;
}

public class UnitDataObject : DataObject
{
    private UnitDatas _data;

    public List<UnitData> Data {  get { return _data.UnitData; } }

    public UnitDataObject(UnitDatas data)
    {
        _data = data;
    }
}