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
        EventManager.Instance.AddListener(CustomEventType.InteractObjectIn, ShowTips);
        EventManager.Instance.AddListener(CustomEventType.InteractObjectOut, HideTips);

    }
    void OnDestroy()
    {
        EventManager.Instance.RemoveListener(CustomEventType.InteractObjectIn, ShowTips);
        EventManager.Instance.RemoveListener(CustomEventType.InteractObjectOut, HideTips);
    }

    private void ShowTips()
    {
        if (tipsText != null) // ��� Text ����Ƿ�Ϊ��
        {

            tipsText.text = "��E������Boss��ս";
        }
        else
        {
            Debug.LogError("Text component is missing in ObjectInteractTips");

        }

    }
    private void HideTips()
    {
        if (tipsText != null) // ��� Text ����Ƿ�Ϊ��
        {

            tipsText.text = "";
        }
        else
        {
            Debug.LogError("Text component is missing in ObjectInteractTips");

        }

    }

}
