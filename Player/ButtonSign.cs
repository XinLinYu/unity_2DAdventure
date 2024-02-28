using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;

public class ButtonSign : MonoBehaviour
{
    private PlayerInputControl playerInput;
    private Animator anim;
    public Transform playerTransform;

    public GameObject buttonSignSprite;
    private IInteractable targerItem;
    private bool canPress;

    private void Awake()
    {
        anim = buttonSignSprite.GetComponent<Animator>();

        playerInput = new PlayerInputControl();
        playerInput.Enable();
    }

    private void OnEnable()
    {
        InputSystem.onActionChange += OnActionChange;

        playerInput.Gameplay.Interact.started += OnInteract;
    }

    private void OnDisable()
    {
        canPress = false;
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (canPress)
        {
            targerItem.TriggerAction();
            // 播放音效
            targerItem.PlayFXAudio();
        }
    }

    private void OnActionChange(object obj, InputActionChange actionChange)
    {
        if (actionChange == InputActionChange.ActionStarted)
        {
            // Debug.Log(((InputAction)obj).activeControl.device);
            var device = ((InputAction)obj).activeControl.device;
            switch (device.device)
            {
                case Keyboard:
                    anim.Play("KeyboardInteract");
                    break;
                case XInputController:
                    anim.Play("GamepadInteract");
                    break;
            }
        }
    }

    private void Update()
    {
        buttonSignSprite.GetComponent<SpriteRenderer>().enabled = canPress;
        buttonSignSprite.transform.localScale = playerTransform.localScale;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        // 如果碰撞体标签为可互动的
        if (collision.CompareTag("Interactable"))
        {
            canPress = true;

            // 获得可互动的目标实体
            targerItem = collision.GetComponent<IInteractable>();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canPress = false;
    }
}
