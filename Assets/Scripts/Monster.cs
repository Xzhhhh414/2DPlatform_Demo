using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Monster : Character
{
    public float walkAcceleration = 3f;
    public float maxSpeed = 3f;
    public float walkStopRate = 0.6f;
    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;
    public GameObject healthBar;
    public Transform healthBarPosition;
    public Vector3 healthBarScale;
    
    //Rigidbody2D rb;


    public enum WalkalbeDirection { Right, Left };

    private WalkalbeDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;

    public WalkalbeDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if (value == WalkalbeDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if (value == WalkalbeDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }
            _walkDirection = value;
        }
    }

    public bool _hasTarget = false;

    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public float AttackCooldown
    {
        get
        {
            return animator.GetFloat(AnimationStrings.attackCooldown);
        }
        private set
        {
            animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
        }
    }

    private void Start()
    {
        EventManager.Instance.TriggerEvent<GameObject>(CustomEventType.MonsterSpawned, gameObject);
    }
    
    void Update()
    {
        HasTarget = attackZone.dectectedColliders.Count > 0;
        //Debug.Log("HasTarget ="+ HasTarget);
        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }

    }


    private void FixedUpdate()
    {

        if (touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection();
        }

        //if (!damageable.LockVelocity)
        {
            if (CanMove && touchingDirections.IsGrounded)
                rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x + (walkAcceleration * walkDirectionVector.x * Time.fixedDeltaTime), -maxSpeed, maxSpeed), rb.velocity.y);
            else
                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
        }
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
        }
        else
        {
            Debug.LogError("Current walkable direction is not set to legal values of right or left");
        }

    }
    

    public void OnCLiffDetected()
    {
        if (touchingDirections.IsGrounded)
        {
            FlipDirection();
        }
    }

}
