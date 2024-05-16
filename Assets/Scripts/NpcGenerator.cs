using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ParadoxNotion.Design;
using FlowCanvas;


public class NpcGenerator : MonoBehaviour
{
    public FlowScript flowScript; // ��FlowScript���������
    Animator animator;
    private bool canInteract = false;
    private bool readyToSpawn = false;
    private float spwanWaitTime;

    [SerializeField]
    private bool isOpened = false;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        EventManager.Instance.AddListener(CustomEventType.AttemptInteractObject, AttemptInteract);
    }


    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(CustomEventType.AttemptInteractObject, AttemptInteract);

    }

    private void Update()
    {
        if (readyToSpawn)
        {
            if (spwanWaitTime > 0)
            {
                spwanWaitTime -= Time.deltaTime;
            }
            else
            {
                SpawnBossInFlowCanvas();

            }
        }

    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOpened)
        {
            canInteract = true;
            EventManager.Instance.TriggerEvent(CustomEventType.InteractObjectIn);
        }

    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isOpened)
        {
            canInteract = false;
            EventManager.Instance.TriggerEvent(CustomEventType.InteractObjectOut);

        }

    }

    
    public void AttemptInteract()
    {
        // ȷ������봫���Ŵ��ڽ���״̬
        if (canInteract)
        {
            animator.SetTrigger("Opening");
            EventManager.Instance.TriggerEvent(CustomEventType.InteractObjectOut);

            isOpened = true;
            spwanWaitTime = 1f;
            readyToSpawn = true;

        }
    }

    public void SpawnBossInFlowCanvas()
    {
        //flowScript.SendEvent(eventName);
        //Debug.Log("Boss Summoned!");

        NodeCanvas.Framework.Graph.SendGlobalEvent("SpwanBoss", 0, flowScript);
        readyToSpawn = false;
    }
}
