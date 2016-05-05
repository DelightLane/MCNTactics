using System;
using System.Collections.Generic;

[Serializable]
public class UnitDatas
{
    public List<Unit> unit;
}

[Serializable]
public class Unit
{
    public int no;
    public string name;
    public string prefabName;
    public int Hp;
    public int Sp;
    public int Atk;
    public int Def;
}

public class UnitDataObject : DataObject
{
    private UnitDatas _data;

    public List<Unit> Data {  get { return _data.unit; } }

    public UnitDataObject(UnitDatas data)
    {
        _data = data;
    }
}