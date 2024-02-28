using UnityEngine;

public class SnailSkillState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = 0;
        currentEnemy.animator.SetBool("walk", false);
        currentEnemy.animator.SetBool("hide", true);
        currentEnemy.animator.SetTrigger("skill");

        // ���ö�ʧ��ʱ����ʱ��
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        // ��������޵�
        currentEnemy.GetComponent<Character>().invulnerable = true;
        currentEnemy.GetComponent<Character>().invulnerableCounter = currentEnemy.lostTimeCounter;
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
        }

        currentEnemy.GetComponent<Character>().invulnerableCounter = currentEnemy.lostTimeCounter;
    }

    public override void PhysicsUpdate()
    {
    }

    public override void OnExit()
    {
        currentEnemy.animator.SetBool("hide", false);
        currentEnemy.GetComponent<Character>().invulnerable = false;
    }
}