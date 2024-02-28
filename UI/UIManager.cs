using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public PlayerStateBar playerStateBar;
    [Header("�¼�����")]
    public CharacterEventSO healthEvent;
    public SceneLoadEventSO unloadedSceneEvent;
    public VoidEventSO loadDataEvent;
    public VoidEventSO gameOverEvent;
    public VoidEventSO backToMenuEvent;
    public FloatEventSO syncVolumeEvent;

    [Header("�¼��㲥")]
    public VoidEventSO pauseEvent;

    [Header("���")]
    public GameObject gameOverPanel;
    public GameObject restartBtn;
    // ��Ļ����
    public GameObject mobileTouch;
    public Button settingButton;
    public GameObject pausePanel;
    public Slider volumeSlider;

    private void Awake()
    {
        // �����PC����ر���Ļ����
#if UNITY_STANDALONE
        mobileTouch.SetActive(false);
#endif

        // �����ð�ť���һ�������¼�
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
            // ������Ϸ
            Time.timeScale = 1;
        } else
        {
            // �����¼��Ĺ㲥
            pauseEvent.RaiseEvent();
            pausePanel.SetActive(true);
            // ��ͣ��Ϸ
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
