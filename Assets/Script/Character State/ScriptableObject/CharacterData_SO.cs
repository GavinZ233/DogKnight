using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Data",menuName ="Character States/Data")]
public class CharacterData_SO : ScriptableObject
{
    [Header("基础属性")]
    public int maxHealth;
    public int currentHealth;
    public int baseDefance;
    public int currentDefance;
    [Header("死亡经验")]
    public int killExpPoint;

    [Header("等级数据")]
    public int currentLevel;
    public int maxLevel;
    public int baseExp;
    public int currentExp;
    public float levelBuff;//成长系数
    /// <summary>
    /// 当前等级加成系数
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
        Debug.Log("升级" + currentLevel + "血量" + maxHealth);

    }
}
