using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("监听事件")]
    public SceneLoadEventSO sceneLoadEvent;
    public VoidEventSO afterSceneLoadedEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO backToMenuEvent;

    public PlayerInputControl playerInputControl;
    private PlayerAnimation playerAnimation;
    private Rigidbody2D rb;
    private PhysicsCheck physicsCheck;
    private SpriteRenderer spriteRenderer;
    private CapsuleCollider2D capsuleCollider;
    public Vector2 inputDirection;
    [Header("基本参数")]
    public float speed;
    public float jumpForce;
    public float wallJumpForce;
    // 受伤时反弹施加的力
    public float hurtForce;
    // 滑铲距离
    public float slideDistance;
    public float slideSpeed;
    // 滑铲能量消耗
    public int slidePowerCost;

    private float runSpeed;
    private float walkSpeed => speed / 3f;

    // 原始的碰撞体偏移量
    private Vector2 originalOffset;
    // 原始的碰撞体大小数值
    private Vector2 originalSize;

    [Header("物理材质")]
    // 带有摩擦力的普通材质
    public PhysicsMaterial2D normal;
    // 不带有摩擦力的光滑材质
    public PhysicsMaterial2D wall;

    [Header("状态")]
    public bool isCrouch;
    // 是否受伤的状态
    public bool isHurt;
    // 是否死亡状态
    public bool isDead;

    public bool isAttack;
    // 蹬墙跳状态
    public bool wallJump;
    public bool isSlide;

    private void Awake()
    {
        
        rb = GetComponent<Rigidbody2D>();
        playerInputControl = new PlayerInputControl();
        physicsCheck = GetComponent<PhysicsCheck>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        playerAnimation = GetComponent<PlayerAnimation>();
        originalOffset = capsuleCollider.offset;
        originalSize = capsuleCollider.size;
        // 获取SpriteRenderer组件
        spriteRenderer = GetComponent<SpriteRenderer>();
        // 跳跃
        playerInputControl.Gameplay.Jump.started += Jump;

        #region 强制走路
        runSpeed = speed;
        playerInputControl.Gameplay.WalkButton.performed += ctx =>
        {
            if (physicsCheck.isGround)
            {
                speed = walkSpeed;
            }
        };

        playerInputControl.Gameplay.WalkButton.canceled += ctx =>
        {
            if (physicsCheck.isGround)
            {
                speed = runSpeed;
            }
        };
        #endregion

        // 攻击
        playerInputControl.Gameplay.Attack.started += PlayerAttack;

        // 滑铲
        playerInputControl.Gameplay.Slide.started += PlayerSlide;

        playerInputControl.Enable();
    }

    private void PlayerSlide(InputAction.CallbackContext context)
    {
        if (!isSlide && physicsCheck.isGround)
        {
            isSlide = true;
            var targetPosition = new Vector3(transform.position.x + slideDistance * transform.localScale.x, transform.position.y);
            gameObject.layer = LayerMask.NameToLayer("Enemy");
            StartCoroutine(TriggerSlide(targetPosition));
        }
    }

    private IEnumerator TriggerSlide(Vector3 target)
    {
        do
        {
            yield return null;
            if(!physicsCheck.isGround)
            {
                break;
            }
            // 滑动过程中撞墙
            if (physicsCheck.touchLeftWall && transform.localScale.x < 0f || physicsCheck.touchRightWall && transform.localScale.x > 0f)
            {
                isSlide = false;
                break;
            }
            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x * slideSpeed, transform.position.y));
        } while (Math.Abs(target.x - transform.position.x) > 0.1f);
        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }

    private void PlayerAttack(InputAction.CallbackContext context)
    {
        playerAnimation.PlayerAttack();
        isAttack = true;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
            // 打断滑铲协程
            StopAllCoroutines();
            isSlide = false;
        } else if(physicsCheck.onWall)
        {
            rb.AddForce(new Vector2(-inputDirection.x, 2f) * wallJumpForce, ForceMode2D.Impulse);
            wallJump = true;
        }
    }

    private void OnEnable()
    {
        sceneLoadEvent.LoadRequestEvent += OnLoadEvent;
        afterSceneLoadedEvent.OnEventRaised += OnAfterLoadEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
    }

    private void OnDisable()
    {
        playerInputControl.Disable();
        sceneLoadEvent.LoadRequestEvent -= OnLoadEvent;
        afterSceneLoadedEvent.OnEventRaised -= OnAfterLoadEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
    }

    private void OnLoadDataEvent()
    {
        isDead = false;
    }

    /// <summary>
    /// 场景加载完毕启用玩家操作
    /// </summary>
    private void OnAfterLoadEvent()
    {
        playerInputControl.Gameplay.Enable();
    }

    /// <summary>
    /// 场景加载时候禁用玩家操作
    /// </summary>
    /// <param name="arg0"></param>
    /// <param name="arg1"></param>
    /// <param name="arg2"></param>
    private void OnLoadEvent(GameSceneSO arg0, Vector3 arg1, bool arg2)
    {
        playerInputControl.Gameplay.Disable();
    }

    private void Update()
    {
        CheckState();
        inputDirection = playerInputControl.Gameplay.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        if(!isHurt && !isAttack)
        {
            Move();
        }
    }
    
    public void Move()
    {
        // 人物x轴正负朝向的数值
        float xDirection = inputDirection.x;
        // 人物移动
        if (isCrouch)
        {
            rb.velocity = new Vector2(0.0f, 0.0f);
        } else if(!isCrouch && !wallJump)
        {
            rb.velocity = new Vector2(xDirection * speed * Time.fixedDeltaTime, rb.velocity.y);
        }
        int faceDir = (int)transform.localScale.x;
        if (inputDirection.x > 0)
        {
            faceDir = 1;
        }
        else if (inputDirection.x < 0)
        {
            faceDir = -1;
        }
        // 人物翻转
        transform.localScale = new Vector3(faceDir, 1, 1);

        /*// 人物翻转（）
        if (xDirection > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (xDirection < 0)
        {
            spriteRenderer.flipX = true;
        }*/

        // 下蹲
        isCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;
        if (isCrouch)
        {
            // 修改碰撞体积大小
            capsuleCollider.offset = new Vector2(-0.05f, 0.80f);
            capsuleCollider.size = new Vector2(0.9f, 1.6f);
        } else
        {
            capsuleCollider.offset = originalOffset;
            capsuleCollider.size = originalSize;
        }
    }

    public void GetHurt(Transform attacker)
    {
        isHurt = true;
        // 受伤时强行使人物停止移动
        rb.velocity = Vector2.zero;
        // 用人物的x坐标减去攻击者的x坐标，如果得到一个负数，则人物在攻击者左侧。否则，在攻击者的右侧。
        // 用差得到方向，乘以受伤的力即可得到加速的效果
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;

        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// 玩家已经死亡，禁止一切操作
    /// </summary>
    public void PlayerDead()
    {
        isDead = true;
        playerInputControl.Gameplay.Disable();
    }

    // 根据人物解除地面或者墙体，动态更改人物的材质
    private void CheckState()
    {
        capsuleCollider.sharedMaterial = physicsCheck.isGround ? normal : wall;

        if(physicsCheck.onWall)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);
        } else
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }

        if(wallJump && rb.velocity.y < 0)
        {
            wallJump = false;
        }
    }
}