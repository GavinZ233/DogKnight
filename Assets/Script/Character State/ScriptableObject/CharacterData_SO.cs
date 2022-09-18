using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data",menuName ="Character States/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("��������")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefance;
    public int currentDefance;
    [Header("��������")]
    public int killExpPoint;

    [Header("�ȼ�����")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;//�ɳ�ϵ��
    /// <summary>
    /// ��ǰ�ȼ��ӳ�ϵ��
    /// </summary>
    public float LevelMultiplier
    {
        get { return 1 + (currentLevel - 1) * levelBuff; }
    }

    public void UpdateExp(int exPoint)
    {
        currentExp += exPoint;
        while (currentExp >= baseExp)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        currentExp -= baseExp;
        currentLevel = Mathf.Clamp(currentLevel+1,1,maxLevel);
        baseExp = (int)(baseExp * LevelMultiplier);
        maxHealth = (int)(maxHealth * LevelMultiplier);
        currentHealth = maxHealth;
        baseDefance = (int)(baseDefance * LevelMultiplier);
        currentDefance = baseDefance;
        GameManager.Instance.playerState.GetComponent<PlayerController>().LevelUp();
        Debug.Log("����" + currentLevel + "Ѫ��" + maxHealth);

    }
}
