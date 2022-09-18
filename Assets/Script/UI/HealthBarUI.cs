using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class HealthBarUI : MonoBehaviour
{
    public GameObject healthUIPrefab;

    public Transform barPoint;
    public bool alwaysVisible;
    public float visibleTime;
    float timeTIK;

    Image healthSlider;
    Transform UIBarTrans;
    Transform cameraTrans;
    CharacterStats characterStats;

    private void Awake()
    {
        characterStats = GetComponent<CharacterStats>();
        characterStats.UpdateHealthBar += UpdateHealthBar;
    }
    private void OnEnable()
    {
        cameraTrans = Camera.main.transform;
        foreach (Canvas canvas in FindObjectsOfType<Canvas>())
        {
            if (canvas.renderMode==RenderMode.WorldSpace)
            {
                UIBarTrans = Instantiate(healthUIPrefab,canvas.transform).transform;
                healthSlider = UIBarTrans.GetChild(0).GetComponent<Image>();
                UIBarTrans.gameObject.SetActive(alwaysVisible);
            }
        }
    }

    private void UpdateHealthBar(int currentHealth,int maxHealth)
    {
        if (currentHealth<=0)
        {
            Destroy(UIBarTrans.gameObject);
        }
        UIBarTrans.gameObject.SetActive(true);
        timeTIK = visibleTime;

        float sliderPercent = (float)currentHealth / maxHealth;
        healthSlider.fillAmount = sliderPercent;
    }

    private void LateUpdate()
    {
        if (UIBarTrans!=null)
        {
            UIBarTrans.position = barPoint.position;
            UIBarTrans.forward = -cameraTrans.forward;
            if (timeTIK <= 0 && !alwaysVisible)
            {
                UIBarTrans.gameObject.SetActive(false);
            }
            else timeTIK -= Time.deltaTime;
        }
    }
}
