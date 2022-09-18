using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : EnemyController
{
    [Header("Skill")]
    public float kickForce;
    public Transform handpos;
    public float rockForce;
    public Collider skillCollider;
    private WeaponCollider skillWC;

    protected override void Awake()
    {
        base.Awake();
        skillWC = skillCollider.GetComponent<WeaponCollider>();
    }
    protected override  void OnEnable()
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
        if (defener.tag == "Enemy") return;
        Vector3 direction = transform.forward;
        defener.GetComponent<Rigidbody>().velocity = direction * kickForce;
        if (defener.tag=="Player")
        {
            defener.GetComponent<PlayerController>().Dizzy();
        }

       
    }

    public void ThrowRock()
    {
        if (attackTarget != null)
        {
            GameObject rock = RockPool.Instance.GetRockPool();
            if (rock!=null)
            {
                rock.SetActive(true);
                rock.transform.position = handpos.position;
                rock.GetComponent<Rock>().target = attackTarget;
                Vector3 direction=(attackTarget.transform.position - handpos.position).normalized;
                rock.GetComponent<Rigidbody>().AddForce(direction * rockForce, ForceMode.Impulse);
                    
                
            }
            else Debug.Log("¿Ø");

        }

    }


}
