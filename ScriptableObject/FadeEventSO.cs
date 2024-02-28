using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/FadeEventSO")]
public class FadeEventSO : ScriptableObject
{
    public UnityAction<Color, float> OnEventRaised;

    /// <summary>
    /// �𽥱��
    /// </summary>
    /// <param name="duration"></param>
    public void FadeIn(float duration)
    {
        // �����¼����ĵĺ�������
        RaiseEvent(Color.black, duration);
    }

    /// <summary>
    /// ��͸��
    /// </summary>
    /// <param name="duration"></param>
    public void FadeOut(float duration) 
    {
        RaiseEvent(Color.clear, duration);
    }

    public void RaiseEvent(Color targetColor, float duration)
    {
        OnEventRaised?.Invoke(targetColor, duration);
    }
}
