#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace LegacyNetworking.Editor
{
    [CustomEditor(typeof(NetworkView))]
    public class NetworkViewEditor : UnityEditor.Editor
    {
        private static bool renderData = false;
        public override void OnInspectorGUI() {
            var target = (NetworkView)this.target;
            EditorGUILayout.BeginVertical("HelpBox");
            EditorGUI.BeginDisabledGroup(true);
            renderData = EditorGUILayout.Foldout(renderData, "Network View Data");
            if (renderData)
                RenderData(target);
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndVertical();
            base.OnInspectorGUI();
        }

        private static void RenderData(NetworkView target) {
            EditorGUILayout.IntField("View ID", target.viewId);
            EditorGUILayout.IntField("Owner", target.owner);
            EditorGUILayout.TextField("Instantiate Key", target.instantiateKey != null ? target.instantiateKey.ToString() : "NONE");
            EditorGUILayout.Toggle("Is Instantiated", target.isInstantiated);
            EditorGUILayout.Toggle("Is Mine", target.isMine);
        }
    }
}
#endif