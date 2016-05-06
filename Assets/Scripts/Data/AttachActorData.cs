using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class UnitActorDataList : DataObject
{
    public List<UnitActorData> unitActor;

    public List<ActorInfo> GetActorList(int unitNo)
    {
        return unitActor.Find(actor => actor.no == unitNo).actor;
    }
}

[Serializable]
public class UnitActorData : DataObject
{
    public int no;
    public List<ActorInfo> actor;
}

[Serializable]
public class ActorInfo
{
    public string name;
    public List<string> weightName;
    public List<int> weightValue;
}