using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeePatrolState : BaseState
{
    private Vector3 taget; //目标位置
    private Vector3 moverDir; //移动方向

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
        //每次调用返回新的点，回到巡逻的范围当中
        taget = enemy.GetNewPoint();
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState((NPCState.Chase));
        }

        //判断 x y 是否达到指定位置 -> 进入等待时间 -> 产生新点
        if (Mathf.Abs(taget.x - currentEnemy.transform.position.x) < 0.1f &&
            Mathf.Abs(taget.y - currentEnemy.transform.position.y) < 0.1f)
        {
            currentEnemy.wait = true;
            taget = currentEnemy.GetNewPoint();
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
        if (!currentEnemy.wait && !currentEnemy.isHurt && !currentEnemy.isDead)
        {
            currentEnemy.rb.velocity = moverDir * (currentEnemy.currentSpeed * Time.deltaTime);
        }
        else
        {
            currentEnemy.rb.velocity = Vector2.zero;
        }
    }

    public override void OnExit()
    {
    }
}