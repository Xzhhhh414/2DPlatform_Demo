using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using System;

public class Attack : MonoBehaviour
{
    public int attackDamage;
    public Vector2 knockback = Vector2.zero;


    public float stunRatio = 0.1f;
    private Animator animator;
    private Dictionary<Damageable, Coroutine> damageableCoroutines = new Dictionary<Damageable, Coroutine>();
    public bool canClearCooldown;
    public UnityEvent ClearCooldown;
    private float lagDuration = 0.2f;//加个计时，命中多个目标只算1次
    private float lagLeftTime;
    private bool hitValid = true;

    private CinemachineImpulseSource impulseSource;
    [SerializeField, Label("震屏发生的帧数")]
    private int impulseFrameIndex = -100;
    private int currentFrame;
    private int totalFrame;
    private AnimationClip currentClip;

    [Label("动作片段")]
    public AnimationClip preClip;
    [SerializeField, Label("震动速度")]
    Vector2 shakeVelocity;
    [SerializeField, Label("震动幅度"), Range(0, 100)]
    float shakeScpoe = 1;
    int _attackDamage;

    #region 命中修改y轴
    [SerializeField, Label("是否命中修改Y轴")]
    private bool hitModifyY = false;
    [SerializeField, Label("修改Y轴的值")]
    private float modifyY = 0;
    public Action<bool, float> ModifyY;
    #endregion

    #region 命中冻结XY轴
    [SerializeField, Label("是否命中冻结XY轴")]
    private bool hitFreezeXY = false;
    private bool _hitFreezeXY = false;
    [SerializeField, Label("冻结的帧数范围")]
    private Vector2 freezeXY = Vector2.zero;
    private Rigidbody2D rb;
    bool isInFrameRangeResultOfFreeze;
    #endregion

    #region ReHit
    [SerializeField]
    List<Damageable> beHitCharacterList = new();
    private int LastFrame = -1;
    #endregion

    int lastStateHash = 0;
    AnimatorStateInfo stateInfo;

    [SerializeField, Label("爆点特效")]
    public GameObject hitEffect;

    private Vector3 firstContactPoint = Vector3.zero; // 记录第一个接触点
    private Vector3 secondContactPoint = Vector3.zero; // 记录第一个接触点

    private void Start()
    {
        animator = GetComponentInParent<Animator>();
        rb = GetComponentInParent<Rigidbody2D>();
        if (impulseFrameIndex >= 0 && preClip != null)
        {
            impulseSource = this.AddComponent<CinemachineImpulseSource>();
            CinemachineImpulseDefinition definition = new()
            {
                m_ImpulseShape = CinemachineImpulseDefinition.ImpulseShapes.Bump,
                m_ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Uniform
            };
            impulseSource.m_ImpulseDefinition = definition;
        }
        _attackDamage = attackDamage;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
#if UNITY_EDITOR
        if (BattleTestManager.Instance.isMinDamage)
        {
            _attackDamage = 1;
        }
        else
        {
            _attackDamage = attackDamage;
        }
#endif
        firstContactPoint = collision.ClosestPoint(transform.position);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable == null) return;

        if (hitFreezeXY)
        {
            _hitFreezeXY = true;
        }

        if (beHitCharacterList.Contains(damageable)) return;
        Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

        secondContactPoint = collision.ClosestPoint(transform.position);
        Vector3 hitPosition = (firstContactPoint + secondContactPoint) / 2f;


        bool gotHit = damageable.Hit(_attackDamage, deliveredKnockback, hitEffect, hitPosition);
        if (gotHit)
        {
            if (damageableCoroutines.ContainsKey(damageable) && damageableCoroutines[damageable] != null)
            {
                StopCoroutine(damageableCoroutines[damageable]);
            }

            damageableCoroutines[damageable] = StartCoroutine(ChangAnimationSpeed(0f, stunRatio, animator, collision.GetComponent<Animator>()));
            if (hitModifyY)
            {
                ModifyY?.Invoke(hitModifyY, modifyY);
            }
            //Debug.Log(collision.name + "hit for" + attackDamage);
            if (canClearCooldown && hitValid)
            {
                // Debug.Log("ClearCooldown Invoke");
                hitValid = false;
                lagLeftTime = lagDuration;
                //ClearCooldown.Invoke();
                EventManager.Instance.TriggerEvent(CustomEventType.Skill03ClearCooldown);
            }


        }
        beHitCharacterList.Add(damageable);
    }

    IEnumerator ChangAnimationSpeed(float newSpeed, float duration, Animator myAnimator, Animator otherAnimator)
    {
        myAnimator.speed = newSpeed;
        otherAnimator.speed = newSpeed;
        yield return new WaitForSeconds(duration);
        otherAnimator.speed = 1;
        myAnimator.speed = 1;
    }

    private void Update()
    {
        if (!hitValid)
        {
            if (lagLeftTime > 0)
            {
                lagLeftTime -= Time.deltaTime;
            }
            else
            {
                hitValid = true;
                lagLeftTime = lagDuration;
            }
        }
        isInFrameRangeResultOfFreeze = IsInFrameRange((int)freezeXY.x, (int)freezeXY.y);
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.shortNameHash != lastStateHash)
        {
            beHitCharacterList.Clear();
            lastStateHash = stateInfo.shortNameHash;
        }
        UpdateLastFrame();
        ImpulseScreen();
    }
    private void FixedUpdate()
    {

        if (_hitFreezeXY)
            FreezeXY();
    }

    void ImpulseScreen()
    {
        if (impulseFrameIndex < 0 || !IsInFrameRange(impulseFrameIndex, impulseFrameIndex) || impulseSource == null)
            return;
        impulseSource.GenerateImpulse(shakeVelocity * shakeScpoe);
    }
    void FreezeXY()
    {
        if (isInFrameRangeResultOfFreeze)
        {
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
            rb.gravityScale = 4;
            _hitFreezeXY = false;
        }
    }
    int GetCurrentFrame()
    {
        currentClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        if (preClip == null || currentClip == null || !currentClip.name.Equals(preClip.name))
            return -1;
        totalFrame = Mathf.RoundToInt(currentClip.length * currentClip.frameRate);
        var clipNormalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        return Mathf.FloorToInt(clipNormalizedTime % 1 * totalFrame);
    }

    bool IsInFrameRange(int startFrame, int endFrame)
    {
        int currentFrame = GetCurrentFrame();
        if (currentFrame == -1)
            return false;
        return currentFrame >= startFrame && currentFrame <= endFrame;
    }

    void UpdateLastFrame()
    {
        LastFrame = GetCurrentFrame();
    }

    private void OnDisable()
    {
        //Debug.Log("Attack OnDisable");
        _hitFreezeXY = false;
        beHitCharacterList.Clear();
    }

    public void ReHitCurrentFrame()
    {
        beHitCharacterList.Clear();
    }

}
