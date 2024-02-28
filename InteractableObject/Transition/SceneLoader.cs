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
    // 玩家坐标
    public Transform playerTrans;
    public Vector3 firstPosition;
    public Vector3 menuPosition;
    public PlayerInputControl playerInputControl;

    [Header("事件监听")]
    public SceneLoadEventSO loadEventSO;
    public VoidEventSO newGameEvent;
    public VoidEventSO backToMenuEvent;

    [Header("广播")]
    public VoidEventSO afterSceneLoadedEvent;
    public FadeEventSO fadeEventSO;
    public SceneLoadEventSO unloadedSceneEvent;

    [Header("场景")]
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
        // 游戏一运行，加载菜单场景
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
        Debug.Log("返回菜单");
        targetSceneToGo = menuLoadScene;
        loadEventSO.RaiseLoadRequestEvent(targetSceneToGo, menuPosition, true);
    }

    private void NewGame()
    {
        targetSceneToGo = firstLoadScene;
        // 通过事件的方式加载场景
        loadEventSO.RaiseLoadRequestEvent(targetSceneToGo, firstPosition, true);
    }
    
    /// <summary>
    /// 场景加载事件请求
    /// </summary>
    /// <param name="sceneToGo"></param>
    /// <param name="positionToGo"></param>
    /// <param name="fadeScreen"></param>
    private void OnLoadRequestEvent(GameSceneSO sceneToGo, Vector3 positionToGo, bool fadeScreen)
    {
        // 如果正在加载，则不执行加载过程中的任何操作
        if (isLoading)
        {
            return;
        }
        isLoading = true;
        targetSceneToGo = sceneToGo;
        targetPositionToGo = positionToGo;
        this.fadeScreen = fadeScreen;

        // 卸载当前场景，装载下一个场景
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
            // 实现淡入
            fadeEventSO.FadeIn(fadeTime);
        }
        // 等待淡入淡出的指定时间
        yield return new WaitForSeconds(fadeTime);

        // 在画面变黑之后，唤起UI的加载
        unloadedSceneEvent.RaiseLoadRequestEvent(targetSceneToGo, targetPositionToGo, true);

        // 非空判断
        yield return currentLoadScene.assetReference.UnLoadScene();
        // 关闭人物
        playerTrans.gameObject.SetActive(false);
        
        LoadNewScene();
    }

    private void LoadNewScene()
    {
        // 加载新场景
        var loadingOperation = targetSceneToGo.assetReference.LoadSceneAsync(LoadSceneMode.Additive, true);
        loadingOperation.Completed += OnLoadComplete;
    }

    private void OnLoadComplete(AsyncOperationHandle<SceneInstance> handle)
    {
        if (fadeScreen)
        {
            // 实现淡出
            fadeEventSO.FadeOut(fadeTime);
        }
        currentLoadScene = targetSceneToGo;

        playerTrans.position = targetPositionToGo;
        // 启动人物
        playerTrans.gameObject.SetActive(true);
        // 加载完毕，则把标识置为false
        isLoading = false;
        // 场景加载完成后的事件，如果是Location标签的场景，则执行。否则，不执行
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
