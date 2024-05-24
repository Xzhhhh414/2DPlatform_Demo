using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class HealthText : MonoBehaviour
{
    public float moveSpeed = 125f;
    public float timeToFade = 1f;
    public GameObject followingObject;

    RectTransform textTransform;
    TextMeshProUGUI textMeshPro;

    private float timeElapsed;
    private Color startColor;

    private void Awake()
    {
        textTransform = GetComponent<RectTransform>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
        startColor = textMeshPro.color;
    }

    private void Update()
    {
        //if (followingObject != null)
        //{
        //    Vector3 screenPosition = Camera.main.WorldToScreenPoint(followingObject.transform.position );
        //    textTransform.position = screenPosition;

        //    textTransform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
        //}
        //else
        //{
        //    textTransform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
        //}


        textTransform.position += new Vector3(0, moveSpeed * Time.deltaTime, 0);
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
