using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class Knight : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float walkStopRate = 0.6f;
    public DetectionZone attackZone;

    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Animator animator;


    public enum WalkalbeDirection { Right, Left };

    private WalkalbeDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;

   public WalkalbeDirection WalkDirection
    {
        get { return _walkDirection; }
        set {
            if (_walkDirection != value) 
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1,gameObject.transform.localScale.y);

                if (value == WalkalbeDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                } else if (value == WalkalbeDirection.Left) 
                {
                    walkDirectionVector = Vector2.left;
                }
            }
            _walkDirection = value; }
    }

    public bool _hasTarget = false;

    public bool HasTarget { get { return _hasTarget; } private set 
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool canMove 
    {
        get 
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HasTarget = attackZone.dectectedColliders.Count > 0;
    }


    private void FixedUpdate()
    {

        if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection();
        }

        if (canMove)
            rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
        else
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x,0,walkStopRate), rb.velocity.y);

    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkalbeDirection.Right)
        {
            WalkDirection = WalkalbeDirection.Left;
        }
        else if (WalkDirection == WalkalbeDirection.Left)
        {
            WalkDirection = WalkalbeDirection.Right;
        }else
        {
            Debug.LogError("Current walkable direction is not set to legal values of right or left");
        }
    
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }


}
