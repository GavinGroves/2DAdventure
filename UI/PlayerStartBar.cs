using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStartBar : MonoBehaviour
{
    private Character currentCharacter;
    public Image healthImage;//green
    public Image healthDelayImage;//red
    public Image powerImage; //yellow;

    private bool isRecovering;

    private void Update()
    {
        if (healthDelayImage.fillAmount > healthImage.fillAmount)
        {
            healthDelayImage.fillAmount -= Time.deltaTime;
        }

        if (isRecovering)
        {
            float persentage = currentCharacter.currentPower / currentCharacter.maxPower;
            powerImage.fillAmount = persentage;
            if (persentage >= 1)
            {
                isRecovering = false;
                return;
            }
        }
    }

    /// <summary>
    /// 接收Health的变更百分比
    /// </summary>
    /// <param name="percentage">百分比：Current/Max</param>
    public void OnHealthChange(float percentage)
    {
        healthImage.fillAmount = percentage;
    }

    public void OnPowerChange(Character character)
    {
        isRecovering = true;
        currentCharacter = character;
    }
}
