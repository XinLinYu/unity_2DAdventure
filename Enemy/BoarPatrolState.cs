using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }
    public override void LogicUpdate()
    {
        // 发现player开始追击chase
        if (currentEnemy.FoundPlayer())
        {
            currentEnemy.SwitchState(NPCState.Chase);
        }

        if (!currentEnemy.physicsCheck.isGround || (currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDirect.x < 0) || (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDirect.x > 0))
        {
            currentEnemy.wait = true;
            currentEnemy.animator.SetBool("walk", false);
        } else
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
