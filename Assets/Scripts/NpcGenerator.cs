using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using FlowCanvas;


public class NpcGenerator : MonoBehaviour
{
    public FlowScript flowScript; // 对FlowScript对象的引用
    private bool canInteract = false;

    [SerializeField]
    private bool isOpened ;

    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isOpened = animator.GetBool("isOpen");
        EventManager.Instance.AddListener(CustomEventType.AttemptInteractObject, AttemptInteract);
    }


    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(CustomEventType.AttemptInteractObject, AttemptInteract);

    }

    private void Update()
    {
        if (isOpened)
        {
            
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //if (isOpened!)
            //{
                canInteract = true;
                EventManager.Instance.TriggerEvent(CustomEventType.InteractObjectIn);
            //}
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //if (isOpened!)
            //{
                canInteract = false;
                EventManager.Instance.TriggerEvent(CustomEventType.InteractObjectOut);
            //}

        }

    }


    public void AttemptInteract()
    {
        // 确保玩家与传送门处于交互状态
        if (canInteract)
        {
            animator.SetTrigger("Opening");
            SpawnBossInFlowCanvas();
        }
    }

    public void SpawnBossInFlowCanvas()
    {
        //flowScript.SendEvent(eventName);
        //Debug.Log("Boss Summoned!");

        NodeCanvas.Framework.Graph.SendGlobalEvent("SpwanBoss", 0, flowScript);
        isOpened = true;
    }
}
