using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class AttachActorDataList : DataObject
{
    public List<AttachActorData> attachActor;

    public List<ActorInfo> GetActorList(int unitNo)
    {
        return attachActor.Find(actor => actor.no == unitNo).actor;
    }
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