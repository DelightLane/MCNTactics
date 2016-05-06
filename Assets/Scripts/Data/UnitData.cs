using System;
using System.Collections.Generic;

[Serializable]
public class UnitDatas
{
    public List<UnitData> unit;
}

[Serializable]
public class UnitData : DataObject
{
    public int no;
    public string name;
    public string prefabName;
    public int Hp;
    public int Sp;
    public int Atk;
    public int Def;
}

public class UnitDataList : DataObject
{
    private UnitDatas _data;

    public List<UnitData> Data {  get { return _data.unit; } }

    public UnitDataList(UnitDatas data)
    {
        _data = data;
    }
}