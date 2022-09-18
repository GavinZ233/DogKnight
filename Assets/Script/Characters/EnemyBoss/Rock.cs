using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class Rock : MonoBehaviour
{
    public enum RockStates { HitPlayer,HitEnemy,HitNothing}
    private Rigidbody rb;
    [Header("��������")]

    public float force;
    public GameObject target;
    public int damage;
    private Vector3 direction;
    public RockStates rockStates;
    public GameObject rockEF;

    private float tikTime;
    public float disableTime;

    private void OnEnable()
    {
        tikTime = disableTime;
        rb.velocity = Vector3.one;  //����0�ٶ�,����״̬ΪHitNothing
        rockStates = RockStates.HitPlayer;
    }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

    }
    private void Update()
    {//��ʱ���Զ��ر�
        if (tikTime<0)
        {
            gameObject.SetActive(false);
        }
        tikTime -= Time.deltaTime;
    }
    private void FixedUpdate()
    {
        if (rockStates==RockStates.HitPlayer)
        {
            if (rb.velocity.sqrMagnitude < 2f)
            {
                rockStates = RockStates.HitNothing;
            }

        }
    }
    //public void FlyToTarget()
    //{
    //    if (target==null)
    //    {
    //        target = FindObjectOfType<PlayerController>().gameObject;
    //    }
    //    direction = (target.transform.position - transform.position).normalized;
    //    rb.AddForce(direction * force, ForceMode.Impulse); //ForceMode.Impulse�����
    //}

    private void OnCollisionEnter(Collision other)
    {
        switch (rockStates)
        {
            case RockStates.HitPlayer:
                if (other.gameObject.CompareTag("Player"))
                {
                    rockStates = RockStates.HitNothing;

                    other.gameObject.GetComponent<Rigidbody>().velocity = force * direction;
                    other.gameObject.GetComponent<PlayerController>().Dizzy();
                    other.gameObject.GetComponent<CharacterStats>().TakeDamage(damage,other.gameObject.GetComponent<CharacterStats>());
                }
                break;
            case RockStates.HitEnemy:
                if (other.gameObject.GetComponent<Golem>())
                {
                    var otherStats = other.gameObject.GetComponent<CharacterStats>();
                    otherStats.TakeDamage(damage, otherStats);
                    Instantiate(rockEF, transform.position, Quaternion.identity);
                    gameObject.SetActive(false);
                }
                break;
            case RockStates.HitNothing:
                break;
        }
    }
    public void HitBack()
    {
        rockStates = RockStates.HitEnemy;
    }
}
