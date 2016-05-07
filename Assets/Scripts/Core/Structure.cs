using UnityEngine;
using System;
using System.Collections.Generic;

namespace FZ
{
    public enum eObjType
    {
        UNIT,
    }

    [System.Serializable]
    public class PlaceInfo
    {
        public Vector2 pos;
        public eCombatTeam team;
        public eObjType type;
        public int no;

        public PlaceInfo(MapObjectData data)
        {
            pos = new Vector2(data.x, data.y);
            team = (eCombatTeam)Enum.Parse(typeof(eCombatTeam), data.team);
            no = data.no;
            type = (eObjType)Enum.Parse(typeof(eObjType), data.type);
        }
    }

    #region pairs
    public class Pair<T, T2>
    {
        public T key;
        public T2 value;
    }

    [System.Serializable]
    public class StringIntPair : Pair<string, int>{ }
    #endregion
}
