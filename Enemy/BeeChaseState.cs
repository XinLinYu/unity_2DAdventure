using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BeeChaseState : BaseState
{
    private Attack attack;
    // Ѳ�ߵ�Ŀ���
    private Vector3 targetPosition;
    // Ѳ�߷���
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
        // ��ʧĿ���ʱ�����л���Ѳ��״̬
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
        }
        // �Ե��˵�����ΪĿ������׷��
        Vector3 position = currentEnemy.attacker.position;
        targetPosition = new Vector3(position.x, position.y + 1.5f, 0);
        // �������˹��������λ�ã���ֹͣ�ƶ���ִ�й�������
        if (Mathf.Abs(targetPosition.x - currentEnemy.transform.position.x) <= attack.attackRange
            && Mathf.Abs(targetPosition.y - currentEnemy.transform.position.y) <= attack.attackRange)
        {
            isAttack = true;
            if (!currentEnemy.isHurt)
            {
                currentEnemy.rb.velocity = Vector2.zero;
            }
            // ���ƹ���Ƶ��
            attackRateCounter -= Time.deltaTime;
            if (attackRateCounter <= 0.0f)
            {
                attackRateCounter = attack.attackRate;
                // ���Ź�������
                currentEnemy.animator.SetTrigger("attack");
            }
        } else
        {
            // ����������Χ
            isAttack = false;
        }
        // ��ȡ�ƶ�����
        movDir = (targetPosition - currentEnemy.transform.position).normalized;
        // �л��泯����
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
