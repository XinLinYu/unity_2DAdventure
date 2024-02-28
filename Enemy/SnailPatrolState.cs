using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnailPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Skill);
        }

        if (!currentEnemy.physicsCheck.isGround || (currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDirect.x < 0) || (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDirect.x > 0))
        {
            currentEnemy.wait = true;
            currentEnemy.animator.SetBool("walk", false);
        }
        else
        {
            currentEnemy.animator.SetBool("walk", true);
        }
    }
    public override void PhysicsUpdate()
    {
    }

    public override void OnExit()
    {
    }

}
