using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using FlowCanvas;


public class NpcGenerator : MonoBehaviour
{
    public FlowScript flowScript; // 对FlowScript对象的引用

    private bool canInteract = false;


    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddListener(CustomEventType.AttemptInteractObject, AttemptInteract);
    }


    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(CustomEventType.AttemptInteractObject, AttemptInteract);

    }



        private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = true;
            EventManager.Instance.TriggerEvent(CustomEventType.InteractObjectIn);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            canInteract = false;
            EventManager.Instance.TriggerEvent(CustomEventType.InteractObjectOut);
        }

    }
    public void AttemptInteract()
    {
        // 确保玩家与传送门处于交互状态
        if (canInteract)
        {
            SpawnBossInFlowCanvas();
        }
    }

    public void SpawnBossInFlowCanvas()
    {
        //flowScript.SendEvent(eventName);
        //Debug.Log("Boss Summoned!");
        NodeCanvas.Framework.Graph.SendGlobalEvent("SpwanBoss", 0, flowScript);
    }
}
