using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage;
    public float attackRange;
    public float attackRate;

    private void OnTriggerStay2D(Collider2D other)
    {
        // ? 用于判断：该类中是否存在这个方法.
        other.GetComponent<Character>()?.TakeDamage(this);
    }
}