using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bee : Enemy
{
    [Header("移动范围")]
    public float patrolRadius;

    protected override void Awake()
    {
        base.Awake();
        patrolState = new BeePatrolState();
        chaseState = new BeeChaseState();
    }

    public override bool FoundPlayer()
    {
        var obj = Physics2D.OverlapCircle(transform.position, checkDistance, attackLayer);
        if (obj)
        {
            attacker = obj.transform;
        }
        return obj;
    }

    public override void OnDrawGizmosSelected()
    {
        // 绘制检测玩家的区域
        Gizmos.DrawWireSphere(transform.position, checkDistance);
        Gizmos.color = Color.green;
        // 绘制巡逻区域
        Gizmos.DrawWireSphere(transform.position, patrolRadius);
    }

    public override Vector3 GetNewPoint()
    {
        // 在一个半径区域内随即找一个点进行来回巡逻
        var targetX = Random.Range(-patrolRadius, patrolRadius);
        var targetY = Random.Range(-patrolRadius, patrolRadius);
        return spawnPoint + new Vector3(targetX, targetY);
    }

    public override void Move()
    {
    }
}
