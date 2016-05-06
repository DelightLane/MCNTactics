﻿using UnityEngine;
using System;
using System.Collections.Generic;

namespace FZ
{
    [System.Serializable]
    public class PlaceInfo
    {
        public Vector2 pos;
        public eCombatTeam team;
        public string unitName;
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
