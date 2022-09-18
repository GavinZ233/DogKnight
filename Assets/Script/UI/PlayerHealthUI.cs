using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthUI : MonoBehaviour,ISave
{
    Text levelText;
    Image healthSlider;
    Image expSlider;
    float currentExp;
    float baseExp;

    Image saveImage;

    private void OnEnable()
    {
        SaveManager.Instance.AddSave(this);
    }

    private void OnDisable()
    {
        if (SaveManager.Instance!=null)
        {
            SaveManager.Instance.RemoveSave(this);

        }

    }
    private void Awake()
    {
        levelText = transform.GetChild(2).GetComponent<Text>();
        healthSlider = transform.GetChild(0).GetChild(0).GetComponent<Image>();
        expSlider = transform.GetChild(1).GetChild(0).GetComponent<Image>();
        saveImage = transform.GetChild(3).GetComponent<Image>();
        saveImage.gameObject.SetActive(false);
    }


    private void LateUpdate()
    {
        currentExp = GameManager.Instance.playerState.characterData.currentExp;
        baseExp = GameManager.Instance.playerState.characterData.baseExp;
        levelText.text = "LEVEL" + GameManager.Instance.playerState.characterData.currentLevel.ToString();
        UpdateHealth();
        UpdateExp();

    }

    void UpdateHealth()
    {
        float sliderPercent = (float)GameManager.Instance.playerState.CurrentHealth / GameManager.Instance.playerState.MaxHealth;
        healthSlider.fillAmount = sliderPercent;
    }
    void UpdateExp()
    {
        float sliderPercent = currentExp / baseExp;
        expSlider.fillAmount = sliderPercent;

    }

    public void SaveSuccess()
    {
        StartCoroutine(showSave());
    }
    IEnumerator showSave()
    {
        saveImage.gameObject.SetActive(true);
        yield return new WaitForSeconds(2);
        saveImage.gameObject.SetActive(false);

    }
}
