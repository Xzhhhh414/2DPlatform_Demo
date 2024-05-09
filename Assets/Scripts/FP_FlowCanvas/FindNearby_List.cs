using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FlowCanvas.Nodes;
using FlowCanvas;
using ParadoxNotion.Design;
using ParadoxNotion;
using System.Linq;



[Category("FP 2DAction")]
public class FindNearby_List : FlowNode
{
 
    private ValueInput<List<GameObject>> gameObjectList;
    private ValueInput<Vector3> targetPos;
    private ValueInput<int> count;



    public override void OnGraphStarted()
    {
        base.OnGraphStarted();
  
    }

    protected override void RegisterPorts()
    {
        gameObjectList = AddValueInput<List<GameObject>>("List");
        targetPos = AddValueInput<Vector3>("targetPos");
        count = AddValueInput<int>("count");


        AddValueOutput<List<GameObject>>("Value", () => GetNearestPos_List(gameObjectList.value, targetPos.value, count.value));

    }


    private List<GameObject> GetNearestPos_List(List<GameObject> gameObjectList, Vector3 targetPos, int count)
    {
        // 根据距离排序所有出生点
        List<GameObject> sortedList = gameObjectList.OrderBy(m => Vector3.Distance(m.transform.position, targetPos)).ToList();

        // 取出距离最近的 count 个出生点
        List<GameObject> nearestList = sortedList.Take(count).ToList();

        return nearestList;
    }


    //private List<GameObject> FindNearby_List(List<GameObject> gameObjectList, Vector3 targetPos, float maxDistance)
    //{
    //    List<GameObject> nearbygameObjectList = new List<GameObject>();

    //    foreach (GameObject newPos in gameObjectList)
    //    {
    //        float distance = Vector3.Distance(newPos.transform.position, targetPos);
    //        if (distance <= maxDistance)
    //        {
    //            nearbygameObjectList.Add(newPos);
    //        }
    //    }

    //    return nearbygameObjectList;
    //}


}
