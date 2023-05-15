using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeChaseState : BaseState
{
    protected Attack attack;
    private Vector3 taget; //目标位置
    private Vector3 moverDir; //移动方向
    private bool isAttack;
    private float attackRateCounter = 0;

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        attack = enemy.GetComponent<Attack>();
        currentEnemy.anim.SetBool("chase", true);
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState((NPCState.Patrol));
        }

        taget = new Vector3(currentEnemy.attacker.position.x, currentEnemy.attacker.position.y + 1.5f, 0);
        //判断攻击距离
        if (Mathf.Abs(taget.x - currentEnemy.transform.position.x) <= attack.attackRange &&
            Mathf.Abs(taget.y - currentEnemy.transform.position.y) <= attack.attackRange)
        {
            //攻击
            isAttack = true;
            if (!currentEnemy.isHurt)
                currentEnemy.rb.velocity = Vector2.zero;

            //计时器
            attackRateCounter -= Time.deltaTime;
            if (attackRateCounter <= 0)
            {
                currentEnemy.anim.SetTrigger("attack");
                attackRateCounter = attack.attackRate;
            }
        }
        else //超出攻击范围
        {
            isAttack = false;
        }

        //获取移动的方向
        moverDir = (taget - currentEnemy.transform.position).normalized;

        //切换飞行的面朝方向
        if (moverDir.x > 0)
            currentEnemy.transform.localScale = new Vector3(-1, 1, 1);
        if (moverDir.x < 0)
            currentEnemy.transform.localScale = new Vector3(1, 1, 1);
    }

    public override void PhysicsUpdate()
    {
        //移动
        if (!currentEnemy.isHurt && !currentEnemy.isDead && !isAttack)
        {
            currentEnemy.rb.velocity = moverDir * (currentEnemy.currentSpeed * Time.deltaTime);
        }
    }

    public override void OnExit()
    {
        currentEnemy.anim.SetBool("chase", false);
    }
}