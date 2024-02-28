using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    // 只传递Character数据
    public UnityAction<Character> OnEventRised;

    /// <summary>
    /// 启动事件
    /// </summary>
    /// <param name="character">谁想启动这个事件，就把自己的Character传递进去</param>
    public void RaiseEvent(Character character)
    {
        OnEventRised?.Invoke(character);
    }
}
