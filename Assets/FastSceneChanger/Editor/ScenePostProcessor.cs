using UnityEditor;
using UnityEngine;
using System.Collections;

/*
* Copyright 2016, HisaCat https://github.com/hisacat
* Released under the MIT license
*/
namespace FastSceneChanger
{
    public class ScenePostProcessor : AssetPostprocessor
    {
        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            int count = 0;

            count = importedAssets.Length;
            for (int i = 0; i < count; i++)
            {
                string asset = importedAssets[i];
                if (System.IO.Path.GetExtension(asset).ToLower() == ".unity")
                {
                    FastSceneChangerEditorWindow fastSceneChanger = FastSceneChangerEditorWindow.GetWindowIsisOpen();
                    if (fastSceneChanger != null)
                    {
                        fastSceneChanger.SceneAdded(asset);
                    }
                }
            }

            count = deletedAssets.Length;
            for (int i = 0; i < count; i++)
            {
                string asset = deletedAssets[i];
                if (System.IO.Path.GetExtension(asset).ToLower() == ".unity")
                {
                    FastSceneChangerEditorWindow fastSceneChanger = FastSceneChangerEditorWindow.GetWindowIsisOpen();
                    if (fastSceneChanger != null)
                    {
                        fastSceneChanger.SceneRemoved(asset);
                    }
                }
            }

            count = movedAssets.Length;
            for (int i = 0; i < count; i++)
            {
                string asset = movedAssets[i];
                string from = movedFromAssetPaths[i];

                if (System.IO.Path.GetExtension(asset).ToLower() == ".unity")
                {
                    FastSceneChangerEditorWindow fastSceneChanger = FastSceneChangerEditorWindow.GetWindowIsisOpen();
                    if (fastSceneChanger != null)
                    {
                        fastSceneChanger.SceneRemoved(from);
                        fastSceneChanger.SceneAdded(asset);
                    }
                }
            }

        }
    }
}