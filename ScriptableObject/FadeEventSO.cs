using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Event/FadeEventSO")]
public class FadeEventSO : ScriptableObject
{
    public UnityAction<Color, float> OnEventRaised;

    /// <summary>
    /// 逐渐变黑
    /// </summary>
    /// <param name="duration"></param>
    public void FadeIn(float duration)
    {
        // 启动事件订阅的函数方法
        RaiseEvent(Color.black, duration);
    }

    /// <summary>
    /// 逐渐透明
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
