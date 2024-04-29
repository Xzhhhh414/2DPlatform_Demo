using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ParallaxLayer : MonoBehaviour
{
    public float parallaxFactorX;
    public float parallaxFactorY;

    public void MoveX(float deltaX)
    {
        Vector3 newPos = transform.localPosition;
        newPos.x -= deltaX * parallaxFactorX;
        transform.localPosition = newPos;
    }
    public void MoveY(float deltaY)
    {
        Vector3 newPos = transform.localPosition;
        newPos.y -= deltaY * parallaxFactorY;
        transform.localPosition = newPos;
    }
}
