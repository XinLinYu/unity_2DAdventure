using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeePatrolState : BaseState
{
    // Ѳ�ߵ�Ŀ���
    private Vector3 targetPosition;
    // Ѳ�߷���
    private Vector3 movDir;

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
        // ��ȡ�µĵ�
        targetPosition = enemy.GetNewPoint();
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Chase);
        }
        // �ж��Ƿ񵽴���Ѳ��Ŀ�ĵص�λ��
        if (Mathf.Abs(targetPosition.x - currentEnemy.transform.position.x) < 0.1f 
            && Mathf.Abs(targetPosition.y - currentEnemy.transform.position.y) < 0.1f)
        {
            // ����ȴ�����ȡ������λ����Χ�������
            currentEnemy.wait = true;
            targetPosition = currentEnemy.GetNewPoint();
        }
        // ��ȡ�ƶ�����
        movDir = (targetPosition - currentEnemy.transform.position).normalized;
        if (!currentEnemy.wait)
        {
            // �л��泯����
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
