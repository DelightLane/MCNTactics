using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

// 픽셀을 다루기 때문에 아틀라스 텍스쳐는 reabable이어야 한다.
public class MapEditor : EditorWindow
{
    private string _mapName;
    private Vector2 _mapSize;

    private Texture2D _tempTile;

    private Vector2 _mapScrollPos;
    private Vector2 _tileScrollPos;

    private string _resultMapName;
    private Vector2 _resultMapSize;
    private Texture2D[] _tileTextureData;

    private AtlasDataList _tileRawData;

    private readonly float MENU_WIDTH = 300;

    [MenuItem("Window/FZTools/MapEditor")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(MapEditor));
    }

    void OnGUI()
    {
        GUILayout.BeginHorizontal();

        DisplaySettingBox();

        DisplayMapData();

        GUILayout.EndHorizontal();
    }

    private void DisplaySettingBox()
    {
        GUILayout.BeginVertical("Box", GUILayout.Width(MENU_WIDTH));

        _mapName = EditorGUILayout.TextField("Map Name: ", _mapName);
        _mapSize = EditorGUILayout.Vector2Field("Map Size", _mapSize);

        if (GUILayout.Button("New"))
        {
            int mapMaxSize = (int)_mapSize.x * (int)_mapSize.y;

            _tileTextureData = new Texture2D[mapMaxSize];

            for (int y = 0; y < _mapSize.y; ++y)
            {
                for(int x = 0; x < _mapSize.x; ++x)
                {
                    int position = x + (y * (x + 1));
                    
                    // 신규 맵을 작성할 시 첫 번째 타일맵으로 전부 다 채운다.
                    _tileTextureData[position] = GetTileImg(_tileRawData.infos[0]);
                }
            }

            _resultMapSize = _mapSize;
        }
        if (GUILayout.Button("Save"))
        {

        }
        if (GUILayout.Button("Load"))
        {

        }
        if( GUILayout.Button("Load Tilemap"))
        {
            DataManager.Instance.LoadData(new AtlasDataFactory("TileAtlas", DataManager.DataType.ATLAS_TILE));
            _tempTile = Resources.Load("Atlases/TileAtlas", typeof(Texture2D)) as Texture2D;
            _tileRawData = DataManager.Instance.GetData<AtlasDataList>(DataManager.DataType.ATLAS_TILE);
        }

        DisplayTileBtnList();

        GUILayout.EndVertical();
    }

    private void DisplayTileBtnList()
    {
        GUILayout.Label("Tiles", EditorStyles.boldLabel);

        GUILayout.BeginVertical("Box", GUILayout.Width(MENU_WIDTH));
        _tileScrollPos = EditorGUILayout.BeginScrollView(_tileScrollPos);

        if (_tileRawData != null)
        {
            int lineCount = 0;

            for (int i = 0; i < _tileRawData.infos.Length; ++i)
            {
                var imageData = _tileRawData.infos[i];
                Texture2D tileImg = GetTileImg(imageData);
                lineCount += tileImg.width;

                if (lineCount == tileImg.width)
                {
                    EditorGUILayout.BeginHorizontal();
                }

                if (lineCount >= MENU_WIDTH)
                {
                    EditorGUILayout.EndHorizontal();
                    lineCount = 0;
                }

                GUIStyle style = new GUIStyle();
                style.alignment = TextAnchor.MiddleCenter;
                GUIContent content = new GUIContent(tileImg);
                if (GUILayout.Button(content, style, GUILayout.Height(tileImg.height), GUILayout.Width(tileImg.width)))
                {
                    Debug.Log("a");
                }
            }

            if (_tileRawData.infos.Length > 0 && lineCount < MENU_WIDTH)
            {
                EditorGUILayout.EndHorizontal();
            }
        }

        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private Texture2D GetTileImg(AtlasData imageData)
    {
        var leftTop = new Vector2(imageData.offsetX, imageData.offsetY + imageData.scaleY);
        var leftBottom = new Vector2(imageData.offsetX, imageData.offsetY);
        var rightBottom = new Vector2(imageData.offsetX + imageData.scaleX, imageData.offsetY);

        int x = (int)(_tempTile.width * leftBottom.x);
        int y = (int)(_tempTile.height * leftBottom.y);
        int width = (int)(_tempTile.width * (Vector2.Distance(leftBottom, rightBottom)));
        int height = (int)(_tempTile.height * (Vector2.Distance(leftBottom, leftTop)));

        var tileImg = new Texture2D(width, height);
        tileImg.SetPixels(_tempTile.GetPixels(x, y, width, height));
        tileImg.Apply(false);

        return tileImg;
    }
    
    private void DisplayMapData()
    {
        GUILayout.BeginVertical("Box", GUILayout.Height(EditorWindow.focusedWindow.position.height));
        _mapScrollPos = EditorGUILayout.BeginScrollView(_mapScrollPos);
        
        if (_resultMapSize != null)
        {
            for (int y = 0; y < _resultMapSize.x; ++y)
            {
                GUILayout.BeginHorizontal();

                for (int x = 0; x < _resultMapSize.y; ++x)
                {
                    int position = x + (y * (x + 1));
                    int margin = 5;

                    GUIStyle style = new GUIStyle();
                    style.alignment = TextAnchor.MiddleCenter;
                    GUIContent content = new GUIContent(_tileTextureData[position]);
                    if(GUILayout.Button(content, style, GUILayout.Width(_tileTextureData[position].width + margin), GUILayout.Height(_tileTextureData[position].height + margin)))
                    {

                    }

                }
                GUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
}
