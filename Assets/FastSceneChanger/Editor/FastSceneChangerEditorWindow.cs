using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

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

    #region Colors
    Color proTextColor = (Color)new Color32(180, 180, 180, 255);
    Color proDisableTextColor = (Color)new Color32(122, 122, 122, 255);
    Color proSelectTextColor = (Color)new Color32(255, 255, 255, 255);
    Color proSelectDisableTextColor = (Color)new Color32(159, 171, 192, 255);

    Color proSelectBackground = (Color)new Color32(62, 95, 150, 255);
    Color proDisalbeSelectBackground = (Color)new Color32(62, 87, 129, 255);

    Color stdTextColor = (Color)new Color32(0, 0, 0, 255);
    Color stdDisableTextColor = (Color)new Color32(111, 111, 111, 255);
    Color stdSelectTextColor = (Color)new Color32(255, 255, 255, 255);
    Color stdSelectDisableTextColor = (Color)new Color32(179, 202, 242, 255);

    Color stdSelectBackground = (Color)new Color32(62, 125, 231, 255);
    Color stdDisableSelectBackground = (Color)new Color32(102, 149, 229, 255);

    #endregion

    #region for view type icon
    private static int viewOption = 0;
    string[] viewOptions = new string[] { "Full Path", "Only Name" };
    Texture2D eyeIcon = null;
    #endregion

    public Vector2 scrollPosition = Vector2.zero;

    //Editor styles in here! : https://docs.unity3d.com/ScriptReference/EditorStyles.html
    void InitStyles()
    {
        //Make Top Menu Style
        if (topMenuStyle == null)
        {
            topMenuStyle = new GUIStyle((GUIStyle)"Toolbar");

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
        }
    }

    void OnGUI()
    {
        InitStyles();
        Color defColor = GUI.color;


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
                //get active scenes
                Scene activeScene = EditorSceneManager.GetActiveScene();

                GUILayout.Label("Scenes In Build", (GUIStyle)"OL Title");
                #region Show Build Scenes

                //Get build scenes
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                int scenesCount = scenes == null ? 0 : scenes.Length;

                #endregion
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

        GUI.color = defColor;
    }

    void GetAllScenes()
    {
        string folderName = Application.dataPath;
        System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(folderName);
        System.IO.FileInfo[] allFileInfos = dirInfo.GetFiles("*.unity", System.IO.SearchOption.AllDirectories);

        int count = allFileInfos.Length;
        int removeCount = Application.dataPath.Length - 6;
        for (int i = 0; i < count; i++)
        {
            Debug.Log(allFileInfos[i].FullName.Remove(0, removeCount));
        }
    }
}
