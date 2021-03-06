﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class DataManager : FZ.GeneralSingleton<DataManager, DataObject>
{
    private DataManager() { }

    public static void LoadDatas()
    {
        // TODO : 나은 위치에서 로드하게 수정
        // 리플랙션을 사용해서 데이터들을 로드하게 하는 건 어떨지?
        LoadData(new UnitDataFactory());
        LoadData(new UnitActorDataFactory());
        LoadData(new MapDataFactory("test")); // TODO : 맵 이름을 런타임에 변경할 수 있게 수정 필요
        LoadData(new AtlasDataFactory<AtlasType_Tile>("TileAtlas"));
    }

    public static void LoadData(DataFactory factory)
    {
        FZ.GeneralSingleton<DataManager, DataObject>.RegisterHandler(factory.LoadDatas());
    }
}

#region DataFactory

public abstract class DataFactory
{
    protected class JsonParser<T>
    {
        public T LoadDatas(DataFactory factory)
        {
            T datas;
            try
            {
                // 직렬화된 클래스들은 반드시 변수명까지 json 형식과 일치해야 한다.
                datas = JsonUtility.FromJson<T>(factory.GetJsonString());
            }
            catch (Exception e)
            {
                throw new UnityException(typeof(T) + " : " + e.ToString());
            }

            return datas;
        }
    }

    protected abstract string GetName();

    public abstract DataObject LoadDatas();

    protected string GetJsonString()
    {
        var textAsset = Resources.Load(GetName()) as TextAsset;
        if (textAsset != null)
        {
            return textAsset.text;
        }
        else
        {
            throw new UnityException("DataFactory : DataPath is not available.");
        }
    }
}

public class UnitDataFactory : DataFactory
{
    protected override string GetName()
    {
        return "Datas/unit";
    }

    public override DataObject LoadDatas()
    {
        return new JsonParser<UnitDataList>().LoadDatas(this);
    }
}

public class UnitActorDataFactory : DataFactory
{
    protected override string GetName()
    {
        return "Datas/unitActor";
    }

    public override DataObject LoadDatas()
    {
        return new JsonParser<UnitActorDataList>().LoadDatas(this);
    }
}

public class MapDataFactory : DataFactory
{
    private string _name;

    public MapDataFactory(string name)
    {
        _name = name;
    }

    protected override string GetName()
    {
        return string.Format("Datas/map_{0}", _name);
    }
    
    public override DataObject LoadDatas()
    {
        return new JsonParser<MapData>().LoadDatas(this);
    }
}

public class AtlasDataFactory<T> : DataFactory where T : AtlasType
{
    private string _name;

    public AtlasDataFactory(string name)
    {
        _name = name;
    }

    protected override string GetName()
    {
        return string.Format("Atlases/{0}", _name);
    }

    public override DataObject LoadDatas()
    {
        return new JsonParser<AtlasDataList<T>>().LoadDatas(this);
    }
}

#endregion
