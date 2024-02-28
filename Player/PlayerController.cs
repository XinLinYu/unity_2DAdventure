using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("�����¼�")]
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
    [Header("��������")]
    public float speed;
    public float jumpForce;
    public float wallJumpForce;
    // ����ʱ����ʩ�ӵ���
    public float hurtForce;
    // ��������
    public float slideDistance;
    public float slideSpeed;
    // ������������
    public int slidePowerCost;

    private float runSpeed;
    private float walkSpeed => speed / 3f;

    // ԭʼ����ײ��ƫ����
    private Vector2 originalOffset;
    // ԭʼ����ײ���С��ֵ
    private Vector2 originalSize;

    [Header("�������")]
    // ����Ħ��������ͨ����
    public PhysicsMaterial2D normal;
    // ������Ħ�����Ĺ⻬����
    public PhysicsMaterial2D wall;

    [Header("״̬")]
    public bool isCrouch;
    // �Ƿ����˵�״̬
    public bool isHurt;
    // �Ƿ�����״̬
    public bool isDead;

    public bool isAttack;
    // ��ǽ��״̬
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
        // ��ȡSpriteRenderer���
        spriteRenderer = GetComponent<SpriteRenderer>();
        // ��Ծ
        playerInputControl.Gameplay.Jump.started += Jump;

        #region ǿ����·
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

        // ����
        playerInputControl.Gameplay.Attack.started += PlayerAttack;

        // ����
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
            // ����������ײǽ
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
            // ��ϻ���Э��
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
    /// �����������������Ҳ���
    /// </summary>
    private void OnAfterLoadEvent()
    {
        playerInputControl.Gameplay.Enable();
    }

    /// <summary>
    /// ��������ʱ�������Ҳ���
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
        // ����x�������������ֵ
        float xDirection = inputDirection.x;
        // �����ƶ�
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
        // ���﷭ת
        transform.localScale = new Vector3(faceDir, 1, 1);

        /*// ���﷭ת����
        if (xDirection > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (xDirection < 0)
        {
            spriteRenderer.flipX = true;
        }*/

        // �¶�
        isCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;
        if (isCrouch)
        {
            // �޸���ײ�����С
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
        // ����ʱǿ��ʹ����ֹͣ�ƶ�
        rb.velocity = Vector2.zero;
        // �������x�����ȥ�����ߵ�x���꣬����õ�һ���������������ڹ�������ࡣ�����ڹ����ߵ��Ҳࡣ
        // �ò�õ����򣬳������˵������ɵõ����ٵ�Ч��
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;

        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }

    /// <summary>
    /// ����Ѿ���������ֹһ�в���
    /// </summary>
    public void PlayerDead()
    {
        isDead = true;
        playerInputControl.Gameplay.Disable();
    }

    // �����������������ǽ�壬��̬��������Ĳ���
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