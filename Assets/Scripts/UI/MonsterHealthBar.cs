using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MonsterHealthBar : MonoBehaviour
{
    public Slider healthSlider_Start; //�ܵ��˺�ǰ��Ѫ��
    public Slider healthSlider_Current;//��ǰ��Ѫ��
    public GameObject followingObject;
    private Monster monster;
    private RectTransform rectTransform;
    private float showTime = 3f;
    private float showTimeLeft;
    private Property prop;
    public float updateSpeed = 0.1f; // Ѫ�������ٶ�


    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

    }
    void Start()
    {
        Init();
    }

    private void Init()
    {
        monster = followingObject.GetComponent<Monster>();
        prop = monster.GetComponent<Property>();
    }

    void Update()
    {
        if (followingObject != null)
        {
            // ��ȡ���������λ��
            Vector3 worldPosition = monster.healthBarPosition.position;
            // ������λ��ת��Ϊ��Ļλ��
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            // ����Ѫ���� UI λ��
            rectTransform.position = screenPosition;
        }
        else if (followingObject != null)
        {
            // ���û������ healthBarPosition����ʹ�ù���� Transform
            Vector3 worldPosition = followingObject.transform.position;
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
            rectTransform.position = screenPosition;
        }

        if (showTimeLeft > 0)
        {
            showTimeLeft -= Time.deltaTime;
        }
        else
        {
            gameObject.SetActive(false);
            showTimeLeft = 0;
            
        }

        if (prop.CurrentHp <= 0)
        {
            
            Destroy(gameObject);
        }

    }

    public void GetDamaged(int damageReceived)
    {
        gameObject.SetActive(true);
        if (followingObject == null || monster == null || prop == null)
        {
            Init();
        }

        healthSlider_Current.value = CalculateSliderPercentage(prop.CurrentHp, prop.MaxHp);
        //Debug.Log("CurrentHp==" + CurrentHp + ",        MaxHp==" + MaxHp) ;
        //Debug.Log(" healthSlider_Current.value==" + healthSlider_Current.value);

        int hpStart = prop.CurrentHp + damageReceived;
        healthSlider_Start.value = CalculateSliderPercentage(hpStart, prop.MaxHp);
        StartCoroutine(SmoothlyUpdatePreviousHealthBar(healthSlider_Current.value));

        showTimeLeft = showTime;

    }


    private float CalculateSliderPercentage(int currentHealth, int maxHealth)
    {
        return (float)currentHealth / maxHealth;
    }
    private IEnumerator SmoothlyUpdatePreviousHealthBar(float targetValue)
    {
        while (healthSlider_Start.value > targetValue)
        {
            healthSlider_Start.value -= updateSpeed * Time.deltaTime;
            yield return null;
        }
        healthSlider_Start.value = targetValue;
    }
}
