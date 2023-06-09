using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public PlayerInputControl inputControl;
    private PhysicsCheck physicsCheck;
    private PlayerAnimation playerAnimation;
    private Character character;

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private CapsuleCollider2D coll;

    public Vector2 inputDirection; //存放获取的移动位置(x,y)
    [Header("基本参数")] private float speed = 290f;
    private float runSpeed; // 用于暂时存放speed
    private float walkSpeed => speed / 2.5f; //每次调用都执行 =>后面 
    private float jumpForce = 16.5f;
    private float hurtForce = 8; // 伤害的力
    public float wallJumpForce;
    public float slideDistance;
    public float slideSpeed;
    public int slidePowerCost;

    private Vector2 originalOffset;
    private Vector2 originalSize;

    [Header("物理材质")] public PhysicsMaterial2D normal;
    public PhysicsMaterial2D wall;

    [Header("状态")] public bool isCrouch; // 下蹲判定 是否为下蹲状态
    public bool isHurt; // 是否正在被伤害
    public bool isDead;
    public bool isAttack; //攻击判定
    public bool wallJump;
    public bool isSlide;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        coll = GetComponent<CapsuleCollider2D>();
        originalOffset = coll.offset;
        originalSize = coll.size;

        inputControl = new PlayerInputControl();
        physicsCheck = GetComponent<PhysicsCheck>();
        playerAnimation = GetComponent<PlayerAnimation>();
        character = GetComponent<Character>();

        // 跳跃
        inputControl.GamePlay.Jump.started += Jump;

        #region 强制走路

        runSpeed = speed;
        inputControl.GamePlay.WalkButtom.performed += ctx =>
        {
            if (physicsCheck.isGround)
                speed = walkSpeed;
        };

        inputControl.GamePlay.WalkButtom.canceled += ctx =>
        {
            if (physicsCheck.isGround)
                speed = runSpeed;
        };

        #endregion

        // 攻击
        inputControl.GamePlay.Attack.started += PlayerAttack;

        //滑铲
        inputControl.GamePlay.Slide.started += PlayerSlide;
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
        inputDirection = inputControl.GamePlay.Move.ReadValue<Vector2>();

        CheckState();
    }

    private void FixedUpdate()
    {
        if (!isHurt && !isAttack)
            Move();
    }

    private void Move()
    {
        // 人物移动
        if (!isCrouch && !wallJump)
            rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, rb.velocity.y);
        
        // if (inputDirection.x > 0)
        //     sr.flipX = false;
        // else if (inputDirection.x < 0)
        //     sr.flipX = true;
        // 人物翻转 
        int faceDir = (int)transform.localScale.x;
        //inputDirection.x 值为 -1 、 1
        if (inputDirection.x > 0)
        {
            faceDir = 1;
        }
        else if (inputDirection.x < 0)
        {
            faceDir = -1;
        }
        transform.localScale = new Vector3(faceDir, 1, 1);

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
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            //打断滑铲协程
            isSlide = false;
            StopAllCoroutines();
        }
        else if (physicsCheck.onWall)
        {
            rb.AddForce(new Vector2(-inputDirection.x, 2f) * wallJumpForce, ForceMode2D.Impulse);
            wallJump = true;
        }
    }

    /// <summary>
    /// 攻击
    /// </summary>
    private void PlayerAttack(InputAction.CallbackContext obj)
    {
        // 不在地面上，不需攻击.
        if (!physicsCheck.isGround)
            return;
        playerAnimation.PlayAttack();
        isAttack = true;
    }

    /// <summary>
    /// 滑铲
    /// </summary>
    private void PlayerSlide(InputAction.CallbackContext obj)
    {
        if (!isSlide && physicsCheck.isGround && character.currentPower >= slidePowerCost)
        {
            isSlide = true;
            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x,
                transform.position.y);
            
            StartCoroutine(TriggerSlide(targetPos));
            character.OnSlide(slidePowerCost);
        }
    }

    private IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {
            yield return null;
            if (!physicsCheck.isGround)
                break;

            if (physicsCheck.touchLeftWall && transform.localScale.x <0f || physicsCheck.touchRightWall&& transform.localScale.x >0f)
            {
                isSlide = false;
                break;
            }

            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed,
                transform.position.y));
        } while (Mathf.Abs(target.x - transform.position.x) > 0.1f);

        isSlide = false;
    }

    #region UnityEvent 人物受伤死亡

    /// <summary>
    /// 受伤后有一个反弹出去的力.
    /// </summary>
    public void GetHurt(Transform attacker)
    {
        // 开启人物受伤，人物就不能移动
        isHurt = true;
        // 先让人物停下来.
        rb.velocity = Vector2.zero;
        // 受伤方向确定，求X是正一(右)还是负一(左)，normalized用于将获得的数值趋近于0~1(防止数值过大).
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;
        // 添加 -> (力的方向 * 力的大小 , 瞬间的力)
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// 人物死亡
    /// </summary>
    public void PlayerDead()
    {
        isDead = true;
        // 关闭移动功能
        inputControl.GamePlay.Disable();
    }

    #endregion

    /// <summary>
    /// 检测状态更换材质
    /// </summary>
    private void CheckState()
    {
        // 在地上为TRUE → normal
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;

        if (isDead || isSlide)
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        else
            gameObject.layer = LayerMask.NameToLayer("Player");

        //在墙上速度减慢
        if (physicsCheck.onWall)
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2f);
        else
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);

        if (wallJump && rb.velocity.y < 0f)
        {
            wallJump = false;
        }
    }
}