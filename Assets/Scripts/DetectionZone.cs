using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    public List<Collider2D> dectectedColliders = new List<Collider2D>();
    Collider2D col;



    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        dectectedColliders.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        dectectedColliders.Remove(collision);
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
