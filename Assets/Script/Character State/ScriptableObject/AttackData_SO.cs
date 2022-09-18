using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="Attack Data",menuName = "Character States/Attack Data")]
public class AttackData_SO : ScriptableObject
{
    public float attackRange;
    public float skillRange;
    public float coolDown;
    [Header("ÉËº¦Êý¾Ý")]
    public int minDamge;
    public int maxDamge;
    public float criticalMultiplier;
    public float criticalChance; 

    //public void ApplyWeaponData()
    //{

    //}
}
