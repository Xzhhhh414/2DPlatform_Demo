using System.Collections;
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
    float duration = 0.1f;
    float currrntTime;
    float startTime;
    float endTime;
    private void OnEnable()
    {
        startTime = Time.time;
        endTime = startTime + duration;
    }

    private void Update()
    {
        if (Time.time > endTime)
        {
            GetComponent<CircleCollider2D>().enabled = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("GrabPoint"))
        {
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
        IsDecteted = false;
        colliders.Clear();
    }
}
