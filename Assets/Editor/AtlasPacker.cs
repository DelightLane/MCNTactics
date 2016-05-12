using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class AtlasPacker : EditorWindow
{
    private List<Texture2D> targetTextures = new List<Texture2D>();

    [MenuItem("Window/AtlasPacker")]
    static void Init()
    {
        AtlasPacker window = (AtlasPacker)EditorWindow.GetWindow(typeof(AtlasPacker));
    }

    void OnGUI()
    {
        if(GUILayout.Button("Load"))
        {
            LoadTextures();
        }

        GUILayout.Label("Selected Textures", EditorStyles.boldLabel);

        for (int i = 0; i < targetTextures.Count; ++i)
        {
            targetTextures[i] = EditorGUILayout.ObjectField(targetTextures[i], typeof(Texture2D), allowSceneObjects: false) as Texture2D;
        }
    }

    private void LoadTextures()
    {
        targetTextures.Clear();

        var path = EditorUtility.OpenFolderPanel("Load png Textures of Directory", "", "");
        var files = Directory.GetFiles(path);
        foreach (var file in files)
        {
            if (file.EndsWith(".png"))
            {
                var fileData = File.ReadAllBytes(file);
                Texture2D tex = new Texture2D(2, 2);
                tex.name = file;
                tex.LoadImage(fileData);

                targetTextures.Add(tex);
            }
        }

        AtlasPacker window = (AtlasPacker)EditorWindow.GetWindow(typeof(AtlasPacker));
    }
}
