using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowCanvas.Nodes;
using FlowCanvas;
using ParadoxNotion.Design;
using ParadoxNotion;



[Category("FP 2DAction")]
public class CountWithTag : FlowNode
{
 
    private ValueInput<string> tagName;



    public override void OnGraphStarted()
    {
        base.OnGraphStarted();
  
    }

    protected override void RegisterPorts()
    {
        tagName = AddValueInput<string>("tagName");

        AddValueOutput<int>("Value", () => CountGameObjectWithTag(tagName.value));

    }


    private int CountGameObjectWithTag(string tagName)
    {

        int currentCount = 0;
        GameObject[] tagPrefab = GameObject.FindGameObjectsWithTag(tagName);
        currentCount = tagPrefab.Length;

        return currentCount;

    }



}
