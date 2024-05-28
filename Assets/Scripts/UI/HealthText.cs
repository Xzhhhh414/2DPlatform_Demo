using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class HealthText : MonoBehaviour
{
    public float moveSpeed;
    public float timeToFade;
    public GameObject followingObject;
    public Color textColor;

    private Vector3 offset = new Vector3(1f, 1.5f, 0); 
    private Vector3 worldPosition; // 储存初始的世界坐标位置

    //RectTransform textTransform;
    TextMeshProUGUI textMeshPro;

    private float timeElapsed;
    private Color startColor;

    private void Awake()
    {
        //textTransform = GetComponent<RectTransform>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
               
    }

    private void Start()
    {
        worldPosition = followingObject.transform.position + offset;
        textMeshPro.color = textColor;
        startColor = textMeshPro.color;
        textMeshPro.outlineWidth = 0.2f;
    }


    private void Update()
    {

        Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);
        transform.position = screenPosition;

        worldPosition += Vector3.up * moveSpeed * Time.deltaTime;

        timeElapsed += Time.deltaTime;
        if (timeElapsed < timeToFade)
        {
            float fadeAlpha = startColor.a * (1 - timeElapsed / timeToFade);
            textMeshPro.color = new Color(startColor.r, startColor.g, startColor.b, fadeAlpha);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
