using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeCanvas : MonoBehaviour
{
    [Header("事件监听")]
    public FadeEventSO fadeEvent;

    public Image fadeImage;

    private void OnEnable()
    {
        fadeEvent.OnEventRaised += OnFadeEvent;
    }

    private void OnDisable()
    {
        fadeEvent.OnEventRaised -= OnFadeEvent;
    }

    /// <summary>
    /// 淡入淡出事件
    /// </summary>
    /// <param name="targetColor">转换的目标颜色</param>
    /// <param name="duration">动画时间</param>
    /// <param name="fadeType">动画类型</param>
    private void OnFadeEvent(Color targetColor, float duration)
    {
        fadeImage.DOBlendableColor(targetColor, duration);
    }
}
