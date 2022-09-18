using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    public event Action<GameObject> OnTrigger;

    private List<GameObject> targets = new List<GameObject>();//�����ײ����Ŀ��



    private void OnDisable()
    {

    }
    private void Update()
    {
        if (GetComponent<Collider>().enabled==false)
        {
            targets.Clear();

        }
    }
    private void OnTriggerEnter(Collider defener)
    {
        if (targets.Contains(defener.gameObject)) return;//�ų���ײ����Ŀ��
        targets.Add(defener.gameObject);
        OnTrigger?.Invoke(defener.gameObject);
    }
}
