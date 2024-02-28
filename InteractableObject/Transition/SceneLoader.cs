using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour, ISaveable
{
    // �������
    public Transform playerTrans;
    public Vector3 firstPosition;
    public Vector3 menuPosition;
    public PlayerInputControl playerInputControl;

    [Header("�¼�����")]
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO newGameEvent;
    public VoidEventSO backToMenuEvent;

    [Header("�㲥")]
    public VoidEventSO afterSceneLoadedEvent;
    public FadeEventSO fadeEventSO;
    public SceneLoadEventSO unloadedSceneEvent;

    [Header("����")]
    public GameSceneSO firstLoadScene;
    public GameSceneSO menuLoadScene;

    private GameSceneSO currentLoadScene;
    private GameSceneSO targetSceneToGo;
    private Vector3 targetPositionToGo;
    private bool fadeScreen;

    public float fadeTime;
    private bool isLoading;
    private void Awake()
    {
        playerInputControl = new PlayerInputControl();
        
    }

    private void Start()
    {
        // ��Ϸһ���У����ز˵�����
        loadEventSO.RaiseLoadRequestEvent(menuLoadScene, menuPosition, true);
    }

    private void OnEnable()
    {
        loadEventSO.LoadRequestEvent += OnLoadRequestEvent;
        newGameEvent.OnEventRaised += NewGame;
        backToMenuEvent.OnEventRaised += OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        loadEventSO.LoadRequestEvent -= OnLoadRequestEvent;
        newGameEvent.OnEventRaised -= NewGame;
        backToMenuEvent.OnEventRaised -= OnBackToMenuEvent;

        ISaveable saveable = this;
        saveable.UnregisterSaveData();
    }

    private void OnBackToMenuEvent()
    {
        Debug.Log("���ز˵�");
        targetSceneToGo = menuLoadScene;
        loadEventSO.RaiseLoadRequestEvent(targetSceneToGo, menuPosition, true);
    }

    private void NewGame()
    {
        targetSceneToGo = firstLoadScene;
        // ͨ���¼��ķ�ʽ���س���
        loadEventSO.RaiseLoadRequestEvent(targetSceneToGo, firstPosition, true);
    }
    
    /// <summary>
    /// ���������¼�����
    /// </summary>
    /// <param name="sceneToGo"></param>
    /// <param name="positionToGo"></param>
    /// <param name="fadeScreen"></param>
    private void OnLoadRequestEvent(GameSceneSO sceneToGo, Vector3 positionToGo, bool fadeScreen)
    {
        // ������ڼ��أ���ִ�м��ع����е��κβ���
        if (isLoading)
        {
            return;
        }
        isLoading = true;
        targetSceneToGo = sceneToGo;
        targetPositionToGo = positionToGo;
        this.fadeScreen = fadeScreen;

        // ж�ص�ǰ������װ����һ������
        if (currentLoadScene != null)
        {
            StartCoroutine(UnloadPreviousScene());
        } else
        {
            LoadNewScene();
        }
    }

    private IEnumerator UnloadPreviousScene()
    {
        if (fadeScreen)
        {
            // ʵ�ֵ���
            fadeEventSO.FadeIn(fadeTime);
        }
        // �ȴ����뵭����ָ��ʱ��
        yield return new WaitForSeconds(fadeTime);

        // �ڻ�����֮�󣬻���UI�ļ���
        unloadedSceneEvent.RaiseLoadRequestEvent(targetSceneToGo, targetPositionToGo, true);

        // �ǿ��ж�
        yield return currentLoadScene.assetReference.UnLoadScene();
        // �ر�����
        playerTrans.gameObject.SetActive(false);
        
        LoadNewScene();
    }

    private void LoadNewScene()
    {
        // �����³���
        var loadingOperation = targetSceneToGo.assetReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOperation.Completed += OnLoadComplete;
    }

    private void OnLoadComplete(AsyncOperationHandle<SceneInstance> handle)
    {
        if (fadeScreen)
        {
            // ʵ�ֵ���
            fadeEventSO.FadeOut(fadeTime);
        }
        currentLoadScene = targetSceneToGo;

        playerTrans.position = targetPositionToGo;
        // ��������
        playerTrans.gameObject.SetActive(true);
        // ������ϣ���ѱ�ʶ��Ϊfalse
        isLoading = false;
        // ����������ɺ���¼��������Location��ǩ�ĳ�������ִ�С����򣬲�ִ��
        if (currentLoadScene.sceneType == SceneType.Location)
        {
            afterSceneLoadedEvent.RaiseEvent();
        }
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data)
    {
        data.SaveGameScene(currentLoadScene);
    }

    public void LoadData(Data data)
    {
        var playerID = playerTrans.GetComponent<DataDefination>().ID;
        if (data.characterPosDict.ContainsKey(playerID))
        {
            targetPositionToGo =  data.characterPosDict[playerID].ToVector3();
            targetSceneToGo = data.getSaveScene();

            OnLoadRequestEvent(targetSceneToGo, targetPositionToGo, true);
        }
    }
}
