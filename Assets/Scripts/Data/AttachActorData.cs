using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class AttachActorDatas
{
    public List<AttachActor> attachActor;
}

[Serializable]
public class AttachActor
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

public class AttachActorDataObject : DataObject
{
    private AttachActorDatas _data;

    private List<AttachActor> Data { get { return _data.attachActor; } }

    public AttachActorDataObject(AttachActorDatas data)
    {
        _data = data;
    }

    public List<ActorInfo> GetActor(int unitNo)
    {
        return Data.Find(actor => actor.no == unitNo).actor;
    }
}
