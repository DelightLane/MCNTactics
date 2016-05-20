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
    private AtlasData[] _tileTextureData;

    private AtlasDataList _tileRawData;

    private AtlasData _selectTileData;

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
            if(_tileRawData == null)
            {
                LoadTileMap();
            }

            int mapMaxSize = (int)_mapSize.x * (int)_mapSize.y;

            _tileTextureData = new AtlasData[mapMaxSize];

            for (int y = 0; y < _mapSize.y; ++y)
            {
                for(int x = 0; x < _mapSize.x; ++x)
                {
                    int position = x + (int)_mapSize.x * y;
                    
                    // 신규 맵을 작성할 시 첫 번째 타일맵으로 전부 다 채운다.
                    _tileTextureData[position] = _tileRawData.infos[0];
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

        DisplaySelectTileList();

        GUILayout.EndVertical();
    }

    private void LoadTileMap()
    {
        DataManager.Instance.LoadData(new AtlasDataFactory("TileAtlas", DataManager.DataType.ATLAS_TILE));
        _tempTile = Resources.Load("Atlases/TileAtlas", typeof(Texture2D)) as Texture2D;
        _tileRawData = DataManager.Instance.GetData<AtlasDataList>(DataManager.DataType.ATLAS_TILE);
    }

    private void DisplaySelectTileList()
    {
        GUILayout.Label("Tiles", EditorStyles.boldLabel);

        GUILayout.BeginVertical("Box", GUILayout.Width(MENU_WIDTH));
        _tileScrollPos = EditorGUILayout.BeginScrollView(_tileScrollPos);

        if (_tileRawData != null)
        {
            int lineCount = 0;
            int margin = 5;

            for (int i = 0; i < _tileRawData.infos.Length; ++i)
            {
                var tileData = _tileRawData.infos[i];
                Texture2D tileImg = GetTileImg(tileData);
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
                if (_selectTileData == tileData)
                {
                    style.normal.background = new Texture2D(1, 1);
                    style.normal.background.SetPixels(new Color[2] { Color.red, Color.red });
                }
                GUIContent content = new GUIContent(tileImg);
                if (GUILayout.Button(content, style, GUILayout.Height(tileImg.height + margin), GUILayout.Width(tileImg.width + margin)))
                {
                    if (_selectTileData != tileData)
                    {
                        _selectTileData = tileData;
                    }
                    else
                    {
                        _selectTileData = null;
                    }
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

    private Dictionary<string, Texture2D> _tileImgPool = new Dictionary<string, Texture2D>();

    private Texture2D GetTileImg(AtlasData imageData)
    {
        if(_tileImgPool.ContainsKey(imageData.imageName))
        {
            return _tileImgPool[imageData.imageName];
        }

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

        _tileImgPool.Add(imageData.imageName, tileImg);

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
                    int position = x + (int)_resultMapSize.x * y;
                    int margin = 3;

                    var texture = GetTileImg(_tileTextureData[position]);

                    GUIStyle style = new GUIStyle();
                    style.alignment = TextAnchor.MiddleCenter;
                    GUIContent content = new GUIContent(texture);
                    if(GUILayout.Button(content, style, GUILayout.Width(texture.width + margin), GUILayout.Height(texture.height + margin)))
                    {
                        if(_selectTileData != null)
                        {
                            _tileTextureData[position] = _selectTileData;
                        }
                    }

                }
                GUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
}
