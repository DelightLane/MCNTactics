using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

// 픽셀을 다루기 때문에 아틀라스 텍스쳐는 reabable이어야 한다.
public class MapEditor : EditorWindow
{
    private string _mapName;
    private Vector2 _mapSize;

    private Texture2D _tempTile;


    private Vector2 _heightScrollPos; // 맵 스크롤


    private string _resultMapName;
    private Vector2 _resultMapSize;
    private Texture2D[] _tileData;

    private readonly float MAP_START_X = 300;
    private readonly float MAP_START_Y = 0;
    private readonly float MAP_MARGIN = 3;
    private readonly float MAP_SIZE = 40;

    [MenuItem("Window/FZTools/MapEditor")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(MapEditor));
    }

    void OnGUI()
    {
        _tempTile = Resources.Load("Atlases/TileAtlas", typeof(Texture2D)) as Texture2D;

        GUILayout.BeginHorizontal();

        DisplaySettingBox();

        DisplayMapData();

        GUILayout.EndHorizontal();
    }

    private void DisplaySettingBox()
    {
        GUILayout.BeginVertical("Box", GUILayout.Width(MAP_START_X));

        _mapName = EditorGUILayout.TextField("Map Name: ", _mapName);
        _mapSize = EditorGUILayout.Vector2Field("Map Size", _mapSize);

        if (GUILayout.Button("New"))
        {
            int mapMaxSize = (int)_mapSize.x * (int)_mapSize.y;

            _tileData = new Texture2D[mapMaxSize];

            for (int y = 0; y < _mapSize.y; ++y)
            {
                for(int x = 0; x < _mapSize.x; ++x)
                {
                    int position = x + (y * (x + 1));

                    // TODO : 타일 데이터를 가져와서 아틀라스에서 해당 부분을 자르게 수정
                    _tileData[position] = new Texture2D(32, 32);
                    _tileData[position].SetPixels(_tempTile.GetPixels(0, 0, 32, 32));
                    _tileData[position].Apply(false);
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
    }

    private void DisplayMapData()
    {
        GUILayout.BeginVertical("Box", GUILayout.Height(EditorWindow.focusedWindow.position.height));
        _heightScrollPos = EditorGUILayout.BeginScrollView(_heightScrollPos);
        // TODO : 제대로 유닛과 맵이 보이게 수정
        if (_resultMapSize != null)
        {
            for (int y = 0; y < _resultMapSize.x; ++y)
            {
                GUILayout.BeginHorizontal();

                for (int x = 0; x < _resultMapSize.y; ++x)
                {
                    int position = x + (y * (x + 1));

                    GUIStyle style = new GUIStyle();
                    style.normal.background = _tileData[position];
                    style.alignment = TextAnchor.MiddleCenter;
                    GUIContent content = new GUIContent(Resources.Load("images/tile1", typeof(Texture2D)) as Texture2D);
                    EditorGUILayout.LabelField(content, style, GUILayout.Width(MAP_SIZE), GUILayout.Height(MAP_SIZE));

                }
                GUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();
        GUILayout.EndVertical();
    }
}
