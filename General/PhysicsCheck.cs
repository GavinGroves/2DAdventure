using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D coll;

    [Header("检测参数")] public bool manual; //true开启手动检测
    public float checkRaduis;
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;
    public LayerMask groundLayer;

    [Header("状态")] public bool isGround;
    public bool touchLeftWall;
    public bool touchRightWall;

    private void Awake()
    {
        // groundLayer = LayerMask.GetMask("Ground");
        // bottomOffset = new Vector2(-0.05f, 0);
        coll = GetComponent<CapsuleCollider2D>();

        // 不是手动检测时，开启自动设置点位
        if (!manual)
        {
            // 整体size.x值 加上偏移量 的一半 的位置
            rightOffset = new Vector2((coll.bounds.size.x + coll.offset.x) / 2, coll.bounds.size.y / 2);
            leftOffset = new Vector2(-rightOffset.x, rightOffset.y);
        }
    }

    private void Update()
    {
        Check();
    }

    private void Check()
    {
        // 检测地面
        // isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRaduis, groundLayer);
        isGround = Physics2D.OverlapCircle(
            (Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y),
            checkRaduis, groundLayer);

        // 墙体判断
        // touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRaduis, groundLayer);
        // touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRaduis, groundLayer);
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(leftOffset.x, leftOffset.y),
            checkRaduis, groundLayer);
        touchRightWall =
            Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(rightOffset.x, rightOffset.y),
                checkRaduis, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        // 绘制 虚线圆形
        // Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRaduis);
        // Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRaduis);
        // Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRaduis);
        Gizmos.DrawWireSphere(
            (Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y),
            checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(leftOffset.x, leftOffset.y), checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(rightOffset.x, rightOffset.y), checkRaduis);
    }
}