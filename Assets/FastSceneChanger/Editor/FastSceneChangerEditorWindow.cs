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
        isGuiStyleInitedWhenProSkin = !EditorGUIUtility.isProSkin;
        InitEyeIcon();
    }

    void InitEyeIcon()
    {
        eyeIcon = EditorGUIUtility.FindTexture("ViewToolOrbit");
    }


    #region GUI Styles
    bool isGuiStyleInitedWhenProSkin = false;
    GUIStyle topMenuStyle = null;
    GUIStyle viewOptionPopupStyle = null;
    GUIStyle sceneBoxStyle = null;

    GUIStyle sceneTextStyle = null;
    #endregion

    #region Colors
    Color proTextColor = Color.clear;
    Color proDisableTextColor = Color.clear;

    Color proSelectBackground = Color.clear;
    Color proDisalbeSelectBackground = Color.clear;

    Color stdTextColor = Color.clear;
    Color stdDisableTextColor = Color.clear;

    Color stdSelectBackground = Color.clear;
    Color stdDisableSelectBackground = Color.clear;
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
        if (isGuiStyleInitedWhenProSkin != EditorGUIUtility.isProSkin)
        {
            InitEyeIcon();
        }

        //Make Top Menu Style
        if (topMenuStyle == null || isGuiStyleInitedWhenProSkin != EditorGUIUtility.isProSkin)
        {
            topMenuStyle = new GUIStyle((GUIStyle)"Toolbar");

            topMenuStyle.fixedHeight = 20;
        }

        //Make View Option Popup style
        if (viewOptionPopupStyle == null || isGuiStyleInitedWhenProSkin != EditorGUIUtility.isProSkin)
        {
            viewOptionPopupStyle = new GUIStyle((GUIStyle)"TE Toolbar");

            GUIStyleState style = new GUIStyleState();
            style.background = eyeIcon;
            style.textColor = Color.clear;
            viewOptionPopupStyle.normal = style;
        }

        if (sceneBoxStyle == null || isGuiStyleInitedWhenProSkin != EditorGUIUtility.isProSkin)
        {
            sceneBoxStyle = new GUIStyle((GUIStyle)"OL box");

            sceneBoxStyle.margin = new RectOffset(10, 10, 4, 4);
            sceneBoxStyle.stretchHeight = false;
        }
        if (sceneTextStyle == null || isGuiStyleInitedWhenProSkin != EditorGUIUtility.isProSkin)
        {
            sceneTextStyle = new GUIStyle(EditorStyles.label);
            sceneTextStyle.active = sceneTextStyle.onActive = sceneTextStyle.normal;
        }

        isGuiStyleInitedWhenProSkin = EditorGUIUtility.isProSkin;
    }
    void InitColors()
    {
        proTextColor = (Color)new Color32(255, 255, 255, 255);
        proDisableTextColor = (Color)new Color32(255, 255, 255, 120);

        proSelectBackground = (Color)new Color32(62, 95, 150, 255);
        proDisalbeSelectBackground = (Color)new Color32(62, 87, 129, 255);

        stdTextColor = (Color)new Color32(0, 0, 0, 255);
        stdDisableTextColor = (Color)new Color32(0, 0, 0, 120);

        stdSelectBackground = (Color)new Color32(62, 125, 231, 255);
        stdDisableSelectBackground = (Color)new Color32(102, 149, 229, 255);
    }
    void OnGUI()
    {
        InitStyles();
        InitColors();

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
            //get active scenes
            Scene activeScene = EditorSceneManager.GetActiveScene();

            GUILayout.BeginVertical(sceneBoxStyle);
            {
                GUILayout.Label("Scenes In Build", (GUIStyle)"OL Title");
                #region Show Build Scenes

                //Get build scenes
                EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
                int scenesCount = scenes == null ? 0 : scenes.Length;

                for (int i = 0; i < scenesCount; i++)
                {
                    bool isEnabled = scenes[i].enabled;
                    bool isActiveScene = (activeScene.path == scenes[i].path);

                    #region get sceneName(path)
                    string sceneName = "";
                    switch (viewOption)
                    {
                        case 0: //Show full path (remove "ASSETS/")
                            sceneName = scenes[i].path.Remove(0, 7);
                            break;
                        case 1: //Show only scene name
                            sceneName = System.IO.Path.GetFileName(scenes[i].path);
                            break;
                    }
                    #endregion

                    sceneName = isActiveScene ? "▶" + sceneName : "　" + sceneName;

                    GUIContent buttonText = new GUIContent(sceneName);
                    Rect rect = GUILayoutUtility.GetRect(buttonText, sceneTextStyle);

                    GUI.color = isEnabled ? (EditorGUIUtility.isProSkin ? proTextColor : stdTextColor) :
                                            (EditorGUIUtility.isProSkin ? proDisableTextColor : stdDisableTextColor);

                    if (GUI.Button(rect, buttonText, sceneTextStyle) && isEnabled)
                    {
                        Debug.Log("ASE");
                    }
                    GUI.color = defColor;
                }

                #endregion
                GUILayout.Space(50);

            }
            GUILayout.EndVertical();

            GUILayout.BeginVertical(sceneBoxStyle);
            {
                GUILayout.Label("All Scenes", (GUIStyle)"OL Title");
                #region Show All Scenes

                #endregion
                GUILayout.Space(50);

            }
            GUILayout.EndVertical();
        }
        GUILayout.EndScrollView();

        GUI.color = defColor;
        Repaint();
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
