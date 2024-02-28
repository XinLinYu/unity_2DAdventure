using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeePatrolState : BaseState
{
    // 巡逻的目标点
    private Vector3 targetPosition;
    // 巡逻方向
    private Vector3 movDir;

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
        // 获取新的点
        targetPosition = enemy.GetNewPoint();
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Chase);
        }
        // 判断是否到达了巡逻目的地的位置
        if (Mathf.Abs(targetPosition.x - currentEnemy.transform.position.x) < 0.1f 
            && Mathf.Abs(targetPosition.y - currentEnemy.transform.position.y) < 0.1f)
        {
            // 进入等待并获取出生点位置周围的随机点
            currentEnemy.wait = true;
            targetPosition = currentEnemy.GetNewPoint();
        }
        // 获取移动方向
        movDir = (targetPosition - currentEnemy.transform.position).normalized;
        if (!currentEnemy.wait)
        {
            // 切换面朝方向
            if (movDir.x > 0)
            {
                currentEnemy.transform.localScale = new Vector3(-1, 1, 1);
            } else if (movDir.x < 0)
            {
                currentEnemy.transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        if (!currentEnemy.wait && !currentEnemy.isHurt && !currentEnemy.isDead)
        {
            currentEnemy.rb.velocity = movDir * currentEnemy.currentSpeed * Time.deltaTime;
        }
        else
        {
            currentEnemy.rb.velocity = Vector2.zero;
        }
    }

    public override void OnExit()
    {
    }
}
