using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeCanvas : MonoBehaviour
{
    [Header("�¼�����")]
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
    /// ���뵭���¼�
    /// </summary>
    /// <param name="targetColor">ת����Ŀ����ɫ</param>
    /// <param name="duration">����ʱ��</param>
    /// <param name="fadeType">��������</param>
    private void OnFadeEvent(Color targetColor, float duration)
    {
        fadeImage.DOBlendableColor(targetColor, duration);
    }
}
