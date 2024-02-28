using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Animator), typeof(PhysicsCheck))]
public class Enemy : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator animator;
    [HideInInspector] public PhysicsCheck physicsCheck;

    [Header("基本参数")]
    // 基本移动速度
    public float normalSpeed;
    // 追击速度
    public float chaseSpeed;
    // 当前速度
    [HideInInspector] public float currentSpeed;
    // 朝向
    public Vector3 faceDirect;
    public Transform attacker;
    public float hurtForce;
    // 出生坐标点
    public Vector3 spawnPoint;

    [Header("计时器")]
    public float waitingTime;
    public float waitingTimeCounter;
    public bool wait;

    // 丢失目标的时间
    public float lostTime;
    // 丢失目标的计时器
    public float lostTimeCounter;

    [Header("检测")]
    // 因为中心点在脚底，所以加一个偏移量
    public Vector2 centerOffset;
    // 检测大小
    public Vector2 checkSize;
    // 检测距离
    public float checkDistance;
    // 图层
    public LayerMask attackLayer;

    [Header("状态")]
    public bool isHurt;
    public bool isDead;

    // 当前状态
    private BaseState currentState;
    // 巡逻状态
    protected BaseState patrolState;
    // 追击状态
    protected BaseState chaseState;
    // 技能状态
    protected BaseState skillState;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        physicsCheck = GetComponent<PhysicsCheck>();
        // waitingTimeCounter = waitingTime;
        currentSpeed = normalSpeed;
        spawnPoint = transform.position;
    }

    private void OnEnable()
    {
        currentState = patrolState;
        currentState.OnEnter(this);
    }

    private void Update()
    {
        // 实时方向
        faceDirect = new Vector3(-transform.localScale.x, 0, 0);
        currentState.LogicUpdate();
        TimeCounter();
    }

    private void FixedUpdate()
    {
        currentState.PhysicsUpdate();
        if (!isHurt && !isDead && !wait)
        {
            Move();
        }
    }

    private void OnDisable()
    {
        currentState.OnExit();
    }

    public virtual void Move()
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("preMove") && !animator.GetCurrentAnimatorStateInfo(0).IsName("snailRecover"))
        {
            rb.velocity = new Vector2(currentSpeed * faceDirect.x * Time.deltaTime, rb.velocity.y);
        }
    }

    public void TimeCounter()
    {
        if (wait)
        {
            waitingTimeCounter -= Time.deltaTime;
            if (waitingTimeCounter < 0)
            {
                wait = false;
                waitingTimeCounter = waitingTime;
                transform.localScale = new Vector3(faceDirect.x, 1, 1);
            }
        }

        if (!FoundPlayer() && lostTimeCounter > 0)
        {
            lostTimeCounter -= Time.deltaTime;
        } else if (FoundPlayer())
        {
            lostTimeCounter = lostTime;
        }
    }

    public virtual bool FoundPlayer()
    {
        return Physics2D.BoxCast(transform.position + (Vector3)centerOffset, checkSize, 0, faceDirect, checkDistance, attackLayer);
    }

    public void SwitchState(NPCState state)
    {
        var newState = state switch
        {
            NPCState.Patrol => patrolState,
            NPCState.Chase => chaseState,
            NPCState.Skill => skillState,
            _ => null
        };
        currentState.OnExit();
        currentState = newState;
        currentState.OnEnter(this);
    }

    public virtual Vector3 GetNewPoint()
    {
        return transform.position;
    }

    public void OnTakeDamage(Transform attackTrans)
    {
        attacker = attackTrans;
        // 转身
        if (attackTrans.position.x - transform.position.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (attackTrans.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        // 受伤被击退
        isHurt = true;
        animator.SetTrigger("hurt");
        // 记录被攻击的方向
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;
        // 受伤时，无论x轴方向当前敌人的力有多少，都会造成击退
        rb.velocity = new Vector2(0, rb.velocity.y);
        StartCoroutine(OnHurt(dir));
    }

    private IEnumerator OnHurt(Vector2 dir)
    {
        // 施加瞬时力
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.45f);
        isHurt = false;
    }

    public void OnDie()
    {
        gameObject.layer = 2;
        animator.SetBool("dead", true);
        isDead = true;
    }

    public void DestroyAfterDeadAnimation()
    {
        Destroy(this.gameObject);
    }

    public virtual void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position + (Vector3)centerOffset + new Vector3(checkDistance * -transform.localScale.x, 0), 0.2f);
    }
}
