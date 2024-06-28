using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AimingZone : MonoBehaviour
{
    [HideInInspector]
    public GameObject aim;

    private float distance;

    private void OnEnable()
    {
        aim = null;
        distance = 999;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        var rst = (other.gameObject.transform.position - this.transform.position).magnitude;
        if (rst < distance)
        {
            aim = other.gameObject;
            distance = rst;
        }
    }
}
