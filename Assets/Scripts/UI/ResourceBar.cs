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
        moneyNumber += number;
        numberText.text = moneyNumber.ToString();

    }




    // Update is called once per frame
    void Update()
    {
        
    }
}
