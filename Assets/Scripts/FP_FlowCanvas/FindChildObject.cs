using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowCanvas.Nodes;
using FlowCanvas;
using ParadoxNotion.Design;
using ParadoxNotion;

public class FindChildObject : FlowNode
{
 
    private ValueInput<GameObject> parentPrefab;
    private ValueInput<string> childNodeName;



    public override void OnGraphStarted()
    {
        base.OnGraphStarted();
  
    }

    protected override void RegisterPorts()
    {
        parentPrefab = AddValueInput<GameObject>("parentPrefab");
        childNodeName  = AddValueInput<string>("childNodeName");

        AddValueOutput<GameObject>("Value", () => FindChildByName(parentPrefab.value, childNodeName.value));

    }


    private GameObject FindChildByName(GameObject parentPrefab, string childNodeName)
    {
        if (parentPrefab != null)
        {
            Transform result = FindInChildren(parentPrefab.transform, childNodeName);
            return result != null ? result.gameObject : null;
        }
        return null;
    }

    private Transform FindInChildren(Transform parent, string childNodeName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childNodeName)
            {
                return child;
            }
            // ����ӽڵ����ӽڵ㣬�ݹ�����
            Transform result = FindInChildren(child, childNodeName);
            if (result != null)
            {
                return result;
            }
        }
        return null;
    }

    //private GameObject Call(GameObject parentPrefab, string childNodeName)
    //{
    //    if (parentPrefab != null)
    //    {
    //        Transform childTransform = parentPrefab.transform.Find(childNodeName);
    //        if (childTransform != null)
    //        {
    //            return childTransform.gameObject;
    //        }
    //    }

    //    return null; // ����Ҳ����ӽڵ㣬���� null
    //}
}
