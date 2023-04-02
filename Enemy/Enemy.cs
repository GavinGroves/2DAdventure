using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    Rigidbody2D rb;
    [HideInInspector]public Animator anim;
    [HideInInspector]public PhysicsCheck physicsCheck;

    [Header("基本参数")] 
    public float normalSpeed; //默认速度
    public float chaseSpeed; //追击速度
    [HideInInspector]public float currentSpeed; //当前速度
    public Vector3 faceDir; //面朝方向
    public float hurtForce; //受伤朝反方向弹开的 力的增量

    public Transform attacker;

    [Header("计时器")] 
    public float waitTime;
    public float waitTimeCount;
    public bool wait;

    [Header("状态")] 
    public bool isHurt;
    public bool isDead;

    private BaseState currentState;
    protected BaseState patrolState;
    protected BaseState chaseState;
    
    protected  virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();

        currentSpeed = normalSpeed;
        waitTimeCount = waitTime;
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
        if (!isHurt & !isDead & !wait)
            Move();
        currentState.PhysicsUpdate();
    }

    private void OnDisable()
    {
        currentState.OnExit();
    }

    public virtual void Move()
    {
        rb.velocity = new Vector2(currentSpeed * faceDir.x * Time.deltaTime, rb.velocity.y);
    }

    /// <summary>
    /// 计时器，敌人碰到墙壁 停止一段时间 再转身
    /// </summary>
    public void TimeCount()
    {
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
    }

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
        Destroy(this.gameObject);
    }
}