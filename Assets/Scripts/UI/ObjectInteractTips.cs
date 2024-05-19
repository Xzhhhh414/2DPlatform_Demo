using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class ObjectInteractTips : MonoBehaviour
{
    [SerializeField]
    public Text tipsText;
       

    void Start()
    {
        EventManager.Instance.AddListener<string>(CustomEventType.InteractObjectIn, ShowTips);
        EventManager.Instance.AddListener(CustomEventType.InteractObjectOut, HideTips);

    }
    void OnDestroy()
    {
        EventManager.Instance.RemoveListener<string>(CustomEventType.InteractObjectIn, ShowTips);
        EventManager.Instance.RemoveListener(CustomEventType.InteractObjectOut, HideTips);
    }

    private void ShowTips(string interactionName)
    {
        if (tipsText != null) // 检查 Text 组件是否为空
        {
            if (interactionName == "NpcGenerator")
            {
                tipsText.text = "按E键开启Boss挑战";
            }
            else if(interactionName == "OpenChest")
            {
                tipsText.text = "按E键打开宝箱";
            }


        }
        else
        {
            Debug.LogError("Text component is missing in ObjectInteractTips");

        }

    }
    private void HideTips()
    {
        if (tipsText != null) // 检查 Text 组件是否为空
        {

            tipsText.text = "";
        }
        else
        {
            Debug.LogError("Text component is missing in ObjectInteractTips");

        }

    }

}
