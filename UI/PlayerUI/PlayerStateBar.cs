using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStateBar : MonoBehaviour
{
    // 绿色血条
    public Image healthImage;
    // 红色血条
    public Image healthDelayImage;
    // 精力条
    public Image powerImage;

    private void Update()
    {
        if (healthDelayImage.fillAmount > healthImage.fillAmount)
        {
            healthDelayImage.fillAmount -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 接受Health的变更百分比
    /// </summary>
    /// <param name="percentage">百分比：current/Max</param>
    public void OnHealthChange(float percentage)
    {
        healthImage.fillAmount = percentage;
    }
}
