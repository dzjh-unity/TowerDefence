using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PathTool : ScriptableObject
{
    static PathNode m_pre = null;

    [MenuItem("PathNode/Create PathNode")]
    static void CreatePathNode() {
        // 创建一个新的路点
        GameObject go = new GameObject();
        go.AddComponent<PathNode>();
        go.name = "PathNode";
        go.tag = "PathNode";
        // 设置新创建的路点处于选中状态
        Selection.activeTransform = go.transform;
    }

    [MenuItem("PathNode/Set PreNode %q")]
    static void SetPreNode() {
        if (!Selection.activeGameObject || Selection.GetTransforms(SelectionMode.Unfiltered).Length > 1) {
            return;
        }
        if (Selection.activeGameObject.tag.CompareTo("PathNode") == 0) {
            m_pre = Selection.activeGameObject.GetComponent<PathNode>();
        }
    }

    [MenuItem("PathNode/Set NextNode %w")]
    static void SetNextNode() {
        if (!Selection.activeGameObject || Selection.GetTransforms(SelectionMode.Unfiltered).Length > 1) {
            return;
        }
        if (Selection.activeGameObject.tag.CompareTo("PathNode") == 0) {
            m_pre.SetNext(Selection.activeGameObject.GetComponent<PathNode>());
            m_pre = null;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
