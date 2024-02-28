using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/SceneLoadEventSO")]
public class SceneLoadEventSO : ScriptableObject
{
    public UnityAction<GameSceneSO, Vector3, bool> LoadRequestEvent;

    /// <summary>
    /// 场景加载请求
    /// </summary>
    /// <param name="gameSceneSO">要加载的场景</param>
    /// <param name="position">人物目的地坐标</param>
    /// <param name="fadeScreen">是否淡入淡出</param>
    public void RaiseLoadRequestEvent(GameSceneSO gameSceneSO, Vector3 position, bool fadeScreen)
    {
        LoadRequestEvent?.Invoke(gameSceneSO, position, fadeScreen);
    }
}
