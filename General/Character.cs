using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;

    [Header("受伤无敌")]
    public float invulnerableDuration; // 受伤后的初始无敌时间 用于重置
    [HideInInspector] public float invulnerableCounter;// 开启无敌时间后 不断减少的时间计数器
    public bool invulnerable;// 是否开启无敌

    public UnityEvent<Transform> OnTakeDamage;// 伤害事件
    public UnityEvent OnDie; //死亡事件

    private void Start()
    {
        currentHealth = maxHealth;
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

        if (currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            //执行受伤
            OnTakeDamage?.Invoke(attacker.transform);
            TriggerInvulnerable();
        }
        else
        {
            currentHealth = 0;
            //触发死亡
            OnDie?.Invoke();
        }
    }

    /// <summary>
    /// 触发无敌 初始化无敌状态
    /// </summary>
    private void TriggerInvulnerable()
    {
        // 重置无敌状态
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }
}