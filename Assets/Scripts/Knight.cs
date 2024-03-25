using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class Knight : MonoBehaviour
{
    public float walkSpeed = 3f;

    Rigidbody2D rb;
    TouchingDirections touchingDirections;


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


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    private void FixedUpdate()
    {

        if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection();
        }
        rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
