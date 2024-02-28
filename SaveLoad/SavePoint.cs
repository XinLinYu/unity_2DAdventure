using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePoint : MonoBehaviour, IInteractable
{
    [Header("广播")]
    public VoidEventSO saveDataEvent;

    public SpriteRenderer spriteRenderer;

    public GameObject lightObj;

    public Sprite darkSprite;
    public Sprite lightSprite;

    private bool isDone = false;

    private void OnEnable()
    {
        spriteRenderer.sprite = isDone ? lightSprite : darkSprite;
        lightObj.SetActive(isDone);
    }

    public void TriggerAction()
    {
        if (!isDone)
        {
            isDone = true;
            spriteRenderer.sprite = lightSprite;
            lightObj.SetActive(true);
            this.gameObject.tag = "Untagged";
            // 保存数据
            saveDataEvent.RaiseEvent();
        }
    }

    public void PlayFXAudio()
    {
        GetComponent<AudioDefinition>()?.PlayAudioClip();
    }
}
