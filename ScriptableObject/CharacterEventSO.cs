using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Event/CharacterEventSO")]
public class CharacterEventSO : ScriptableObject
{
    // ֻ����Character����
    public UnityAction<Character> OnEventRised;

    /// <summary>
    /// �����¼�
    /// </summary>
    /// <param name="character">˭����������¼����Ͱ��Լ���Character���ݽ�ȥ</param>
    public void RaiseEvent(Character character)
    {
        OnEventRised?.Invoke(character);
    }
}
