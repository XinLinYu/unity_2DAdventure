using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateBar : MonoBehaviour
{
    // ��ɫѪ��
    public Image healthImage;
    // ��ɫѪ��
    public Image healthDelayImage;
    // ������
    public Image powerImage;

    private void Update()
    {
        if (healthDelayImage.fillAmount > healthImage.fillAmount)
        {
            healthDelayImage.fillAmount -= Time.deltaTime;
        }
    }

    /// <summary>
    /// ����Health�ı���ٷֱ�
    /// </summary>
    /// <param name="percentage">�ٷֱȣ�current/Max</param>
    public void OnHealthChange(float percentage)
    {
        healthImage.fillAmount = percentage;
    }
}
