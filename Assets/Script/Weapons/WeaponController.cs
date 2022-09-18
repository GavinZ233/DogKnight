using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// װ���������ݣ����ݸ�player��
/// ��ײ��⣬�����˺�������
/// </summary>
public class WeaponController : MonoBehaviour
{
    [Header("��������")]
    PlayerCamera playerCamera;
    private bool isPlayer;
    public bool isCritical;
    Animator animator;
    public AttackData_SO attackData;
    private GameObject attackTarget;
    public GameObject skillTarget;

    [Header("Ĭ�Ϲ�������")]
    public Collider attack1Coll;
    public Collider attack2Coll;
    private WeaponCollider attack1WC;
    private WeaponCollider attack2WC;
    public AttackData_SO fistAttackData;
    public AnimatorOverrideController fistAnim;
    [Header("��������")]
    public Transform weaponSlot;
    public AttackData_SO weaponAttackData;
    private Collider weaponCollider;
    private WeaponCollider weaponWC;




    private void Awake()
    {
        if (attack1Coll!=null)
            attack1WC = attack1Coll.GetComponent<WeaponCollider>();
            attack1Coll.enabled = false;
        if (attack2Coll!=null)
            attack2WC = attack2Coll.GetComponent<WeaponCollider>();
            attack2Coll.enabled = false;

        attackTarget = null;
        skillTarget = null;
        isPlayer = gameObject.tag == "Player";

    }


    private void OnEnable()
    {
        if (attack1WC != null)
            attack1WC.OnTrigger += TriggerEnter;
            attack1Coll.enabled = false;

        if (attack2WC != null)
            attack2WC.OnTrigger += TriggerEnter;
            attack2Coll.enabled = false;

    }

    private void OnDisable()
    {
        if (attack1WC != null)
            attack1WC.OnTrigger -= TriggerEnter;
        if (attack2WC != null)
            attack2WC.OnTrigger -= TriggerEnter;

    }

    /// <summary>
    /// ��ʼ������ȡ���������붯����������Ч
    /// </summary>
    public void Init(PlayerCamera playerCamera,Animator animator)
    {
        this.playerCamera = playerCamera;
        this.animator = animator;
    }
    public  void TriggerEnter(GameObject defener)
    {

        attackTarget = defener;
        if (isPlayer)
        {

            if (defener.tag == "Enemy" )
            {
                if (isCritical)
                {
                    playerCamera.Shake();//����ʱ�ٵ���
                    StartCoroutine(PauseFrame());

                }
                Hit();
            }
            if (defener.tag == "Attackable")
            {
                StartCoroutine(PauseFrame());
                Hit();
            }
        } 
        else
        {
            //��ͨ���˹���
            if (defener.tag=="Player")
            {
                Hit();
               
            }


        }
       

    }


    public void ChangeWeapon(ItemData_SO weapon)
    {
        UnEquipWeapon();
        EquipWeapon(weapon);

    }
    public void EquipWeapon(ItemData_SO weapon)
    {
        //�滻��attackData��animator������ģ�ͣ�����weapon��ײ��
        if (weapon.weaponProfab!=null)
        {
            weaponCollider = Instantiate(weapon.weaponProfab, weaponSlot).GetComponent<Collider>();
        }
        weaponWC = weaponCollider.GetComponent<WeaponCollider>();
        weaponWC.OnTrigger += TriggerEnter;

        attackData = weapon.weaponData;
        animator.runtimeAnimatorController = weapon.weaponAnimator;
    }

    public void UnEquipWeapon()
    {
        //��Ĭ��fist�滻��attackData��animator��ɾ��ģ�ͣ�����weapon��ײ��
        if (weaponSlot.transform.childCount != 0)
        {
            for (int i = 0; i < weaponSlot.transform.childCount; i++)
            {
                weaponWC.OnTrigger -= TriggerEnter;
                Destroy(weaponSlot.transform.GetChild(i).gameObject);
            }
        }
        attackData = fistAttackData;
        animator.runtimeAnimatorController = fistAnim;
    }

    /// <summary>
    /// ���㲢���ر��������weapon
    /// </summary>
    /// <returns></returns>
    public bool IsCritical()
    {
        return isCritical = Random.value < attackData.criticalChance;
    }

    public int CurrentDamage()
    {
        float coreDamage = Random.Range(attackData.minDamge, attackData.maxDamge);
        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
        }
        return (int)coreDamage;
    }
    public void Hit()
    {
        if (attackTarget.CompareTag("Attackable"))
        {
            if ( attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().HitBack();

                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.zero;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);

            }

        }
        else
        {
            if (attackTarget.GetComponent<CharacterStats>())
            {
                attackTarget.GetComponent<CharacterStats>().TakeDamage(this);

            }

        }
    }
    #region ��ײ�忪��
    public void OpenCollider01()
    {

        attack1Coll.enabled=true;

    }
    public void CloseCollider01()
    {
        attack1Coll.enabled = false;
    }
    public void OpenCollider02()
    {       

        attack2Coll.enabled = true;

    }
    public void CloseCollider02()
    {
        attack2Coll.enabled = false;
    }
    public void OpenWeaponCollider()
    {

        weaponCollider.enabled = true;

    }
    public void CloseWeaponCollider()
    {
        weaponCollider.enabled = false;
    }
    #endregion
    /// <summary>
    /// ��֡Ч��
    /// </summary>
    /// <returns></returns>
    private IEnumerator PauseFrame()
    {
        // �ö���ֹͣ
        animator.speed = 0;
        // �ӳ�һ��ʱ��
        yield return new WaitForSeconds(0.25f);
        // �ö����ָ�
        animator.speed = 1;

    }
}
