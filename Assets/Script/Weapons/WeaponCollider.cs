using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    public event Action<GameObject> OnTrigger;

    private List<GameObject> targets = new List<GameObject>();//存放碰撞过的目标



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
        if (targets.Contains(defener.gameObject)) return;//排除碰撞过的目标
        targets.Add(defener.gameObject);
        OnTrigger?.Invoke(defener.gameObject);
    }
}
