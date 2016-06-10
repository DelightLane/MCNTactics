using System;
using System.Collections.Generic;

[Serializable]
public class UnitDataList : DataObject
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
    public int ActRange;
}