using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour, IInteractable
{
    public SceneLoadEventSO loadEventSO;

    // 去往的场景
    public GameSceneSO sceneToGo;

    // 坐标点
    public Vector3 positionToGo;
    
    public void PlayFXAudio()
    {
        
    }

    public void TriggerAction()
    {
        loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
    }
}
