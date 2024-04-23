using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class TouchingDirections : MonoBehaviour
{

    public ContactFilter2D castFliter;
    public float groundDistance = 0.05f;
    public float wallDistance = 0.2f;
    public float ceilingDistance = 0.05f;


    BoxCollider2D touchingCol;
    Animator animator;

    RaycastHit2D[] groundHits = new RaycastHit2D[5];
    RaycastHit2D[] wallHits = new RaycastHit2D[5];
    RaycastHit2D[] ceilingHits = new RaycastHit2D[5];

    [SerializeField]
    private bool _isGrounded;

    public bool IsGrounded
    {
        get
        {
            return _isGrounded;
        }
        private set
        {
            _isGrounded = value;
            animator.SetBool(AnimationStrings.isGrounded, value);
        }
    }

    [SerializeField]
    private bool _isOnWall;

    public bool IsOnWall
    {
        get
        {
            return _isOnWall;
        }
        private set
        {
            _isOnWall = value;
            animator.SetBool(AnimationStrings.isOnwall, value);
        }
    }


    [SerializeField]
    private bool _isOnWCeiling;
    private Vector2 wallCheckDirection => gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;

    public bool IsOnCeiling
    {
        get
        {
            return _isOnWCeiling;
        }
        private set
        {
            _isOnWCeiling = value;
            animator.SetBool(AnimationStrings.isOnCeiling, value);
        }
    }


    private void Awake()
    {
        touchingCol = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }


    // Start is called before the first frame update
    void Start()
    {

    }

    public ContactFilter2D contactFilter;
    private Collider2D[] results = new Collider2D[5];
    enum CollisionDirection
    {
        Horiziontal = 1,
        Vertical,
    }// Update is called once per frame
    void FixedUpdate()
    {
        if (touchingCol.Cast(Vector2.down, castFliter, groundHits, groundDistance) > 0)
            IsGrounded = CollisionDetector(groundHits, CollisionDirection.Horiziontal);
        else
            IsGrounded = false;
        if (touchingCol.Cast(wallCheckDirection, castFliter, wallHits, wallDistance) > 0)
            IsOnWall = CollisionDetector(wallHits, CollisionDirection.Vertical);
        else
            IsOnWall = false;
        IsOnCeiling = touchingCol.Cast(Vector2.up, castFliter, ceilingHits, ceilingDistance) > 0;
    }
    private void LateUpdate()
    {
        Array.Clear(groundHits, 0, groundHits.Length);
        Array.Clear(wallHits, 0, wallHits.Length);
    }

    bool CollisionDetector(RaycastHit2D[] hits, CollisionDirection direction)
    {
        foreach (var hit in hits)
        {
            if (hit.collider != null)
            {
                var distance = touchingCol.Distance(hit.collider);
                if (distance.distance < -0.01)
                    continue;
                switch (direction)
                {
                    case CollisionDirection.Vertical:
                        if (hit.collider.tag == "One Way")
                        {
                            continue;
                        }
                        if (hit.normal.x == 1 || hit.normal.x == -1)
                        {
                            return true;
                        }
                        break;
                    case CollisionDirection.Horiziontal:

                        if (hit.normal.y > 0)
                        {
                            return true;
                        }
                        break;
                }
            }
        }
        return false;
    }

}
