using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerManager : MonoBehaviour
{
    private Character character;
    private PlayerAnimation playerAnimation;

    // 受伤后的初始无敌时间 用于重置
    private float invulnerableDuration;
    // 开启无敌时间后 不断减少的时间计数器
    private float invulnerableCounter;
    // 是否开启无敌
    private bool invulnerable;
    public UnityEvent<Transform> OnTakeDamage;

    void Awake()
    {
        character = GetComponent<Character>();

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
}
