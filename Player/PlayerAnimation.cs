using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator m_Animator;
    private Rigidbody2D m_Rigidbody2d;
    private PhysicsCheck physicsCheck;
    private PlayerController playerController;

    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
        m_Rigidbody2d = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        SetAnimation();
    }

    public void SetAnimation()
    {
        // 走跑动画 传递速度绝对值
        m_Animator.SetFloat("VelocityX", Mathf.Abs(m_Rigidbody2d.velocity.x));
        // jump动画传递参数
        m_Animator.SetFloat("VelocityY", m_Rigidbody2d.velocity.y);
        // 检测人物是否在地面上
        m_Animator.SetBool("isGround", physicsCheck.isGround);
        // 下蹲
        m_Animator.SetBool("isCrouch", playerController.isCrouch);
        // 人物死亡
        m_Animator.SetBool("isDeath", playerController.isDead);
        // 人物攻击
        m_Animator.SetBool("isAttack", playerController.isAttack);
        // 人物是否靠着墙,在墙上
        m_Animator.SetBool("onWall",physicsCheck.onWall);
        //滑铲
        m_Animator.SetBool("isSlide",playerController.isSlide);
    }

    public void PlayHurt()
    {
        m_Animator.SetTrigger("hurt");
    }

    public void PlayAttack()
    {
        m_Animator.SetTrigger("attack");
    }
}