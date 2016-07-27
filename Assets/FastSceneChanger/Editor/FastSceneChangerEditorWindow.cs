using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
namespace FastSceneChanger
{
    public class FastSceneChangerEditorWindow : EditorWindow
    {
        public static FastSceneChangerEditorWindow GetWindowIsisOpen()
        {
            FastSceneChangerEditorWindow[] windows = Resources.FindObjectsOfTypeAll<FastSceneChangerEditorWindow>();
            if (windows != null && windows.Length > 0)
                return windows[0];

            return null;
        }

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
            selectedSceneIndex = -1;

            viewAllScene = false;
            allScenes = null;
            GC.Collect();
        }

        void OnEnable()
        {
            isGuiStyleInitedWhenProSkin = !EditorGUIUtility.isProSkin;
            InitEyeIcon();
        }

        void OnLostFocus()
        {
            selectedSceneIndex = -1;
        }

        void InitEyeIcon()
        {
            eyeIcon = EditorGUIUtility.FindTexture("ViewToolOrbit");
        }

        int selectedSceneIndex = -1;                        //Last click scene's index (if Last click in "All Scenes", plus Scenes In Build's Count)

        #region For double click check
        int lastClickedScene = -1;                     //Last click scene's index (if Last click in "All Scenes", plus Scenes In Build's Count) for double click
        float lastClickTime = 0;
        const float DoubleClickDelay = 0.25f;
        #endregion

        bool viewAllScene = false;
        List<EditorBuildSettingsScene> allScenes = null;

        #region GUI Styles
        bool isGuiStyleInitedWhenProSkin = false;
        GUIStyle topMenuStyle = null;
        GUIStyle viewOptionPopupStyle = null;
        GUIStyle sceneBoxStyle = null;

        GUIStyle sceneTextStyle = null;
        GUIStyle sceneSelectedTextStyle = null;
        #endregion

        #region Colors
        Color proTextColor = Color.clear;
        Color proDisableTextColor = Color.clear;

        Color proSelectBackground = Color.clear;
        Color proDisableSelectBackground = Color.clear;
        Color proLostFocusSelectBackground = Color.clear;
        Color proLostFocusDisableSelectBackground = Color.clear;

        Color stdTextColor = Color.clear;
        Color stdSelectTextColor = Color.clear;
        Color stdDisableTextColor = Color.clear;
        Color stdSelectDisableTextColor = Color.clear;

        Color stdSelectBackground = Color.clear;
        Color stdDisableSelectBackground = Color.clear;
        Color stdLostFocusSelectBackground = Color.clear;
        Color stdLostFocusDisableSelectBackground = Color.clear;
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
                sceneBoxStyle.border = new RectOffset(2, 2, 2, 2);
                sceneBoxStyle.stretchHeight = false;
                sceneBoxStyle.stretchWidth = true;
            }
            if (sceneTextStyle == null || isGuiStyleInitedWhenProSkin != EditorGUIUtility.isProSkin)
            {
                sceneTextStyle = new GUIStyle(EditorStyles.label);
                sceneTextStyle.padding = new RectOffset(10, 10, 2, 2);
                sceneTextStyle.margin = new RectOffset(1, 0, 0, 0);
                sceneTextStyle.active = sceneTextStyle.onActive = sceneTextStyle.normal = sceneTextStyle.onNormal;
            }
            if (sceneSelectedTextStyle == null || isGuiStyleInitedWhenProSkin != EditorGUIUtility.isProSkin)
            {
                sceneSelectedTextStyle = new GUIStyle(EditorStyles.label);
                sceneSelectedTextStyle.padding = new RectOffset(10, 10, 2, 2);
                sceneSelectedTextStyle.margin = new RectOffset(1, 0, 0, 0);
                sceneSelectedTextStyle.onNormal.textColor = Color.white;
                sceneSelectedTextStyle.active = sceneSelectedTextStyle.onActive = sceneSelectedTextStyle.normal = sceneSelectedTextStyle.onNormal;
            }

            isGuiStyleInitedWhenProSkin = EditorGUIUtility.isProSkin;
        }
        void InitColors()
        {
            proTextColor = (Color)new Color32(255, 255, 255, 255);
            proDisableTextColor = (Color)new Color32(255, 255, 255, 120);

            proSelectBackground = (Color)new Color32(62, 95, 150, 255);
            proDisableSelectBackground = (Color)new Color32(62, 95, 150, 200);

            proLostFocusSelectBackground = (Color)new Color32(255, 255, 255, 20);
            proLostFocusDisableSelectBackground = (Color)new Color32(255, 255, 255, 10);

            stdTextColor = (Color)new Color32(0, 0, 0, 255);
            stdSelectTextColor = (Color)new Color32(255, 255, 255, 255);
            stdDisableTextColor = (Color)new Color32(0, 0, 0, 120);
            stdSelectDisableTextColor = (Color)new Color32(255, 255, 255, 120);

            stdSelectBackground = (Color)new Color32(62, 125, 231, 255);
            stdDisableSelectBackground = (Color)new Color32(62, 125, 231, 200);

            stdLostFocusSelectBackground = (Color)new Color32(0, 0, 0, 80);
            stdLostFocusDisableSelectBackground = (Color)new Color32(0, 0, 0, 50);
        }
        private int nowSceneIndex = 0;

        void OnGUI()
        {
            InitColors();
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


            bool sceneClicked = false;

            scrollPosition = GUILayout.BeginScrollView(scrollPosition, (GUIStyle)"hostview");
            {
                nowSceneIndex = 0;
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
                        bool isActiveScene = (activeScene.path == scenes[i].path);

                        bool nowClicked = false;
                        DrawSceneButton(scenes[i], isActiveScene, defColor, out nowClicked);
                        sceneClicked = sceneClicked || nowClicked;
                    }
                    #endregion
                    GUILayout.Space(50);

                }
                GUILayout.EndVertical();

                if (viewAllScene)
                {
                    GUILayout.BeginVertical(sceneBoxStyle);
                    {
                        GUILayout.Label("All Scenes", (GUIStyle)"OL Title");
                        #region Show All Scenes

                        //Get build scenes
                        int scenesCount = allScenes == null ? 0 : allScenes.Count;

                        for (int i = 0; i < scenesCount; i++)
                        {
                            bool isActiveScene = (activeScene.path == allScenes[i].path);

                            bool nowClicked = false;
                            DrawSceneButton(allScenes[i], isActiveScene, defColor, out nowClicked);
                            sceneClicked = sceneClicked || nowClicked;
                        }
                        #endregion
                        GUILayout.Space(50);
                    }
                    GUILayout.EndVertical();
                }
                else
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(sceneBoxStyle.margin.left);
                        if (GUILayout.Button("View All Scenes"))
                        {
                            if (EditorUtility.DisplayDialog("View All Scenes", "View all scenes in assets folder.\r\nif project is big, this work may take some time at first scene searching.\r\nView all scenes?", "Yes", "No"))
                            {
                                viewAllScene = true;
                                allScenes = new List<EditorBuildSettingsScene>(LoadAllScenes());
                                allScenes.Sort(SceneCompare);
                            }
                        }
                        GUILayout.Space(sceneBoxStyle.margin.right);
                    }
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();

            if (Event.current.type == EventType.mouseDown && !sceneClicked)
            {
                selectedSceneIndex = -1;
            }


            GUI.color = defColor;
            Repaint();
        }

        void DrawSceneButton(EditorBuildSettingsScene scene, bool isActiveScene, Color defColor, out bool nowClicked)
        {
            nowClicked = false;

            #region get sceneName(path)
            string sceneName = "";
            switch (viewOption)
            {
                case 0: //Show full path (remove "ASSETS/")
                    sceneName = scene.path.Remove(0, 7);
                    break;
                case 1: //Show only scene name
                    sceneName = System.IO.Path.GetFileName(scene.path);
                    break;
            }
            #endregion

            #region draw scene text
            bool isNowSelectedScene = (selectedSceneIndex == nowSceneIndex);

            GUIContent sceneText = new GUIContent(sceneName);
            Rect sceneTextRect = GUILayoutUtility.GetRect(sceneText, sceneTextStyle);

            //Draw background when selected
            if (isNowSelectedScene)
            {
                if (focusedWindow == this)
                    GUI.color = EditorGUIUtility.isProSkin ? (scene.enabled ? proSelectBackground : proDisableSelectBackground) :
                                                         (scene.enabled ? stdSelectBackground : stdDisableSelectBackground);
                else
                    GUI.color = EditorGUIUtility.isProSkin ? (scene.enabled ? proLostFocusSelectBackground : proLostFocusDisableSelectBackground) :
                                                         (scene.enabled ? stdLostFocusSelectBackground : stdLostFocusDisableSelectBackground);
                GUI.DrawTexture(sceneTextRect, EditorGUIUtility.whiteTexture);
            }

            GUI.color = scene.enabled ? (EditorGUIUtility.isProSkin ? proTextColor : (isNowSelectedScene ? stdSelectTextColor : stdTextColor)) :
                                        (EditorGUIUtility.isProSkin ? proDisableTextColor : (isNowSelectedScene ? stdSelectDisableTextColor : stdDisableTextColor));

            GUIStyle nowSceneTextStyle = isNowSelectedScene ? sceneSelectedTextStyle : sceneTextStyle;
            nowSceneTextStyle.fontStyle = isActiveScene ? FontStyle.Bold : FontStyle.Normal;

            GUI.Label(sceneTextRect, sceneText, nowSceneTextStyle);
            #endregion

            #region when click
            Event e = Event.current;
            if (sceneTextRect.Contains(e.mousePosition))
            {
                if (e.type == EventType.mouseDown)
                {
                    nowClicked = true;
                    selectedSceneIndex = nowSceneIndex;
                }
                else if (e.type == EventType.mouseUp)
                {
                    if (e.button == 0)
                    {
                        if (lastClickedScene == nowSceneIndex && Time.realtimeSinceStartup - lastClickTime <= DoubleClickDelay)
                            SceneMenuCallback(new object[] { scene, SceneMenuType.Open });
                        else
                        {
                            lastClickedScene = nowSceneIndex;
                            lastClickTime = Time.realtimeSinceStartup;
                        }
                    }
                    else if (e.button == 1)
                    {
                        GenericMenu menu = new GenericMenu();
                        menu.AddItem(new GUIContent("Show in Project"), false, SceneMenuCallback, new object[] { scene, SceneMenuType.ShowInProject });
                        menu.AddItem(new GUIContent("Show in Explorer"), false, SceneMenuCallback, new object[] { scene, SceneMenuType.ShowInExplorer });
                        menu.AddSeparator("");
                        menu.AddItem(new GUIContent("Open"), false, SceneMenuCallback, new object[] { scene, SceneMenuType.Open });
                        menu.AddItem(new GUIContent("Open Scene Addtive"), false, SceneMenuCallback, new object[] { scene, SceneMenuType.ShowInAddtive });
                        menu.ShowAsContext();
                    }
                }
            }
            #endregion
            nowSceneIndex++;
            GUI.color = defColor;
        }

        enum SceneMenuType : int
        {
            ShowInProject = 0,
            ShowInExplorer = 1,
            Open = 2,
            ShowInAddtive = 3,
        }
        void SceneMenuCallback(object userData)
        {
            EditorBuildSettingsScene scene = (EditorBuildSettingsScene)((object[])userData)[0];
            SceneMenuType type = (SceneMenuType)((object[])userData)[1];

            string sceneFullPath = string.Concat(Application.dataPath, "/", scene.path.Remove(0, 7));
            bool isSceneExits = System.IO.File.Exists(sceneFullPath);
            if (!isSceneExits)
            {
                EditorUtility.DisplayDialog("Scene not found", string.Concat("Scene ", scene.path, " is not exists."), "Ok");
                return;
            }

            switch (type)
            {
                case SceneMenuType.ShowInProject:
                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(scene.path);
                    break;
                case SceneMenuType.ShowInExplorer:
                    EditorUtility.OpenWithDefaultApp(System.IO.Path.GetDirectoryName(scene.path));
                    break;
                case SceneMenuType.Open:
                    if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                        EditorSceneManager.OpenScene(scene.path);
                    break;
                case SceneMenuType.ShowInAddtive:
                    EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
                    break;
            }
        }

        EditorBuildSettingsScene[] LoadAllScenes()
        {
            string folderName = Application.dataPath;
            System.IO.DirectoryInfo dirInfo = new System.IO.DirectoryInfo(folderName);
            System.IO.FileInfo[] allFileInfos = dirInfo.GetFiles("*.unity", System.IO.SearchOption.AllDirectories);

            int count = allFileInfos.Length;
            EditorBuildSettingsScene[] scenes = new EditorBuildSettingsScene[count];

            int removeCount = Application.dataPath.Length - 6;
            for (int i = 0; i < count; i++)
            {
                scenes[i] = new EditorBuildSettingsScene(allFileInfos[i].FullName.Remove(0, removeCount), true);
            }
            return scenes;
        }

        public void SceneRemoved(string path)
        {
            if (!viewAllScene || allScenes == null)
                return;

            allScenes.RemoveAll(x => x.path == path);
            allScenes.Sort(SceneCompare);
        }
        public void SceneAdded(string path)
        {
            if (!viewAllScene || allScenes == null)
                return;

            if (allScenes.Find(x => x.path == path) == null)
                allScenes.Add(new EditorBuildSettingsScene(path, true));

            allScenes.Sort(SceneCompare);
        }

        public static int SceneCompare(EditorBuildSettingsScene x, EditorBuildSettingsScene y)
        {
            return -x.path.CompareTo(y.path);
        }
    }
}