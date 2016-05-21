using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

// 픽셀을 다루기 때문에 아틀라스 텍스쳐는 reabable이어야 한다.
public class MapEditor : EditorWindow
{
    private string _mapName;
    private Vector2 _mapSize;

    private Texture2D _tileAtlasTexture;

    private Vector2 _mapScrollPos;
    private Vector2 _tileScrollPos;

    private string _resultMapName;
    private Vector2 _resultMapSize;
    private AtlasData[] _tileTextureData;

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
            if(_tileRawData == null || _tileRawData.infos.Length <= 0)
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
            _tileRawData = DataManager.Instance.GetData<AtlasDataList>(DataManager.DataType.ATLAS_TILE);
            _tileAtlasTexture = _tileRawData.GetMaterial().mainTexture as Texture2D;
        }

        DisplaySelectTileList();

        GUILayout.EndVertical();
    }

    private void LoadTileMap()
    {
        DataManager.Instance.LoadData(new AtlasDataFactory("TileAtlas", DataManager.DataType.ATLAS_TILE));
        _tileAtlasTexture = Resources.Load("Atlases/TileAtlas", typeof(Texture2D)) as Texture2D;
        _tileRawData = DataManager.Instance.GetData<AtlasDataList>(DataManager.DataType.ATLAS_TILE);
    }

    public int _selTileGridInt = -1;
    private void DisplaySelectTileList()
    {
        if (_tileRawData != null)
        {
            int lineWidth = 0;
            int lineCount = 0;
            
            List<GUIContent> contents = new List<GUIContent>();

            for (int i = 0; i < _tileRawData.infos.Length; ++i)
            {
                var tileData = _tileRawData.infos[i];
                Texture2D tileImg = GetTileImg(tileData);
 
                lineWidth += tileImg.width;
               
                if (lineWidth <= MENU_WIDTH)
                {                    
                    lineCount += 1;
                }

                contents.Add(new GUIContent(tileImg));
            }

            if(lineCount <= 0)
            {
                lineCount = 1;
            }

            GUILayout.Label("Tiles", EditorStyles.boldLabel);

            GUILayout.BeginVertical("Box", GUILayout.Width(MENU_WIDTH));
            _tileScrollPos = EditorGUILayout.BeginScrollView(_tileScrollPos);

            _selTileGridInt = GUILayout.SelectionGrid(_selTileGridInt, contents.ToArray(), lineCount, GUILayout.ExpandWidth(false));

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
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

        int x = (int)(_tileAtlasTexture.width * leftBottom.x);
        int y = (int)(_tileAtlasTexture.height * leftBottom.y);
        int width = (int)(_tileAtlasTexture.width * (Vector2.Distance(leftBottom, rightBottom)));
        int height = (int)(_tileAtlasTexture.height * (Vector2.Distance(leftBottom, leftTop)));

        var tileImg = new Texture2D(width, height);
        tileImg.SetPixels(_tileAtlasTexture.GetPixels(x, y, width, height));
        tileImg.Apply(false);

        _tileImgPool.Add(imageData.imageName, tileImg);

        return tileImg;
    }
    

    public int _selMapGridInt = -1;
    private void DisplayMapData()
    {
        _selMapGridInt = -1;
        var margin = 3;

        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.margin = new RectOffset(margin, margin, margin, margin);

        List<GUIContent> contents = new List<GUIContent>();
        
        if (_resultMapSize != null)
        {
            for (int y = 0; y < _resultMapSize.y; ++y)
            {
                for (int x = 0; x < _resultMapSize.x; ++x)
                {
                    int position = x + (int)_resultMapSize.x * y;

                    var texture = GetTileImg(_tileTextureData[position]);
                    GUIContent content = new GUIContent(texture);
                    contents.Add(content);
                }
            }
        }

        if (contents.Count > 0)
        {
            GUILayout.BeginVertical("Box", GUILayout.Height(EditorWindow.focusedWindow.position.height));
            _mapScrollPos = EditorGUILayout.BeginScrollView(_mapScrollPos);

            _selMapGridInt = GUILayout.SelectionGrid(_selMapGridInt, contents.ToArray(), (int)_resultMapSize.x, style, GUILayout.ExpandWidth(false));
            if (_selTileGridInt > -1 && _selMapGridInt >= 0)
            {
                _tileTextureData[_selMapGridInt] = _tileRawData.infos[_selTileGridInt];
            }

            EditorGUILayout.EndScrollView();
            GUILayout.EndVertical();
        }
    }
}
