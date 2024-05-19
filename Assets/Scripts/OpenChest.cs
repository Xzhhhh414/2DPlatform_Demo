using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using FlowCanvas;

public class OpenChest : MonoBehaviour
{
    public FlowScript selfFlowCanvas; // ×Ô¼ºµÄflowScript

    Animator animator;
    Animator animatorDropingEffect;

    private bool canInteract = false;
    private bool readyToSpawn = false;
    private float spwanWaitTime;

    [SerializeField]
    private GameObject droppingEffect;
    private bool isOpened = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        animatorDropingEffect = droppingEffect.GetComponent<Animator>();

        EventManager.Instance.AddListener(CustomEventType.AttemptInteractObject, AttemptInteract);
    }

    private void OnDestroy()
    {
        EventManager.Instance.RemoveListener(CustomEventType.AttemptInteractObject, AttemptInteract);

    }


    // Update is called once per frame
    void Update()
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
            EventManager.Instance.TriggerEvent<string>(CustomEventType.InteractObjectIn, "OpenChest");
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

        if (canInteract)
        {
            animator.SetTrigger("Opening");
            animatorDropingEffect.SetTrigger("Dropping") ;
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

        NodeCanvas.Framework.Graph.SendGlobalEvent("RandomResult", 0, selfFlowCanvas);
        readyToSpawn = false;
    }
}
