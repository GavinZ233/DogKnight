using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterStats : MonoBehaviour
{
    public event Action<int, int> UpdateHealthBar;
    public CharacterData_SO characterData;
    public CharacterData_SO templateData;
    private AttackData_SO baseAttackData;

    public bool unBreakable;
    private bool isPlayer;
    private CharacterData_SO curArm;
    public int CurArm { get { if (curArm != null) return curArm.baseDefance; else return 0; } }
    #region ���ݱ��� CharacterData_SO
    //��ȡ����������д��
    public int MaxHealth
    {
        get { if (characterData != null) return characterData.maxHealth; else return 0; }
        set { characterData.maxHealth = value; }
    }
    public int CurrentHealth
    {
        get { if (characterData != null) return characterData.currentHealth; else return 0; }
        set { characterData.currentHealth = value; }
    }
    public int BaseDefance
    {
        get { if (characterData != null) return characterData.baseDefance; else return 0; }
        set { characterData.baseDefance = value; }
    }
    public int CurrentDefance
    {
        get { if (characterData != null) return characterData.currentDefance; else return 0; }
        set { characterData.currentDefance = value; }
    }

    #endregion


    private void Awake()
    {
        if (templateData!=null)
        {
            characterData=Instantiate(templateData);
        }
        isPlayer = GetComponent<PlayerController>();
        unBreakable = false;
    }
    private void Update()
    {
        if (isPlayer)
        {
            unBreakable = GetComponent<PlayerController>().unBreakable;

        }

    }

    public void EquipArm(CharacterData_SO armData)
    {
        curArm = armData;
    }
    public void UnEquipArm()
    {
        curArm = null;    }

    /// <summary>
    /// �ܵ���ɫ�˺�
    /// </summary>
    public void TakeDamage(WeaponController attacker )
    {
        if (unBreakable) return;
        int damage = Mathf.Max(attacker.CurrentDamage() - CurrentDefance-CurArm, 0);
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        //�ܻ�����
        if (attacker.isCritical)
        {
            GetComponent<Animator>().SetTrigger("Hit");
            Debug.Log(attacker+"������" + this+damage);

        }
        else 
        Debug.Log(attacker + "������" + this + damage);
        UpdateHealthBar?.Invoke(CurrentHealth,MaxHealth);
        if (CurrentHealth <= 0 && !isPlayer)
        {
            GameManager.Instance.playerState.characterData.UpdateExp(characterData.killExpPoint);
        }
    }
    /// <summary>
    /// �ܵ������˺�
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="defener"></param>
    public void TakeDamage(int damage,CharacterStats defener)
    {
        if (unBreakable) return;
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        //�ܻ�����
         GetComponent<Animator>().SetTrigger("Hit");
         Debug.Log(  "ʯͷ����" + this + damage);


        UpdateHealthBar?.Invoke(CurrentHealth, MaxHealth);

        if (CurrentHealth <= 0&&!isPlayer)
        {
            GameManager.Instance.playerState.characterData.UpdateExp(characterData.killExpPoint);
        }

    }

    public void ApplyHealth(int Point)
    {
        CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + Point);
        Debug.Log(CurrentHealth);
    }
}
