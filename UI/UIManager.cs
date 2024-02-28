using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PlayerStateBar playerStateBar;
    [Header("事件监听")]
    public CharacterEventSO healthEvent;
    public SceneLoadEventSO unloadedSceneEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO gameOverEvent;
    public VoidEventSO backToMenuEvent;
    public FloatEventSO syncVolumeEvent;

    [Header("事件广播")]
    public VoidEventSO pauseEvent;

    [Header("组件")]
    public GameObject gameOverPanel;
    public GameObject restartBtn;
    // 屏幕触控
    public GameObject mobileTouch;
    public Button settingButton;
    public GameObject pausePanel;
    public Slider volumeSlider;

    private void Awake()
    {
        // 如果是PC，则关闭屏幕触控
#if UNITY_STANDALONE
        mobileTouch.SetActive(false);
#endif

        // 给设置按钮添加一个监听事件
        settingButton.onClick.AddListener(TogglePausePanel);
    }

    private void OnEnable()
    {
        healthEvent.OnEventRised += OnHealthEvent;
        unloadedSceneEvent.LoadRequestEvent += OnUnloadSceneEvent;
        loadDataEvent.OnEventRaised += OnLoadDataEvent;
        gameOverEvent.OnEventRaised += OnGameOverEvent;
        backToMenuEvent.OnEventRaised += OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised += OnSyncVolumeEvent;
    }

    private void OnDisable()
    {
        healthEvent.OnEventRised -= OnHealthEvent;
        unloadedSceneEvent.LoadRequestEvent -= OnUnloadSceneEvent;
        loadDataEvent.OnEventRaised -= OnLoadDataEvent;
        gameOverEvent.OnEventRaised -= OnGameOverEvent;
        backToMenuEvent.OnEventRaised -= OnLoadDataEvent;
        syncVolumeEvent.OnEventRaised -= OnSyncVolumeEvent;
    }

    private void OnSyncVolumeEvent(float amount)
    {
        volumeSlider.value = (amount + 80) / 100;
    }

    private void TogglePausePanel()
    {
        bool activeFlag = pausePanel.activeInHierarchy;
        if (activeFlag)
        {
            pausePanel.SetActive(false);
            // 启动游戏
            Time.timeScale = 1;
        } else
        {
            // 唤起事件的广播
            pauseEvent.RaiseEvent();
            pausePanel.SetActive(true);
            // 暂停游戏
            Time.timeScale = 0;
        }
    }

    private void OnGameOverEvent()
    {
        gameOverPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(restartBtn);
    }

    private void OnLoadDataEvent()
    {
        gameOverPanel.SetActive(false);
    }

    private void OnUnloadSceneEvent(GameSceneSO sceneToLoad, Vector3 arg1, bool arg2)
    {
        var isMenu = sceneToLoad.sceneType == SceneType.Menu;
        playerStateBar.gameObject.SetActive(!isMenu);
    }

    private void OnHealthEvent(Character character)
    {
        var percentage = character.currentHealth / character.maxHealth;
        playerStateBar.OnHealthChange(percentage);
    }
}
