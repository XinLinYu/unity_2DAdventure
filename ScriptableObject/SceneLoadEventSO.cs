using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/SceneLoadEventSO")]
public class SceneLoadEventSO : ScriptableObject
{
    public UnityAction<GameSceneSO, Vector3, bool> LoadRequestEvent;

    /// <summary>
    /// ������������
    /// </summary>
    /// <param name="gameSceneSO">Ҫ���صĳ���</param>
    /// <param name="position">����Ŀ�ĵ�����</param>
    /// <param name="fadeScreen">�Ƿ��뵭��</param>
    public void RaiseLoadRequestEvent(GameSceneSO gameSceneSO, Vector3 position, bool fadeScreen)
    {
        LoadRequestEvent?.Invoke(gameSceneSO, position, fadeScreen);
    }
}
