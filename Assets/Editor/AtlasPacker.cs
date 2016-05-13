using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class AtlasPacker : EditorWindow
{
    private enum eAtlasType
    {
        PNG,
        JSON
    }

    private List<Texture2D> _targetTextures = new List<Texture2D>();
    private Texture2D _atlas = null;

    private string _path;

    private string _atlasName = "Atlas";

    [MenuItem("Window/AtlasPacker")]
    static void Init()
    {
        AtlasPacker window = (AtlasPacker)EditorWindow.GetWindow(typeof(AtlasPacker));
    }

    void OnGUI()
    {
        if(GUILayout.Button("Select Image Folder"))
        {
            LoadTextures();
        }

        if(GUILayout.Button("Make Atlas"))
        {
            var atlasInfo = PackTexturesToAtlas();
            SaveAtlas();
            SaveJson(atlasInfo);
        }

        _atlasName = EditorGUILayout.TextField("Atlas Name: ", _atlasName);

        DisplayResultAtlas();

        DisplayTargetTextures();
    }

    #region Display Methods
    private void DisplayResultAtlas()
    {
        if (_atlas != null)
        {
            GUILayout.Label("Result Atlas", EditorStyles.boldLabel);

            _atlas = EditorGUILayout.ObjectField(_atlas, typeof(Texture2D), allowSceneObjects: false) as Texture2D;
        }
    }

    private void DisplayTargetTextures()
    {
        if (_targetTextures.Count > 0)
        {
            GUILayout.Label("Selected Textures", EditorStyles.boldLabel);

            for (int i = 0; i < _targetTextures.Count; ++i)
            {
                _targetTextures[i] = EditorGUILayout.ObjectField(_targetTextures[i], typeof(Texture2D), allowSceneObjects: false) as Texture2D;
            }
        }
    }
    #endregion

    private void LoadTextures()
    {
        _targetTextures.Clear();

        _path = EditorUtility.OpenFolderPanel("Load png Textures of Directory", "/Resources/Images/", "");

        if (_path != null && _path != string.Empty)
        {
            var files = Directory.GetFiles(_path);
            foreach (var file in files)
            {
                if (file.EndsWith(".png"))
                {
                    var fileData = File.ReadAllBytes(file);
                    Texture2D tex = new Texture2D(2, 2);

                    string name = GetTextureName(file);

                    tex.name = name;
                    tex.LoadImage(fileData);

                    _targetTextures.Add(tex);
                }
            }
        }

        AtlasPacker window = (AtlasPacker)EditorWindow.GetWindow(typeof(AtlasPacker));
    }

    private string GetTextureName(string filePath)
    {
        string[] splitPath = filePath.Split('.', '/', '\\');
        string name = splitPath[splitPath.Length - 2];

        return name;
    }

    private Rect[] PackTexturesToAtlas()
    {
        if (_atlasName != string.Empty)
        {
            _atlas = new Texture2D(8192, 8192);
            Rect[] rects = _atlas.PackTextures(_targetTextures.ToArray(), 0, 8192);

            _atlas.name = GetTextureName(GetResultPath(eAtlasType.PNG));

            AtlasPacker window = (AtlasPacker)EditorWindow.GetWindow(typeof(AtlasPacker));

            return rects;
        }
        else
        {
            EditorUtility.DisplayDialog("Atlas name", "Please input atlas name.", "Ok");
        }

        return null;
    }

    private string GetResultPath(eAtlasType type)
    {
        if (type == eAtlasType.PNG)
        {
            return string.Format("{0}/{1}.png", _path, _atlasName);
        }
        else if(type == eAtlasType.JSON)
        {
            return string.Format("{0}/{1}.json", _path, _atlasName);
        }

        throw new UnityException("eAtlasType is not invailable.");
    }

    private void SaveJson(Rect[] rectInfos)
    {
        if(rectInfos == null)
        {
            return;
        }

        List<AtlasData> _rects = new List<AtlasData>();

        for(int i = 0; i < rectInfos.Length; ++i)
        {
            if (_targetTextures[i] != null)
            {
                var info = new AtlasData();
                info.imageName = _targetTextures[i].name;
                info.x = rectInfos[i].x;
                info.y = rectInfos[i].y;
                info.width = rectInfos[i].width;
                info.height = rectInfos[i].height;

                _rects.Add(info);
            }
        }

        AtlasDataList atlasInfo = new AtlasDataList();
        atlasInfo.infos = _rects.ToArray();

        string json = JsonUtility.ToJson(atlasInfo);
        string resultPath = GetResultPath(eAtlasType.JSON);

        if (resultPath == null || resultPath == string.Empty)
        {
            return;
        }

        try
        {
            var file = File.Open(resultPath, FileMode.Create);
            var binary = new BinaryWriter(file);
            binary.Write(json);
            file.Close();
        }
        catch (IOException e)
        {
            
        }
    }

    private void SaveAtlas()
    {
        string resultPath = GetResultPath(eAtlasType.PNG);

        if (resultPath == null || resultPath == string.Empty)
        {
            return;
        }

        try
        {
            var bytes = _atlas.EncodeToPNG();
            var file = File.Open(resultPath, FileMode.Create);
            var binary = new BinaryWriter(file);
            binary.Write(bytes);
            file.Close();

            EditorUtility.DisplayDialog("Save", "Making atlas is Successful.", "Ok");
        }
        catch (IOException e)
        {
            EditorUtility.DisplayDialog("Save", e.ToString(), "Ok");
        }
    }
}
