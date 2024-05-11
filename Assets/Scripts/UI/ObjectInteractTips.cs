using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInteractTips : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventManager.Instance.AddListener(CustomEventType.CanInteractObject, ShowTips);
        EventManager.Instance.AddListener(CustomEventType.CanInteractObject, HideTips);

    }

    private void HideTips()
    {
        throw new NotImplementedException();
    }

    private void ShowTips()
    {
        throw new NotImplementedException();
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
