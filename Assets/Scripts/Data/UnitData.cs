using System;
using System.Collections.Generic;

public class UnitData : DataObject
{
    public int no;
    public string name;
    public string prefabName;
    public ActorInfo actor;

    public class ActorInfo
    {
        public string name;
        public List<string> weightName;
        public List<int> weightValue;
    }
}
