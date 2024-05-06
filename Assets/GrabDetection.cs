using System.Collections.Generic;
using UnityEngine;

public class GrabDetection : MonoBehaviour
{
    [HideInInspector]
    public bool IsDecteted
    {
        get
        {
            return colliders.Count > 0;
        }
        private set { }
    }
    [HideInInspector]
    public List<Collider2D> colliders = new List<Collider2D>();
    public CircleCollider2D circleCollider2D;
    float duration = 0.1f;
    private void Start()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        circleCollider2D.enabled = false;
    }

    private void Update()
    {
        if (circleCollider2D.enabled)
        {
            duration -= Time.deltaTime;
        }
        else
        {
            IsDecteted = false;
            colliders.Clear();
        }
        if (duration <= 0)
        {
            circleCollider2D.enabled = false;
            duration = 0.1f;
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("GrabPoint"))
        {
            if (!colliders.Contains(other))
                colliders.Add(other);
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("GrabPoint"))
        {
            if (colliders.Contains(other))
                colliders.Remove(other);
        }
    }
    private void OnDisable()
    {
    }
}
