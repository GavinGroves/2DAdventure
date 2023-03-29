using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;
    private PhysicsCheck physicsCheck;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CapsuleCollider2D coll;

    private Vector2 inputDirection;
    private float jumpForce = 16.5f;
    private float speed = 300f;
    private float runSpeed; // 用于暂时存放speed
    private float walkSpeed => speed / 2.5f; //每次调用都执行 =>后面 
    // 下蹲判定 是否为下蹲状态
    private bool isCrouch;
    public bool IsCrouch
    {
        get { return isCrouch; }
    }
    private Vector2 originalOffset;
    private Vector2 originalSize;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();

        coll = GetComponent<CapsuleCollider2D>();
        originalOffset = coll.offset;
        originalSize = coll.size;

        inputControl = new PlayerInputControl();
        physicsCheck = GetComponent<PhysicsCheck>();

        inputControl.Gmaeplay.Jump.started += Jump;

        #region 强制走路
        runSpeed = speed;
        inputControl.Gmaeplay.WalkButtom.performed += ctx =>
        {
            if (physicsCheck.isGround)
                speed = walkSpeed;
        };

        inputControl.Gmaeplay.WalkButtom.canceled += ctx =>
        {
            if (physicsCheck.isGround)
                speed = runSpeed;
        };
        #endregion
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    private void Update()
    {
        // 持续获取按键(手柄)输入的数值
        inputDirection = inputControl.Gmaeplay.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        // 人物移动
        if (!isCrouch)
            rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);

        // 人物翻转
        if (inputDirection.x > 0)
            sr.flipX = false;
        else if (inputDirection.x < 0)
            sr.flipX = true;

        // 下蹲
        isCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;
        if (isCrouch)
        {
            // 修改碰撞体大小和位移
            coll.offset = new Vector2(-0.1f, 0.835f);
            coll.size = new Vector2(0.65f, 1.67f);
        }
        else
        {
            // 还原之前碰撞体参数
            coll.offset = originalOffset;
            coll.size = originalSize;
        }

    }

    /// <summary>
    /// 跳跃方法
    /// </summary>
    private void Jump(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGround)
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }
}