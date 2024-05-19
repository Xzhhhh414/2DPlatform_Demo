using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class ResourceBar : MonoBehaviour
{
    [SerializeField]
    public Text numberText;
    private int moneyNumber = 0;

    // Start is called before the first frame update

    void Start()
    {
        numberText.text = moneyNumber.ToString();
        EventManager.Instance.AddListener<int>(CustomEventType.ResourceMoneyAdd, MoneyAdd);
    }


    void OnDestroy()
    {
        EventManager.Instance.RemoveListener<int>(CustomEventType.ResourceMoneyAdd, MoneyAdd);
    }


    private void MoneyAdd(int number)
    {
        if (number != null) // 检查 Text 组件是否为空
        {

            moneyNumber += number;
            numberText.text = moneyNumber.ToString();
        }
        else
        {
            Debug.LogError("Text component is missing in ResourceBar");

        }

    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
