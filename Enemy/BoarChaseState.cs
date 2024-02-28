using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.animator.SetBool("run", true);
    }
    public override void LogicUpdate()
    {
        // 丢失目标计时结束切换回巡逻状态
        if(currentEnemy.lostTimeCounter <= 0)
        {
            currentEnemy.SwitchState(NPCState.Patrol);
        }
        // 追击时，撞墙不等待并进行转身
        if (!currentEnemy.physicsCheck.isGround || (currentEnemy.physicsCheck.touchLeftWall && currentEnemy.faceDirect.x < 0) || (currentEnemy.physicsCheck.touchRightWall && currentEnemy.faceDirect.x > 0))
        {
            currentEnemy.transform.localScale = new Vector3(currentEnemy.faceDirect.x, 1, 1);
        }
    }

    public override void PhysicsUpdate()
    {
    }

    public override void OnExit()
    {
        currentEnemy.animator.SetBool("run", false);
    }
}
