using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class AtlasPacker : EditorWindow
{
    private enum eAtlasType
    {
        PNG,
        JSON,
        LOAD_TEXTURE
    }

    public static string AtlasMaterialPath = "Resources/Materials";
    public static string AtlasTexturePath = "Resources/Atlases";
    

    private List<Texture2D> _targetTextures = new List<Texture2D>();
    private Texture2D _atlas = null;

    private Vector2 _scrollPosition;

    private string _path;

    private string _atlasName;

    [MenuItem("Window/FZTools/AtlasPacker")]
    static void Init()
    {
        EditorWindow.GetWindow(typeof(AtlasPacker));
    }

    void OnGUI()
    {
        if(GUILayout.Button("Select Image Folder"))
        {
            LoadTextures();
        }

        if(GUILayout.Button("Make Atlas"))
        {
            if (_targetTextures.Count > 0)
            {
                SaveEverything();
            }
            else
            {
                EditorUtility.DisplayDialog("Save", "Select Images First.", "Ok");
            }
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
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            GUILayout.Label("Selected Textures", EditorStyles.boldLabel);

            for (int i = 0; i < _targetTextures.Count; ++i)
            {
                _targetTextures[i] = EditorGUILayout.ObjectField(_targetTextures[i], typeof(Texture2D), allowSceneObjects: false) as Texture2D;
            }

            EditorGUILayout.EndScrollView();
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

        EditorWindow.GetWindow(typeof(AtlasPacker));
    }

    private string GetTextureName(string filePath)
    {
        string[] splitPath = filePath.Split('.', '/', '\\');
        string name = splitPath[splitPath.Length - 2];

        return name;
    }

    private void SaveEverything()
    {
        SaveJson(PackAtlas());
        SaveMaterial();
    }

    private Rect[] PackAtlas()
    {
        if (_atlasName != null && _atlasName != string.Empty)
        {
            var atlas = new Texture2D(8192, 8192);
            Rect[] rects = atlas.PackTextures(_targetTextures.ToArray(), 0, 8192);

            atlas.name = _atlasName;

            SaveAtlas(atlas);

            // assign
            _atlas = atlas;

            EditorWindow.GetWindow(typeof(AtlasPacker));

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
            return string.Format("{0}/{1}/{2}.png", Application.dataPath, AtlasTexturePath, _atlasName);
        }
        else if (type == eAtlasType.JSON)
        {
            return string.Format("{0}/{1}/{2}.json", Application.dataPath, AtlasTexturePath, _atlasName);
        }
        else if (type == eAtlasType.LOAD_TEXTURE)
        {
            string path = string.Empty;

            string[] pathSplit = AtlasTexturePath.Split('/', '\\');
            bool passPivot = false;
            for (int i = 0; i < pathSplit.Length; ++i)
            {
                if (passPivot)
                {
                    path += pathSplit[i];

                    if (i < pathSplit.Length - 1)
                    {
                        path += '/';
                    }
                }

                if (pathSplit[i] == "Resources")
                {
                    passPivot = true;
                }
            }

            return string.Format("{0}/{1}", path, _atlasName);
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
                info.offsetX = rectInfos[i].x;
                info.offsetY = rectInfos[i].y;
                info.scaleX = rectInfos[i].width;
                info.scaleY = rectInfos[i].height;

                _rects.Add(info);
            }
        }

        AtlasDataList atlasInfo = new AtlasDataList();
        atlasInfo.name = _atlasName;
        atlasInfo.infos = _rects.ToArray();

        string json = JsonUtility.ToJson(atlasInfo);
        string resultPath = GetResultPath(eAtlasType.JSON);

        if (resultPath == null || resultPath == string.Empty)
        {
            return;
        }

        try
        {
            var file = File.Open(resultPath, FileMode.Create, FileAccess.Write);
            var binary = new BinaryWriter(file);
            binary.Write(json.ToCharArray());
            file.Close();
        }
        catch (IOException e)
        {
            EditorUtility.DisplayDialog("Save", e.ToString(), "Ok");
        }
    }

    private void SaveAtlas(Texture2D atlas)
    {
        string resultPath = GetResultPath(eAtlasType.PNG);

        if (resultPath == null || resultPath == string.Empty)
        {
            return;
        }

        try
        {
            Directory.CreateDirectory(AtlasTexturePath);

            var bytes = atlas.EncodeToPNG();
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

    // 반드시 아틀라스 텍스쳐가 만들어진 후에 불려야 한다.
    private void SaveMaterial()
    {
        string resultPath = string.Format("Assets/{0}/{1}.mat", AtlasMaterialPath, _atlasName);

        Material material = new Material(Shader.Find("Unlit/Texture"));
        material.mainTexture = Resources.Load(GetResultPath(eAtlasType.LOAD_TEXTURE), typeof(Texture)) as Texture;

        if (material.mainTexture != null)
        {
            try
            {
                Directory.CreateDirectory(AtlasMaterialPath);

                AssetDatabase.CreateAsset(material, resultPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            catch(IOException e)
            {
                EditorUtility.DisplayDialog("Save", e.ToString(), "Ok");
            }
        }
    }
}
