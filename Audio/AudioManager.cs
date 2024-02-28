using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    [Header("事件监听")]
    // 音效事件监听
    public PlayAudioEventSO FXEvent;
    // 背景音乐事件监听
    public PlayAudioEventSO BGMEvent;
    public FloatEventSO volumeEvent;
    public VoidEventSO pauseEvent;

    [Header("事件广播")]
    public FloatEventSO syncVolumeEvent;

    // 背景音乐源
    public AudioSource BGMSource;
    // 音效源
    public AudioSource FXSource;

    public AudioMixer audioMixer;

    private void OnEnable()
    {
        FXEvent.OnEventRaised += OnFXEvent;
        BGMEvent.OnEventRaised += OnFXEvent;
        volumeEvent.OnEventRaised += OnVolumeEvent;
        pauseEvent.OnEventRaised += OnPauseEvent;
    }

    private void OnDisable()
    {
        FXEvent.OnEventRaised -= OnFXEvent;
        BGMEvent.OnEventRaised -= OnBGMEvent;
        volumeEvent.OnEventRaised -= OnVolumeEvent;
        pauseEvent.OnEventRaised -= OnPauseEvent;
    }

    private void OnPauseEvent()
    {
        float amount;
        audioMixer.GetFloat("MasterVolume",out amount);
        syncVolumeEvent.RaiseEvent(amount);
    }

    private void OnVolumeEvent(float amount)
    {
        audioMixer.SetFloat("MasterVolume", amount * 100 - 80);
    }

    private void OnBGMEvent(AudioClip clip)
    {
        BGMSource.clip = clip;
        BGMSource.Play();
    }

    private void OnFXEvent(AudioClip clip)
    {
        FXSource.clip = clip;
        FXSource.Play();
    }
}
