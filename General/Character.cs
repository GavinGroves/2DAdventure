using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    // 最大血量
    private float maxHealth;
    // 当前血量
    private float currentHealth;

    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }

    public float CurrentHealth
    {
        get { return currentHealth; }
        set { currentHealth = value; }
    }

    public override string ToString()
    {
        return string.Format("最大血量：{0},当前血量{1}", this.maxHealth, this.currentHealth);
    }
}
