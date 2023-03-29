using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private Character character;
    private Attack attack;
    void Start()
    {
        character = GetComponent<Character>();
        attack = GetComponent<Attack>();
        // 初始化野猪攻击 人物所受到的伤害数值
        attack.Damage = 5;
    }

    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ? 用于判断：该类中是否存在这个方法.
        other.GetComponent<PlayerManager>()?.TakeDamage(this.attack);
    }
}
