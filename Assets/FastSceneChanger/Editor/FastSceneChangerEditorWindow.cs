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

    void OnDisable()
    {

    }

    void OnEnable()
    {
        if (eyeIcon == null)
            eyeIcon = EditorGUIUtility.FindTexture("d_ViewToolOrbit");
    }

    #region GUI Styles
    GUIStyle topMenuStyle = null;
    GUIStyle viewOptionPopupStyle = null;
    GUIStyle sceneBoxStyle = null;
    #endregion

    #region for view type icon
    private static int viewOption = 0;
    string[] viewOptions = new string[] { "Default", "OnlyName" };
    Texture2D eyeIcon = null;
    #endregion

    public Vector2 scrollPosition = Vector2.zero;

    void InitStyles()
    {
        //Make Top Menu Style
        if (topMenuStyle == null)
        {
            topMenuStyle = new GUIStyle(EditorStyles.toolbar);
            topMenuStyle.fixedHeight = 20;
        }

        //Make View Option Popup style
        if (viewOptionPopupStyle == null)
        {
            viewOptionPopupStyle = new GUIStyle((GUIStyle)"TE Toolbar");

            GUIStyleState style = new GUIStyleState();
            style.background = eyeIcon;
            style.textColor = Color.clear;

            viewOptionPopupStyle.normal = style;
        }

        if (sceneBoxStyle == null)
        {
            sceneBoxStyle = new GUIStyle((GUIStyle)"OL box");
            sceneBoxStyle.margin = new RectOffset(10, 10, 4, 4);
            sceneBoxStyle.stretchHeight = false;
            Debug.Log(sceneBoxStyle.stretchHeight);
        }
    }


    void OnGUI()
    {
        //Editor styles in here! : https://docs.unity3d.com/ScriptReference/EditorStyles.html

        InitStyles();

        GUILayout.BeginVertical(topMenuStyle);
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Scenes");
                viewOption = EditorGUILayout.Popup("", viewOption, viewOptions, viewOptionPopupStyle, GUILayout.Width(18));
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();


        scrollPosition = GUILayout.BeginScrollView(scrollPosition, (GUIStyle)"hostview");
        {
            GUILayout.BeginVertical(sceneBoxStyle);
            {
                GUILayout.Label("Scenes In Build", (GUIStyle)"OL Title");
                GUILayout.Space(50);

            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(sceneBoxStyle);
            {
                GUILayout.Label("All Scenes", (GUIStyle)"OL Title");
                GUILayout.Space(50);

            }
            GUILayout.EndVertical();
        }
        GUILayout.EndScrollView();
    }
}
