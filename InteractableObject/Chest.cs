using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    private SpriteRenderer spriteRenderer;
    public Sprite openSprite;
    public Sprite closeSprite;
    // ������ϱ�־����¼�ѻ�����״̬
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
        // ������󣬽�����ı�ǩ���ΪĬ�ϣ���������Ӵ����ѽ����ı���Ͳ��ᵯ��������ʾ��
        this.gameObject.tag = "Untagged";
    }

    public void PlayFXAudio()
    {
        GetComponent<AudioDefinition>()?.PlayAudioClip();
    }
}
