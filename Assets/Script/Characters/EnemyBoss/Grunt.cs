using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grunt : EnemyController
{
    [Header("Skill")]
    public float kickForce;
    public Collider skillCollider;
    private WeaponCollider skillWC;

    protected override void Awake()
    {
        base.Awake();
        skillWC = skillCollider.GetComponent<WeaponCollider>();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        skillWC.OnTrigger += KickOff;


    }
    protected override void OnDisable()
    {
        base.OnDisable();
        skillWC.OnTrigger -= KickOff;

    }

    public void KickOff(GameObject defener)
    {
        if (defener.tag == "Enemy")        return;
        Vector3 direction = transform.forward;
        defener.GetComponent<Rigidbody>().velocity = direction * kickForce;
        if (defener.tag == "Player")
        {
            defener.GetComponent<PlayerController>().Dizzy();
        }
    }


}
