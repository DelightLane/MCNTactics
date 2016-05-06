using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class AttachActorDatas
{
    public List<AttachActorData> attachActor;
}

[Serializable]
public class AttachActorData : DataObject
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

public class AttachActorDataList : DataObject
{
    private AttachActorDatas _data;

    private List<AttachActorData> Data { get { return _data.attachActor; } }

    public AttachActorDataList(AttachActorDatas data)
    {
        _data = data;
    }

    public List<ActorInfo> GetActorList(int unitNo)
    {
        return Data.Find(actor => actor.no == unitNo).actor;
    }
}
