using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private float checkRadius = 0.08f;
    private Vector2 bottomOffset;
    public bool isGround;
    private LayerMask groundLayer;

    private void Awake()
    {
        groundLayer = LayerMask.GetMask("Ground");
        bottomOffset = new Vector2(-0.05f, 0);
    }
    private void Update()
    {
        Check();
    }

    private void Check()
    {
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        // 绘制 虚线圆形
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRadius);
    }


}
