using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class trampoline : MonoBehaviour
{
    public float jumpForce = 1f;

    private void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.transform.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = (Vector2.up * jumpForce * 4);
        }
        else if (collision.transform.CompareTag("Monster"))
        { 
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = (Vector2.up * jumpForce * 2);
        }
    }
}
