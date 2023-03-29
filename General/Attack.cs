using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    // 攻击伤害
    private int damage;
    // 攻击范围
    private float accackRange;
    // 攻击频率
    private float accackRate;

    public int Damage
    {
        get { return damage; }
        set { damage = value; }
    }

    public float AccackRange
    {
        get { return accackRange; }
        set { accackRange = value; }
    }

    public float AccackRate
    {
        get { return accackRate; }
        set { accackRate = value; }
    }

    public override string ToString()
    {
        return string.Format("攻击伤害：{0},攻击范围：{1},攻击频率:{2}", this.damage, this.accackRange, this.accackRate);
    }
}