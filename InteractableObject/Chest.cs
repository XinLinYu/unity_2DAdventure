using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    private SpriteRenderer spriteRenderer;
    public Sprite openSprite;
    public Sprite closeSprite;
    // 互动完毕标志，记录已互动的状态
    public bool isDone;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? openSprite : closeSprite;
    }

    public void TriggerAction()
    {
        if (!isDone)
        {
            OpenChest();
        }
    }

    private void OpenChest()
    {
        spriteRenderer.sprite = openSprite;
        isDone = true;
        // 互动完后，将宝箱的标签标记为默认，这样后面接触到已交互的宝箱就不会弹出按键提示了
        this.gameObject.tag = "Untagged";
    }

    public void PlayFXAudio()
    {
        GetComponent<AudioDefinition>()?.PlayAudioClip();
    }
}
