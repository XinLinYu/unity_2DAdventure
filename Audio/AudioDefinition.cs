using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioDefinition : MonoBehaviour
{
    public PlayAudioEventSO playAudioEvent;
    public AudioClip audioClip;
    // 是否在启动时播放
    public bool playOnEnable;

    private void OnEnable()
    {
        if (playOnEnable)
        {
            PlayAudioClip();
        }
    }

    public void PlayAudioClip()
    {
        playAudioEvent.OnEventRaised(audioClip);
    }
}