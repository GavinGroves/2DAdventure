using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PhysicsCheck))]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator anim;
    [HideInInspector] public PhysicsCheck physicsCheck;

    [Header("基本参数")] public float normalSpeed; //默认速度
    public float chaseSpeed; //追击速度
    [HideInInspector] public float currentSpeed; //当前速度
    public Vector3 faceDir; //面朝方向
    public float hurtForce; //受伤朝反方向弹开的 力的增量
    public Transform attacker;
    public Vector3 spwanPoint;//初始位置

    [Header("追击范围检测")] public Vector2 centerOffset;
    public Vector2 checkSize;
    public float checkDistance;
    public LayerMask attackLayer;

    [Header("计时器")] public float waitTime;
    public float waitTimeCount;
    public bool wait;
    public float lostTime;
    public float lostTimeCounter;

    [Header("状态")] public bool isHurt;
    public bool isDead;

    private BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    protected BaseState skillState;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();

        currentSpeed = normalSpeed;
        // waitTimeCount = waitTime;
        spwanPoint = transform.position;
    }

    private void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    private void Update()
    {
        // 面朝方向 从 Scale.x 实时获取
        faceDir = new Vector3(-transform.localScale.x, 0, 0);

        currentState.LogicUpdate();

        TimeCount();
    }

    private void FixedUpdate()
    {
        currentState.PhysicsUpdate();
        if (!isHurt && !isDead && !wait)
            Move();
    }

    private void OnDisable()
    {
        currentState.OnExit();
    }

    public virtual void Move()
    {
        //蜗牛敌人的第一个动画片段不执行移动，到第二个才开始移动
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("snail_PerMove") &&
            !anim.GetCurrentAnimatorStateInfo(0).IsName("snail_Recover"))
            rb.velocity = new Vector2(currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
    }

    /// <summary>
    /// 计时器
    /// </summary>
    public void TimeCount()
    {
        //敌人碰到墙壁 停止一段时间 再转身
        if (wait)
        {
            waitTimeCount -= Time.deltaTime;
            if (waitTimeCount <= 0)
            {
                wait = false;
                waitTimeCount = waitTime;
                transform.localScale = new Vector3(faceDir.x, 1, 1);
            }
        }

        //检测到玩家切换状态，丢失玩家重置时间  
        if (!FoundPlayer() && lostTimeCounter > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        }
        else if (FoundPlayer()) // 添加这个额外的判断，在发现玩家的时候重置丢失时间
        {
            lostTimeCounter = lostTime;
        }
    }

    /// <summary>
    /// 检测敌人周围是否有player
    /// </summary>
    public virtual bool FoundPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0,
            faceDir, checkDistance, attackLayer);
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    /// <param name="state"></param>
    public void SwitchState(NPCState state)
    {
        var newState = state switch
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            NPCState.Skill => skillState,
            _ => null
        };

        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }

    public virtual Vector3 GetNewPoint()
    {
        return transform.position;
    }
    

    #region 事件执行方法

    /// <summary>
    /// 获得伤害
    /// </summary>
    /// <param name="attackTrans"></param>
    public void OnTakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;
        // 转身 , 判断玩家在左边还是右边攻击
        // 攻击者x - 敌人x 大于0正数值 代表 玩家在左边攻击敌人
        if (attackTrans.position.x - transform.position.x > 0)
            // 敌人转身
            transform.localScale = new Vector3(-1, 1, 1);
        if (attackTrans.position.x - transform.position.x < 0)
            transform.localScale = new Vector3(1, 1, 1);

        // 受伤被击退
        isHurt = true;
        anim.SetTrigger("hurt");
        // 记录攻击的方向
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;
        rb.velocity = new Vector2(0, rb.velocity.y);
        StartCoroutine(OnHurt(dir));
    }

    IEnumerator OnHurt(Vector2 dir)
    {
        // 受伤弹出的力
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;
    }

    public void OnDie()
    {
        gameObject.layer = 2;
        anim.SetBool("dead", true);
        isDead = true;
    }

    // 死亡销毁物体
    public void DestroyAfterAnimation()
    {
        Destroy(gameObject);
    }

    #endregion

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position + (Vector3)centerOffset + new Vector3(checkDistance * -transform.localScale.x, 0),
            0.1f);
    }
}