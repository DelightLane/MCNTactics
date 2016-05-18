using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MapEditor : EditorWindow
{
    private string _mapName;
    private Vector2 _mapSize;

    private List<Texture2D> _tileData = new List<Texture2D>();

    private readonly float MAP_X = 300;
    private readonly float MAP_Y = 0;
    private readonly float MAP_MARGIN = 3;
    private readonly float MAP_SIZE = 80;

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
        GUILayout.BeginVertical("Box", GUILayout.Width(MAP_X));

        _mapName = EditorGUILayout.TextField("Map Name: ", _mapName);
        _mapSize = EditorGUILayout.Vector2Field("Map Size", _mapSize);
        if (GUILayout.Button("New"))
        {
            _tileData.Add(Resources.Load("Images/tile0", typeof(Texture2D)) as Texture2D);
        }
        if (GUILayout.Button("Save"))
        {

        }
        if (GUILayout.Button("Load"))
        {

        }

        GUILayout.EndVertical();
    }

    private void DisplayMapData()
    {
        // TODO : 제대로 유닛과 맵이 보이게 수정
        for (int i = 0; i < _tileData.Count; ++i)
        {
            GUIStyle style = new GUIStyle();
            style.normal.background = _tileData[i];
            style.alignment = TextAnchor.MiddleCenter;
            GUIContent content = new GUIContent(Resources.Load("images/tile1", typeof(Texture2D)) as Texture2D);
            EditorGUI.LabelField(new Rect(MAP_X + (i + 1) * MAP_MARGIN + i * MAP_SIZE, MAP_Y + MAP_MARGIN, MAP_SIZE, MAP_SIZE), content, style);
        }
    }
}
