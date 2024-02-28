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

    [Header("��������")]
    // �����ƶ��ٶ�
    public float normalSpeed;
    // ׷���ٶ�
    public float chaseSpeed;
    // ��ǰ�ٶ�
    [HideInInspector] public float currentSpeed;
    // ����
    public Vector3 faceDirect;
    public Transform attacker;
    public float hurtForce;
    // ���������
    public Vector3 spawnPoint;

    [Header("��ʱ��")]
    public float waitingTime;
    public float waitingTimeCounter;
    public bool wait;

    // ��ʧĿ���ʱ��
    public float lostTime;
    // ��ʧĿ��ļ�ʱ��
    public float lostTimeCounter;

    [Header("���")]
    // ��Ϊ���ĵ��ڽŵף����Լ�һ��ƫ����
    public Vector2 centerOffset;
    // ����С
    public Vector2 checkSize;
    // ������
    public float checkDistance;
    // ͼ��
    public LayerMask attackLayer;

    [Header("״̬")]
    public bool isHurt;
    public bool isDead;

    // ��ǰ״̬
    private BaseState currentState;
    // Ѳ��״̬
    protected BaseState patrolState;
    // ׷��״̬
    protected BaseState chaseState;
    // ����״̬
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
        // ʵʱ����
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
        // ת��
        if (attackTrans.position.x - transform.position.x > 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (attackTrans.position.x - transform.position.x < 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        // ���˱�����
        isHurt = true;
        animator.SetTrigger("hurt");
        // ��¼�������ķ���
        Vector2 dir = new Vector2(transform.position.x - attackTrans.position.x, 0).normalized;
        // ����ʱ������x�᷽��ǰ���˵����ж��٣�������ɻ���
        rb.velocity = new Vector2(0, rb.velocity.y);
        StartCoroutine(OnHurt(dir));
    }

    private IEnumerator OnHurt(Vector2 dir)
    {
        // ʩ��˲ʱ��
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
