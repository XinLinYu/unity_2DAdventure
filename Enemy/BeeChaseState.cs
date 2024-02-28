using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeeChaseState : BaseState
{
    private Attack attack;
    // 巡逻的目标点
    private Vector3 targetPosition;
    // 巡逻方向
    private Vector3 movDir;

    private bool isAttack;

    private float attackRateCounter = 0.0f;

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        attack = enemy.GetComponent<Attack>();
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;

        currentEnemy.animator.SetBool("chase", true);
    }

    public override void LogicUpdate()
    {
        // 丢失目标计时结束切换回巡逻状态
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
        }
        // 以敌人的坐标为目标点进行追击
        Vector3 position = currentEnemy.attacker.position;
        targetPosition = new Vector3(position.x, position.y + 1.5f, 0);
        // 当到达了攻击距离的位置，就停止移动，执行攻击动画
        if (Mathf.Abs(targetPosition.x - currentEnemy.transform.position.x) <= attack.attackRange
            && Mathf.Abs(targetPosition.y - currentEnemy.transform.position.y) <= attack.attackRange)
        {
            isAttack = true;
            if (!currentEnemy.isHurt)
            {
                currentEnemy.rb.velocity = Vector2.zero;
            }
            // 限制攻击频率
            attackRateCounter -= Time.deltaTime;
            if (attackRateCounter <= 0.0f)
            {
                attackRateCounter = attack.attackRate;
                // 播放攻击动画
                currentEnemy.animator.SetTrigger("attack");
            }
        } else
        {
            // 超出攻击范围
            isAttack = false;
        }
        // 获取移动方向
        movDir = (targetPosition - currentEnemy.transform.position).normalized;
        // 切换面朝方向
        if (movDir.x > 0)
        {
            currentEnemy.transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (movDir.x < 0)
        {
            currentEnemy.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public override void PhysicsUpdate()
    {
        if (!currentEnemy.isHurt && !currentEnemy.isDead && !isAttack)
        {
            currentEnemy.rb.velocity = movDir * (currentEnemy.currentSpeed * Time.deltaTime);
        }
    }

    public override void OnExit()
    {
        currentEnemy.animator.SetBool("chase", false);
    }
}
