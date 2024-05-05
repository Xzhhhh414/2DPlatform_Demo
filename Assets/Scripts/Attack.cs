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
    #endregion

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

        //Debug.Log("OnTriggerEnter2D!!!!!");
        Damageable damageable = collision.GetComponent<Damageable>();
        if (damageable != null)
        {

            Vector2 deliveredKnockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            bool gotHit = damageable.Hit(_attackDamage, deliveredKnockback);

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
                if (hitFreezeXY)
                {
                    _hitFreezeXY = true;
                }
                //Debug.Log(collision.name + "hit for" + attackDamage);

                if (canClearCooldown && hitValid)
                {
                    Debug.Log("ClearCooldown Invoke");
                    hitValid = false;
                    lagLeftTime = lagDuration;
                    ClearCooldown.Invoke();
                }
            }
            else
            {
                // Debug.Log("No Damage");
            }
        }
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
    }
    private void FixedUpdate()
    {
        ImpulseScreen();
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
        if (IsInFrameRange((int)freezeXY.x, (int)freezeXY.y))
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
    bool IsInFrameRange(int startFrame, int endFrame)
    {
        currentClip = animator.GetCurrentAnimatorClipInfo(0)[0].clip;
        if (preClip == null || currentClip == null || !currentClip.name.Equals(preClip.name))
            return false;
        totalFrame = Mathf.RoundToInt(currentClip.length * currentClip.frameRate);
        var clipNormalizedTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        currentFrame = Mathf.RoundToInt(clipNormalizedTime % 1 * totalFrame);
        return currentFrame >= startFrame && currentFrame <= endFrame;
    }

    private void OnDisable()
    {
        _hitFreezeXY = false;
    }

}
