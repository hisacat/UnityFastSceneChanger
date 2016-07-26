using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

/*
* Copyright 2016, HisaCat https://github.com/hisacat
* Released under the MIT license
*/

/// <summary>
/// This is the class for fast switching of scene,
/// for development convenience.
/// </summary>
public class FastSceneChangerEditorWindow : EditorWindow
{

    [MenuItem("Window/Fast Scene Changer")]
    static void OpenWindow()
    {
        FastSceneChangerEditorWindow window = (FastSceneChangerEditorWindow)EditorWindow.GetWindow(typeof(FastSceneChangerEditorWindow));
        Texture icon = AssetDatabase.LoadAssetAtPath<Texture>("Assets/Sprites/Gear.png");
        GUIContent titleContent = new GUIContent("Scene", icon);
        window.titleContent = titleContent;

        window.Show();
    }
}
