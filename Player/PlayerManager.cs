using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    private Character character;
    private PlayerAnimation playerAnimation;
    private Rigidbody2D rb;
    private PlayerController playerController;
    private float invulnerableDuration;  // 受伤后的初始无敌时间 用于重置
    private float invulnerableCounter;   // 开启无敌时间后 不断减少的时间计数器
    private bool invulnerable;  // 是否开启无敌
    private bool isHurt; // 是否正在被伤害
    public bool IsHurt
    {
        get { return isHurt; }
        set { isHurt = value; }
    }
    private bool isDead;
    public bool IsDead
    {
        get { return isDead; }
        set { isDead = value; }
    }
    private float hurtForce = 8; // 伤害的力

    public UnityEvent<Transform> OnTakeDamage;// 伤害事件
    public UnityEvent OnDeath; //死亡事件

    void Awake()
    {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();

        // 初始化人物血量
        character.MaxHealth = 100;
        character.CurrentHealth = character.MaxHealth;
        // 受到伤害后的初始无敌时间
        invulnerableDuration = 2.0f;
    }

    void Update()
    {
        // 开启无敌时间
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }
    }

    /// <summary>
    /// 碰到野猪获得伤害值
    /// </summary>
    public void TakeDamage(Attack attacker)
    {
        if (invulnerable)
            return;

        if (character.CurrentHealth - attacker.Damage > 0)
        {
            character.CurrentHealth -= attacker.Damage;
            Triggerinvulnerable();
            // 执行受伤
            OnTakeDamage?.Invoke(attacker.transform);
        }
        else
        {
            // 触发死亡
            character.CurrentHealth = 0;
            OnDeath.Invoke();
        }
    }

    /// <summary>
    /// 触发无敌 初始化无敌状态
    /// </summary>
    private void Triggerinvulnerable()
    {
        // 重置无敌状态
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }
    #region UnityEvent
    /// <summary>
    /// 受伤后有一个反弹出去的力.
    /// </summary>
    public void GetHurt(Transform attacker)
    {
        // 开启人物受伤，人物就不能移动
        isHurt = true;
        // 先让人物停下来.
        rb.velocity = Vector2.zero;
        // 受伤方向确定，求X是正一(右)还是负一(左)，normalized用于将获得的数值趋近于0~1(防止数值过大).
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;
        // 添加 -> (力的方向 * 力的大小 , 瞬间的力)
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// 人物死亡
    /// </summary>
    public void PlayerDead()
    {
        isDead = true;
        // 关闭移动功能
        playerController.inputControl.Gmaeplay.Disable();
    }
    #endregion
}
