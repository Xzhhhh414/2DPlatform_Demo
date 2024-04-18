using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 5f;
    //public float runSpeed = 8f;
    public float airWalkSpeed = 3f;
    public float jumpImpulse = 10f;
    public Vector2 moveInput;
    public bool isOnMoveHolding = false;
    TouchingDirections touchingDirections;
    Damageable damageable;

    public UnityEvent SpellSkill01;
    public UnityEvent SpellSkill02;
    public UnityEvent SpellSkill03;

    //dash逻辑目前只为冲刺技能服务
    private float dashSpeed = 75f; // 冲刺速度
    private float dashDuration = 0.1f; // 冲刺持续时间，单位秒
    private float dashTimeLeft; // 剩余冲刺时间
    private bool isDashing; // 是否正在冲刺

    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                if (IsMoving && !touchingDirections.IsOnWall)
                {
                    if (touchingDirections.IsGrounded)
                    {

                        return walkSpeed;

                    }
                    else
                    {
                        return airWalkSpeed;
                    }
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }

        }
    }



    [SerializeField]
    private bool _IsMoving = false;

    public bool IsMoving
    {
        get
        {
            return _IsMoving;
        }
        private set
        {
            _IsMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }

    //[SerializeField]
    //private bool _isRunning = false;
    //public bool IsRunning
    //{
    //    get
    //    {
    //        return _isRunning;
    //    }
    //    set
    //    {
    //        _isRunning = value;
    //        animator.SetBool(AnimationStrings.isRunning, value);
    //    }
    //}

    public bool _isFacingRight = true;
    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {

            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }
            _isFacingRight = value;
        }
    }

    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);
        }

    }

    public float Skill01Cooldown
    {
        get
        {
            return animator.GetFloat(AnimationStrings.skill01Cooldown);
        }
        private set
        {
            animator.SetFloat(AnimationStrings.skill01Cooldown, Mathf.Max(value, 0));
        }
    }

    public float Skill02Cooldown
    {
        get
        {
            return animator.GetFloat(AnimationStrings.skill02Cooldown);
        }
        private set
        {
            animator.SetFloat(AnimationStrings.skill02Cooldown, Mathf.Max(value, 0));
        }
    }

    public float Skill03Cooldown
    {
        get
        {
            return animator.GetFloat(AnimationStrings.skill03Cooldown);
        }
        private set
        {
            animator.SetFloat(AnimationStrings.skill03Cooldown, Mathf.Max(value, 0));
        }
    }

    public bool CanChangeDIR
    {
        get
        {
            return animator.GetBool(AnimationStrings.canChangeDIR);
        }
    }

    public bool LockInAir
    {
        get
        {
            return animator.GetBool(AnimationStrings.lockInAir);
        }
    }


    Rigidbody2D rb;
    Animator animator;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Skill01Cooldown > 0)
        {
            Skill01Cooldown -= Time.deltaTime;
        }

        if (Skill02Cooldown > 0)
        {
            Skill02Cooldown -= Time.deltaTime;
        }

        if (Skill03Cooldown > 0)
        {
            Skill03Cooldown -= Time.deltaTime;
        }

        if (isOnMoveHolding)
        {
            //Debug.Log("OnMoveHolding~~~~~~~~~~~~~~~~~~~~");
            Moving(moveInput);
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            BattleTestManager.Instance.GMTimeScale();
        }
        

    }
    private void FixedUpdate()
    {
        if (LockInAir)
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionY | rb.constraints;
        }
        else
        {
            rb.constraints &= ~RigidbodyConstraints2D.FreezePositionY;
        }

        if (isDashing)
        {
            if (dashTimeLeft > 0)
            {
                rb.velocity = new Vector2(transform.localScale.x * dashSpeed, rb.velocity.y); // 维持冲刺速度
                dashTimeLeft -= Time.fixedDeltaTime; // 减少剩余冲刺时间
            }
            else
            {
                isDashing = false;
                rb.velocity = new Vector2(0, rb.velocity.y); // 冲刺结束后重置水平速度
            }
        }
        else
        {
            //if (!damageable.LockVelocity)
            //{
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
            //}

            animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);

        }


    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if (context.started)
        {
            //Debug.Log("started~~~~~~~~~~~~~~~~~~~~");
            Moving(moveInput);
            isOnMoveHolding = true;
        }

        if (context.performed)
        {
            isOnMoveHolding = true;
            //Debug.Log("performed~~~~~~~~~~~~~~~~~~~~");
        }

        if (context.canceled)
        {
            //Debug.Log("canceled~~~~~~~~~~~~~~~~~~~~");
            isOnMoveHolding = false;
            Moving(moveInput);
        }
    }

    private void Moving(Vector2 moveInput)
    {
        if (IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;

            SetFacingDirection(moveInput);

        }
        else
        {
            IsMoving = false;
        }

    }


    private void SetFacingDirection(Vector2 moveInput)
    {
        if (CanChangeDIR)
        {
            if (moveInput.x > 0 && !IsFacingRight)
            {
                IsFacingRight = true;
            }
            else if (moveInput.x < 0 && IsFacingRight)
            {
                IsFacingRight = false;
            }
        }

    }


    //public void OnRun(InputAction.CallbackContext context)
    //{
    //    if (context.started)
    //    {
    //        IsRunning = true;
    //    }
    //    else if (context.canceled)
    //    {
    //        IsRunning = false;
    //    }
    //}

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started && touchingDirections.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }

    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);

        }

    }

    public void OnSkill01(InputAction.CallbackContext context)
    {
        animator.ResetTrigger(AnimationStrings.skill01Release);

        if (context.started && Skill01Cooldown <= 0)
        {
            animator.SetTrigger(AnimationStrings.skill01Tap);
            SpellSkill01.Invoke();
        }

        if (context.canceled)
        {
            animator.SetTrigger(AnimationStrings.skill01Release);
            //Debug.Log("OnSkill01 Released====================================");

        }

    }

    public void OnSkill02(InputAction.CallbackContext context)
    {

        if (context.started && Skill02Cooldown <= 0)
        {
            animator.SetTrigger(AnimationStrings.skill02Tap);

            SpellSkill02.Invoke();
        }



    }

    public void OnSkill03(InputAction.CallbackContext context)
    {

        if (context.started && Skill03Cooldown <= 0)
        {
            animator.SetTrigger(AnimationStrings.skill03Tap);
            SpellSkill03.Invoke();

            StartDash();
        }

    }


    private void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, rb.velocity.y); // ������ҳ������ó�̷���
    }




    public void OnHit(int damage, Vector2 knockback)
    {
        //rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}
