using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    public float walkSpeed;   
    public float airWalkSpeed;
    public float airDrag; //空中水平速度衰减系数
    public float airDragInDrifting; //钩爪飘逸状态下空中水平速度衰减系数
    public float jumpImpulse_OnGround;
    public float jumpImpulse_InAir;
    
    
    private int airJumpsLeft; // 记录剩余的空中跳跃次数
    private Vector2 moveInput;
    private bool isOnMoveHolding = false;
    private bool isOnAttackHolding = false;
    private float attackInputTimer = 0.0f;
    private float attackInputInterval = 0.2f;
    private Property prop;

    TouchingDirections touchingDirections;
    Damageable damageable;
    Attack attackSkill03;
    BoxCollider2D bCollider;

    Animator animator;
    Rigidbody2D rb;

    //skill03的技能逻辑
    private float dashSpeed = 75f; // 冲刺速度
    private float dashDuration = 0.1f; // 冲刺持续时间，单位秒
    private float dashTimeLeft; // 剩余冲刺时间
    private bool isDashing; // 是否正在冲刺
    private LayerMask wallLayerMask;  //冲刺检测的墙面layer
    private bool clearCDTrigger = false; //可清CD的触发器
    private float clearCDTriggerTimeLeft;//窗口时间的倒计时
    private float clearCDTriggerDuration = 2f;//命中后可继续使用技能的窗口时间
    private bool SettingSkill03CD = false;
    private float lagTimeLeft;//剩余释放技能后进CD的时间
    private float lagDuration = 0.1f;//释放技能后是否进CD的延迟时间
    private bool SettingSkill03StunLag = false;
    private float stunTimeLeft;//连续释放的间隔时间的倒计时
    private float stunTimeDuration = 0.2f;//连续释放的间隔时间
    private int clearCDTimeLeft; //当前轮技能清CD剩余次数
    private int clearCDMaxTime = 2; //当前轮技能清CD最大次数
    private bool hitDamage = false;//技能初始状态是没命中

    #region 特殊Y轴速度
    public float specialY = 0f;
    public bool isUsingSpecialY = false;
    #endregion
    #region 命中修改y轴
    private bool isHitModifyY = false;
    private float hitModifyY = 0;
    private List<Attack> attacks = new();
    #endregion
    
    AnimatorStateInfo stateInfo;

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
        get { return _IsMoving; }
        private set
        {
            _IsMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }


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

    public bool Skill03StunFinished
    {
        get
        {
            return animator.GetBool(AnimationStrings.skill03StunFinished);
        }
    }

    public bool CanAttack
    {
        get
        {
            return animator.GetBool(AnimationStrings.canAttack);
        }
    }
    public bool CanJump
    {
        get
        {
            return animator.GetBool(AnimationStrings.canJump);
        }
    }

    public bool CanGrab { get => animator.GetBool(AnimationStrings.canGrab); }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();

        GameObject skill03 = GameObject.Find("skill03_rush");
        if (skill03 == null)
        {
            Debug.Log("No skill03_rush found in the scene");
        }
        attackSkill03 = skill03.GetComponent<Attack>();

        attacks.AddRange(GetComponentsInChildren<Attack>());
        bCollider = GetComponent<BoxCollider2D>();
        distanceJoint2D = GetComponent<DistanceJoint2D>();
        grabDetection = GetComponentInChildren<GrabDetection>();
        lineRenderer = GetComponent<LineRenderer>();
        prop = this.GetComponent<Property>();
        grabbingHand = transform.Find("GrabbingHand");
    }
    private void Start()
    {
        wallLayerMask = LayerMask.GetMask("Ground");
        clearCDTimeLeft = clearCDMaxTime;
        airJumpsLeft = prop.MaxAirJumps; // 初始化剩余的空中跳跃次数
        distanceJoint2D.enabled = false;
        distanceJoint2D.autoConfigureDistance = false;
        //distanceJoint2D.anchor = grabbingHand.position;
        lineRenderer.positionCount = 40;
        lineRenderer.enabled = false;
        foreach (var attack in attacks)
        {
            attack.ModifyY += ModifyYOnHit;
        }
        detectionGradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.cyan, 0.0f), new GradientColorKey(Color.cyan, 0.5f), new GradientColorKey(Color.cyan, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(0.0f, 0.0f), new GradientAlphaKey(0.7f, 0.7f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        grabGradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.cyan, 0.0f), new GradientColorKey(Color.cyan, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
    }

    private void ModifyYOnHit(bool arg1, float arg2)
    {
        isHitModifyY = arg1;
        hitModifyY = arg2;
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

        if (SettingSkill03CD)
        {
            if (lagTimeLeft > 0)
            {
                lagTimeLeft -= Time.deltaTime;
            }
            else
            {
                if (clearCDTrigger && clearCDTimeLeft >= 0 && hitDamage)
                {
                    //Debug.Log("clearCDTrigger====" + clearCDTrigger);
                    //Debug.Log("clearCDTimeLeft====" + clearCDTimeLeft);
                    Skill03Cooldown = 0;
                    //skill03ClearCDSuccess.Invoke();
                    EventManager.Instance.TriggerEvent(CustomEventType.Skill03ClearCDSuccess);
                    lagTimeLeft = lagDuration;
                }
                else
                {
                    Skill03Cooldown = 5;
                    clearCDTimeLeft = clearCDMaxTime;
                    //SpellSkill03.Invoke();
                    EventManager.Instance.TriggerEvent(CustomEventType.SpellSkill03);
                    SettingSkill03CD = false;

                }


            }
        }


        if (clearCDTrigger)
        {

            if (clearCDTriggerTimeLeft > 0)
            {
                clearCDTriggerTimeLeft -= Time.deltaTime;

            }
            else
            {
                clearCDTrigger = false;

            }
        }


        if (SettingSkill03StunLag)
        {
            if (stunTimeLeft > 0)
            {
                stunTimeLeft -= Time.deltaTime;

            }
            else
            {
                animator.SetBool(AnimationStrings.skill03StunFinished, true);
                SettingSkill03StunLag = false;

            }
        }



        if (isOnMoveHolding)
        {
            //Debug.Log("OnMoveHolding~~~~~~~~~~~~~~~~~~~~");
            Moving(moveInput);
        }


        if (touchingDirections.IsGrounded)
        {
            airJumpsLeft = prop.MaxAirJumps; // 如果在地面上，重置空中跳跃次数
            isDrifting = false;
        }

        if (isOnAttackHolding)
        {
            attackInputTimer += Time.deltaTime;
            if (attackInputTimer >= attackInputInterval)
            {
                Attacking();
                attackInputTimer = 0.0f;
            }
  
        }


#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            BattleTestManager.Instance.GMTimeScale();
        }
#endif
    }
    private void FixedUpdate()
    {
        //CheckSlope();
        Grabable();
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

            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right * transform.localScale.x, 2f, wallLayerMask);
            if (hit.collider != null)
            {
                //Debug.Log("检测墙体生效！！！！！！！！");
                // 如果检测到墙体，则停止冲刺
                isDashing = false;
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
            else
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
        }

        else if (!isGrabbing)
        {
            if (touchingDirections.IsGrounded) //地面移动跳跃
            {
                rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);

            }
            else if (isDrifting) //使用钩爪后的飘逸阶段
            {
                CalcVelocityInAir(airDragInDrifting);
 
            }
            else //空中移动
            {
                CalcVelocityInAir(airDrag);
     
            }
      
            if (isUsingSpecialY)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * specialY, ForceMode2D.Impulse);
            }

            if (isHitModifyY)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0);
                rb.AddForce(Vector2.up * hitModifyY, ForceMode2D.Impulse);
                isHitModifyY = false;
            }

            animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);

        }

    }

    private void CalcVelocityInAir(float dragValue)
    {
        if (moveInput.x != 0) // 有水平输入
        {
            var tempVelocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
            if (rb.velocity.magnitude <= tempVelocity.magnitude || rb.velocity.x * moveInput.x < 0)//判断方向是否有变化，手动移动速度是不是大于现在的速度，不然就走惯性
            {
                rb.velocity = tempVelocity;
            }

            float targetSpeed = moveInput.x * CurrentMoveSpeed;
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, targetSpeed, dragValue), rb.velocity.y);
        }
        else // 没有水平输入，逐渐衰减 X 轴速度
        {
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, dragValue), rb.velocity.y);

            if (Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }

        }
    }

    private void OnEnable()
    {
        //attackSkill03.ClearCooldown.AddListener(TrigClearSkill03CD);
        EventManager.Instance.AddListener(CustomEventType.Skill03ClearCooldown, TrigClearSkill03CD);
    }

    private void OnDisable()
    {
        //attackSkill03.ClearCooldown.AddListener(TrigClearSkill03CD);
        EventManager.Instance.RemoveListener(CustomEventType.Skill03ClearCooldown, TrigClearSkill03CD);
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
        if (CanChangeDIR && !isGrabbing)
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


    [SerializeField]
    bool isJumping;
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && CanMove && CanJump && HaveJumpTimes())
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);

            if (touchingDirections.IsGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpImpulse_OnGround);
                //Debug.Log("rb.velocity====" + rb.velocity);
            }
            else
            {

                rb.velocity = new Vector2(rb.velocity.x, jumpImpulse_InAir);
                //Debug.Log("rb.velocity===="+ rb.velocity);
            }

            airJumpsLeft -= 1;
        }

    }

    private bool HaveJumpTimes()
    {

        return touchingDirections.IsGrounded || airJumpsLeft > 0;
    }



    public void OnAttack(InputAction.CallbackContext context)
    {
        //if (context.started)
        //{
        //  Attacking();
        //  isOnAttackHolding = true;
        //}

        if (context.performed)
        {
            Attacking();
            isOnAttackHolding = true;

        }

        if (context.canceled)
        {
            isOnAttackHolding = false;
            attackInputTimer = 0.0f;
        }
    }

    private void Attacking()
    {
        if (CanAttack)
        {
            animator.SetTriggerByTime(AnimationStrings.attackTrigger, 0.3f);
        }

    }

    public void OnSkill01(InputAction.CallbackContext context)
    {
        animator.ResetTrigger(AnimationStrings.skill01Release);

        if (context.started && Skill01Cooldown <= 0 && CanAttack)
        {
            animator.SetTrigger(AnimationStrings.skill01Tap);
            //SpellSkill01.Invoke();
            EventManager.Instance.TriggerEvent(CustomEventType.SpellSkill01);
        }

        if (context.canceled)
        {
            animator.SetTrigger(AnimationStrings.skill01Release);
            //Debug.Log("OnSkill01 Released====================================");

        }

    }

    public void OnSkill02(InputAction.CallbackContext context)
    {

        if (context.started && Skill02Cooldown <= 0 && CanAttack)
        {
            animator.SetTrigger(AnimationStrings.skill02Tap);
            //SpellSkill02.Invoke();
            EventManager.Instance.TriggerEvent(CustomEventType.SpellSkill02);
        }



    }

    public void OnSkill03(InputAction.CallbackContext context)
    {

        if (context.started && Skill03Cooldown <= 0 && Skill03StunFinished && CanAttack)
        {
            animator.SetTrigger(AnimationStrings.skill03Tap);

            StartDash();

            SetSkill03Cooldown();
            animator.SetBool(AnimationStrings.skill03StunFinished, false);

        }

    }


    private void StartDash()
    {
        isDashing = true;
        dashTimeLeft = dashDuration;
        rb.velocity = new Vector2(transform.localScale.x * dashSpeed, rb.velocity.y); // 冲刺保留Y轴速度
    }


    private void TrigClearSkill03CD()
    {
        hitDamage = true;
        clearCDTrigger = true;
        clearCDTriggerTimeLeft = clearCDTriggerDuration;
        clearCDTimeLeft -= 1;
    }

    private void SetSkill03Cooldown()
    {

        SettingSkill03CD = true;
        lagTimeLeft = lagDuration;
        SettingSkill03StunLag = true;
        stunTimeLeft = stunTimeDuration;
        hitDamage = false;

    }


    # region Grab
    public AnimationCurve animationCurve;
    public AnimationCurve ropeProgressionCurve;

    private DistanceJoint2D distanceJoint2D;
    private GrabDetection grabDetection;
    private Vector2 grabPosition = Vector2.negativeInfinity;
    private bool startGrab;
    private bool isGrabbing;
    private float grabDistance;
    [SerializeField, Label("钩爪射出的速度")]
    float grabSpeed = 1f;
    float progress = 0;//辅助计算绳子射出速度的值
    private GrabPoint grabPoint;
    private LineRenderer lineRenderer;
    private int resolution = 40;//绳子的总节点数
    [SerializeField, Label("绳子的曲度")]
    float startwaveRate = 4;
    float waveRate = 1;
    [SerializeField, Label("从曲绳到直绳的时间")]
    float ropeSetRope = 1;
    [SerializeField, Label("钩爪手")]
    Transform grabbingHand;
    Gradient detectionGradient = new();
    Gradient grabGradient = new();
    private bool isDrifting; //钩爪使用成功后的飘逸阶段


    void Grabable()
    {
        if (grabDetection.IsDecteted && !isGrabbing)
        {

            var tempDis = float.MaxValue;
            for (var i = 0; i < grabDetection.colliders.Count; i++)
            {
                grabDistance = Vector2.Distance(grabDetection.colliders[i].transform.position, grabbingHand.position);
                if (grabDistance < tempDis)
                {
                    tempDis = grabDistance;
                    grabPosition = grabDetection.colliders[i].transform.position;
                    grabPoint = grabDetection.colliders[i].GetComponent<GrabPoint>();
                }
            }
            if (IsInterruptGrab())
            {
                lineRenderer.enabled = false;
                startGrab = false;
                return;
            }
            lineRenderer.startWidth = 0.1f;
            lineRenderer.endWidth = 0.1f;
            // lineRenderer.colorGradient = detectionGradient;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, grabbingHand.position);
            lineRenderer.SetPosition(1, grabPosition);
            lineRenderer.enabled = true;

            if (!startGrab) return;
            lineRenderer.enabled = false;
            animator.SetTriggerByTime(AnimationStrings.grabTrigger, 1f);
            waveRate = startwaveRate;
            lineRenderer.positionCount = resolution;
            if (grabPosition != Vector2.negativeInfinity)
            {
                var dir = grabPosition - (Vector2)grabbingHand.position;
                if (dir.x > 0f)
                    IsFacingRight = true;
                else
                    IsFacingRight = false;
                isGrabbing = true;
                rb.gravityScale = 0;
                rb.velocity = Vector2.zero;
                distanceJoint2D.connectedAnchor = grabPosition;
                distanceJoint2D.anchor = transform.InverseTransformPoint(grabbingHand.position);
                distanceJoint2D.distance = grabDistance;
                distanceJoint2D.enabled = true;
            }
            else
            {
                isGrabbing = false;
                distanceJoint2D.enabled = false;
                // grabDetection.circleCollider2D.enabled = false;
                startGrab = false;
                rb.gravityScale = 4;
            }
        }
        else if (isGrabbing)
        {
            rb.gravityScale = 0;
            progress += Time.deltaTime;
            lineRenderer.startWidth = 3f;
            lineRenderer.endWidth = 3f;
            // lineRenderer.colorGradient = grabGradient;
            lineRenderer.enabled = true;
            waveRate -= Time.deltaTime * ropeSetRope;
            if (waveRate > 0)
                DrawCurveRope();
            else if (!IsInterruptGrab())
            {
                distanceJoint2D.distance = Mathf.Lerp(grabDistance, 0, progress * grabSpeed);
                waveRate = 0;
                DrawStraightLine();
            }
            else
            {
                isGrabbing = false;
                distanceJoint2D.enabled = false;
                // grabDetection.circleCollider2D.enabled = false;
                startGrab = false;
                lineRenderer.enabled = false;
                progress = 0;
                rb.gravityScale = 4;
            }

            var newDistance = Vector2.Distance(grabPosition, grabbingHand.position);
            if (newDistance <= 0.5f)
            {
                isGrabbing = false;
                distanceJoint2D.enabled = false;
                // grabDetection.circleCollider2D.enabled = false;
                startGrab = false;
                lineRenderer.enabled = false;
                progress = 0;
                var dir = grabPosition - (Vector2)grabbingHand.position;
                rb.AddForce(dir.normalized * grabPoint.Force, ForceMode2D.Impulse);
                rb.gravityScale = 4;
                animator.Play("player_falling");
                isDrifting = true;
            }
        }
        else
        {
            lineRenderer.enabled = false;
            startGrab = false;
        }
    }
    bool IsInterruptGrab()
    {
        Vector2[] boundsPoints = new[] {
            new Vector2(bCollider.bounds.min.x,bCollider.bounds.min.y),
            new Vector2(bCollider.bounds.min.x,bCollider.bounds.max.y),
            new Vector2(bCollider.bounds.max.x,bCollider.bounds.min.y),
            new Vector2(bCollider.bounds.max.x,bCollider.bounds.max.y),
        };
        foreach (var boundsPoint in boundsPoints)
        {
            Vector2 direction = grabPosition - boundsPoint;
            RaycastHit2D[] hits = Physics2D.RaycastAll(boundsPoint, direction, Vector2.Distance(grabPosition, boundsPoint));
            foreach (var hit in hits)
            {
                if (hit.collider != null)
                {
                    GameObject hitObject = hit.collider.gameObject;
                    if (hitObject.layer == LayerMask.NameToLayer("Ground") && !hitObject.CompareTag("One Way"))
                    {
                        return true;
                    }

                    if (hitObject.CompareTag("One Way"))
                    {
                        //Debug.Log("bCollider.bounds.min.y===" + bCollider.bounds.min.y + "hit.point.y===" + hit.point.y);
                        if (bCollider.bounds.min.y >= hit.point.y)  //玩家在软平台上方，不能用钩锁
                        {
                            return true;
                        }
                        
                    }
                }
            }

        }
        return false;
    }

    void DrawStraightLine()
    {
        if (lineRenderer.positionCount != 2) { lineRenderer.positionCount = 2; }
        lineRenderer.SetPosition(0, grabbingHand.position);
        lineRenderer.SetPosition(1, grabPosition);
    }

    void DrawCurveRope()
    {
        for (int i = 0; i < resolution; i++)
        {
            float x = (float)i / ((float)resolution - 1f);
            float y = animationCurve.Evaluate(x);
            Vector2 offset = Vector2.Perpendicular((grabPosition - (Vector2)grabbingHand.position).normalized * y) * waveRate;
            Vector2 tragetPosition = Vector2.Lerp(grabbingHand.position, grabPosition, x) + offset;
            Vector2 currentPosition = Vector2.Lerp(grabbingHand.position, tragetPosition, progress * grabSpeed);
            lineRenderer.SetPosition(i, currentPosition);
        }
    }

    public void OnGrabSope(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // grabDetection.circleCollider2D.enabled = true;
            if (!CanGrab) return;
            startGrab = true;
        }
    }

    #endregion

    void ReHit()
    {
        foreach (var attack in attacks)
        {
            attack.ReHitCurrentFrame();
        }
    }


    public void OnObjectInteract(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            EventManager.Instance.TriggerEvent(CustomEventType.AttemptInteractObject);
        }
    }


    [SerializeField, Label("被击飞时的击退倍率")]
    private float KnockBackRate = 1f;
    public void OnHit(int damage, Vector2 knockback, int knockbackLevel, int armorLevel)
    {
        if (knockbackLevel >= armorLevel)
        {
            rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y) * KnockBackRate;
            //Debug.Log(rb.velocity);
        }
    }

    #region 废弃
    #region 斜坡判定
    //Vector2 normalPerpRight;
    //Vector2 normalPerpLeft;
    //[Obsolete("游戏中没有斜坡，暂时不用")]
    //private void CheckSlope()
    //{
    //    Vector3 rightPoint = new Vector3(bCollider.bounds.max.x, bCollider.bounds.min.y);
    //    Vector3 leftPoint = new Vector3(bCollider.bounds.min.x, bCollider.bounds.min.y);

    //    RaycastHit2D hitRight = Physics2D.Raycast(rightPoint, Vector2.down, slopeDistance, wallLayerMask);
    //    RaycastHit2D hitLeft = Physics2D.Raycast(leftPoint, Vector2.down, slopeDistance, wallLayerMask);

    //    normalPerpRight = hitRight ? Vector2.Perpendicular(hitRight.normal).normalized : Vector2.zero;
    //    normalPerpLeft = hitLeft ? Vector2.Perpendicular(hitLeft.normal).normalized : Vector2.zero;
    //    Debug.DrawRay(hitRight.point, hitRight.normal, Color.red);
    //    Debug.DrawRay(hitLeft.point, hitLeft.normal, Color.red);

    //    Debug.DrawRay(hitRight.point, normalPerpRight, Color.green);
    //    Debug.DrawRay(hitLeft.point, normalPerpLeft, Color.green);

    //    RaycastHit2D hitFront = Physics2D.Raycast(rightPoint, Vector2.right, slopeDistance, wallLayerMask);
    //    RaycastHit2D hitBack = Physics2D.Raycast(leftPoint, -Vector2.right, slopeDistance, wallLayerMask);
    //    Debug.DrawRay(hitFront.point, hitFront.normal, Color.yellow);
    //    Debug.DrawRay(hitBack.point, hitBack.normal, Color.yellow);
    //    if (hitFront)
    //    {
    //        isOnSlope = true;
    //        normalPerp = Vector2.Perpendicular(hitFront.normal).normalized;
    //    }
    //    else if (hitBack)
    //    {
    //        isOnSlope = true;
    //        normalPerp = Vector2.Perpendicular(hitBack.normal).normalized;
    //    }
    //    else
    //    {
    //        isOnSlope = false;
    //    }

    //    if (!IsNormalVertical(hitRight.normal, Vector2.up) && hitRight.normal != Vector2.zero)
    //    {
    //        isOnSlope = true;
    //        normalPerp = normalPerpRight;
    //    }
    //    else if (!IsNormalVertical(hitLeft.normal, Vector2.up) && hitLeft.normal != Vector2.zero)
    //    {
    //        isOnSlope = true;
    //        normalPerp = normalPerpLeft;
    //    }
    //    else
    //    {
    //        isOnSlope = false;
    //    }
    //}
    //bool IsNormalVertical(Vector2 normal, Vector2 reference)
    //{
    //    float dotProduce = Vector2.Dot(normal.normalized, reference.normalized);
    //    float angle = Mathf.Acos(dotProduce) * Mathf.Rad2Deg;
    //    if (angle < 2)
    //    {
    //        return true;
    //    }
    //    return false;
    //}
    #endregion
    #endregion
}
