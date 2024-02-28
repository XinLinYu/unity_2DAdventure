using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{
    private CapsuleCollider2D capsuleCollider;
    private PlayerController playerController;
    private Rigidbody2D rigidbody2D;
    [Header("状态")]
    public bool isGround;
    public bool onWall;
    public bool touchLeftWall;
    public bool touchRightWall;
    [Header("检测参数")]
    // 自动或手动找到碰撞体位置
    public bool manual;
    public bool isPlayer;
    // 检测半径
    public float checkRadius;
    public LayerMask groundLayer;

    public Vector2 bottomOffset;

    public Vector2 leftOffset;
    public Vector2 rightOffset;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>(); 
        if(!manual)
        {
            rightOffset = new Vector2((capsuleCollider.bounds.size.x / 2 + capsuleCollider.offset.x) , capsuleCollider.offset.y);
            leftOffset = new Vector2(- (capsuleCollider.bounds.size.x / 2 - capsuleCollider.offset.x), rightOffset.y);
        }

        if(isPlayer)
        {
            playerController = GetComponent<PlayerController>();
        }
    }

    private void Update()
    {
        Check();
    }
    public void Check()
    {
        // 检测地面
        if(onWall)
        {
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRadius, groundLayer);
        }
        else
        {
            isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, 0), checkRadius, groundLayer);
        }

        rightOffset = new Vector2((capsuleCollider.bounds.size.x / 2 + capsuleCollider.offset.x * transform.localScale.x), capsuleCollider.offset.y);
        leftOffset = new Vector2(-(capsuleCollider.bounds.size.x / 2 - capsuleCollider.offset.x * transform.localScale.x), rightOffset.y);

        //墙体判断
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(leftOffset.x, leftOffset.y), checkRadius, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(rightOffset.x, rightOffset.y), checkRadius, groundLayer);

        // 在墙壁上
        if(isPlayer)
        {
            onWall = (touchLeftWall && playerController.inputDirection.x < 0f || touchRightWall && playerController.inputDirection.x > 0f) && rigidbody2D.velocity.y < 0f;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(bottomOffset.x * transform.localScale.x, bottomOffset.y), checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(leftOffset.x * transform.localScale.x, leftOffset.y), checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + new Vector2(rightOffset.x * transform.localScale.x, rightOffset.y), checkRadius);
    }
}
