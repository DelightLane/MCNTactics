using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class AtlasPacker : EditorWindow
{
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
            PackTexturesToAtlas();
            SaveAtlas();
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
                    tex.name = file;
                    tex.LoadImage(fileData);

                    _targetTextures.Add(tex);
                }
            }
        }

        AtlasPacker window = (AtlasPacker)EditorWindow.GetWindow(typeof(AtlasPacker));
    }

    private void PackTexturesToAtlas()
    {
        if (_atlasName != string.Empty)
        {
            _atlas = new Texture2D(8192, 8192);
            var rects = _atlas.PackTextures(_targetTextures.ToArray(), 0, 8192);
            _atlas.name = GetResultPath();

            AtlasPacker window = (AtlasPacker)EditorWindow.GetWindow(typeof(AtlasPacker));
        }
        else
        {
            EditorUtility.DisplayDialog("Atlas name", "Please input atlas name.", "Ok");
        }
    }

    private string GetResultPath()
    {
        return string.Format("{0}/{1}.png", _path, _atlasName);
    }

    private void SaveAtlas()
    {
        string resultPath = GetResultPath();

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
