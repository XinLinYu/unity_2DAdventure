using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPoint : MonoBehaviour, IInteractable
{
    public SceneLoadEventSO loadEventSO;

    // ȥ���ĳ���
    public GameSceneSO sceneToGo;

    // �����
    public Vector3 positionToGo;
    
    public void PlayFXAudio()
    {
        
    }

    public void TriggerAction()
    {
        loadEventSO.RaiseLoadRequestEvent(sceneToGo, positionToGo, true);
    }
}
