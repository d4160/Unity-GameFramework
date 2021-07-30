﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GraphProcessor;
using UnityEditor.Callbacks;
using System.IO;

namespace d4160.Editor.NodeGraphProcessor
{
    public class GraphAssetCallbacks
    {
        [MenuItem("Assets/Create/GraphProcessor", false, 10)]
        public static void CreateGraphPorcessor()
        {
            var graph = ScriptableObject.CreateInstance<BaseGraph>();
            ProjectWindowUtil.CreateAsset(graph, "GraphProcessor.asset");
        }

        [OnOpenAsset(0)]
        public static bool OnBaseGraphOpened(int instanceID, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID) as BaseGraph;

            if (asset != null) //&& AssetDatabase.GetAssetPath(asset).Contains("Examples")
            {
                EditorWindow.GetWindow<CustomGraphWindow>().InitializeGraph(asset as BaseGraph);
                return true;
            }
            return false;
        }
    }
}