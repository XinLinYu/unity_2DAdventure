using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;

public class CameraControl : MonoBehaviour
{
    [Header("事件监听")]
    public VoidEventSO afterSceneLoadedEvent;

    private CinemachineConfiner2D confiner2D;
    public CinemachineImpulseSource impulseSource;
    public VoidEventSO cameraShakeEvent;

    private void Awake()
    {
        confiner2D = GetComponent<CinemachineConfiner2D>();
    }

    private void OnEnable()
    {
        cameraShakeEvent.OnEventRaised += OnCameraShakeEvent;
        afterSceneLoadedEvent.OnEventRaised += OnAfterSceneLoadedEvent;
    }

    private void OnDisable()
    {
        cameraShakeEvent.OnEventRaised -= OnCameraShakeEvent;
        afterSceneLoadedEvent.OnEventRaised -= OnAfterSceneLoadedEvent;
    }

    private void OnAfterSceneLoadedEvent()
    {
        GetNewCameraBounds();
    }

    private void OnCameraShakeEvent()
    {
        impulseSource.GenerateImpulse();
    }

    /// <summary>
    /// 切换场景时，获取新的摄像机边界
    /// </summary>
    private void GetNewCameraBounds()
    {
        var obj = GameObject.FindGameObjectWithTag("Bounds");
        if (null == obj)
        {
            return;
        }
        confiner2D.m_BoundingShape2D = obj.GetComponent<Collider2D>();
        // 清除边界缓存
        confiner2D.InvalidateCache();
    }
}
