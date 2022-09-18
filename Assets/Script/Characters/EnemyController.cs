using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// פ�أ�Ѳ�ߣ�ս��״̬������
/// </summary>
public enum EnemyState { GUARD,PATROL,CHASE,DEAD}
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour,IEnemyControl,IPlayerState
{
    private EnemyState enemyState;
    private NavMeshAgent agent;
    private Animator anim;
    private Collider coll;
    protected WeaponController weaponController;
    private CharacterStats characterStats;
    [Header("�������� ")]
    public float sightRadius;
    public bool isGuard;
    public float lookAtTime;
    public float rotationSpeed;
    private float remainLookAtTime;
    protected GameObject attackTarget;
    private float speed;
    private float lastAttackTime;


    [Header("·������")]
    public float patrolRange;
    private Vector3 wayPos;
    private Vector3 guardPos;
    private Quaternion guardRotation;
    bool isWalk;
    bool isChase;
    bool isRun;
    bool isDead;
    bool playerDead;
    bool cantFound;


    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider>();
        weaponController = GetComponent<WeaponController>();
        characterStats = GetComponent<CharacterStats>();
        speed = agent.speed;
        remainLookAtTime = lookAtTime;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        cantFound = false;

    }
    private void Start()
    {
        GameManager.Instance.AddControl(this);
        GameManager.Instance.AddIPlayerState(this);
        if (isGuard)
        {
            enemyState = EnemyState.GUARD;
        }
        else
        {
            enemyState = EnemyState.PATROL;
            GetNewWayPos();
        }
    }

    private void Update()
    {
        if (characterStats.characterData.currentHealth == 0)
        {
            isDead = true;
        }
        
        SwitchEnemyState();
        SwitchAnimation();
        lastAttackTime -= Time.deltaTime;
    }
    protected virtual void OnEnable()
    {
    }
    protected virtual void OnDisable()
    {
        //��ֹgamemanager��ǰɾ��ʱ������enemy���ÿպ�����������
        if (!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveControl(this);
        GameManager.Instance.RemoveIPlayerState(this);
        if (GetComponent<LootSpawner>()&&isDead)
        {
            GetComponent<LootSpawner>().SpawnLoot();
        }
        if (QuestManager.IsInitialized && isDead)
        {
            QuestManager.Instance.UpdateQuestProgress(this.name, 1);
        }
    }

    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Run", isRun);
        anim.SetBool("Death", isDead);

    }
    
    void SwitchEnemyState()
    {
        if (isDead)
        {
            enemyState = EnemyState.DEAD;
        }
        else if (FoundPlayer())
        {
            enemyState = EnemyState.CHASE;
        }
        switch (enemyState)
        {
            case EnemyState.GUARD:
                agent.speed = speed * 0.5f;
                isChase = false;
                //Ŀ��·��Ϊפ�ص㣬������פ�ص�ʱ���ص����rotation
                if (transform.position!=guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;
                    //SqrMagnitude��������ƽ����magnitude�������ĳ��ȣ�ʹ��ƽ����ʡ����
                    if (Vector3.SqrMagnitude(guardPos-transform.position)<=agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, rotationSpeed);
                    }
                }
                break;
            case EnemyState.PATROL:
                agent.speed = speed * 0.5f;
                isChase =false;
                //�ж��Ƿ񵽴�Ѳ�ߵ�
                if (Vector3.Distance(wayPos , transform.position) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    //����Ѱ·��󣬷���һ�����Ѱ·
                    if (remainLookAtTime>0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {

                        GetNewWayPos();
                    }
                }
                else
                {
                    isWalk = true;
                    agent.destination = wayPos;
                }

                break;
            case EnemyState.CHASE:
                agent.speed = speed;
                isWalk = false;
                isRun = true;
                isChase = true;
                if (!FoundPlayer())
                {
                    isRun = false;
                    //��ʧĿ���ȷ���
                    if (remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    //�ع����״̬
                    else if (isGuard) enemyState = EnemyState.GUARD;
                    else enemyState = EnemyState.PATROL;
                }
                else
                {
                    isRun = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;

                }
                if (TargetInAttackRange()||TargetInSkillRange())
                {
                    isRun = false;
                    agent.speed = 0;
                    RotationBeforeAttack();
                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = weaponController.attackData.coolDown;
                        Attack();
                    }
                }
               
                break;
            case EnemyState.DEAD:
                coll.enabled = false;
                agent.radius = 0;//����뾶
                Destroy(gameObject,2f);
                break;
        }
    }
    void Attack()
    {
        if (TargetInSkillRange())
        {
            anim.SetTrigger("Skill");
        }
        if (TargetInAttackRange())
        {
            anim.SetBool("Critical", weaponController.IsCritical());
            anim.SetTrigger("Attack");

        }



    }
    void RotationBeforeAttack()
    {
        Vector3 v = (attackTarget.transform.position - transform.position);
        Quaternion rotate = Quaternion.LookRotation(v);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotate, rotationSpeed);
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= weaponController.attackData.attackRange;
        }
        else return false;
    }
    bool TargetInSkillRange()
    {
        if (attackTarget != null)
        {
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= weaponController.attackData.skillRange;
        }
        else return false;
    }
        /// <summary>
        /// �������㸽���������waypos
        /// </summary>
        void GetNewWayPos()
    {
        remainLookAtTime = lookAtTime;
        float randomX = Random.Range(-patrolRange,patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);
        Vector3 randomPoint = new Vector3(guardPos.x+randomX, transform.position.y,guardPos.z+randomZ );
        NavMeshHit hit;
        //��ָ����Χ���ҵ���������������ĵ㡣�˺����Ե���������в��������ҵ���������������ĵ㡣
        wayPos = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : transform.position;
    }


    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach (var item in colliders)
        {
            if (item.CompareTag("Player")&&!playerDead&&!cantFound)
            {
                attackTarget = item.gameObject;
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmosSelected()  //������Ұ��Χ
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sightRadius);
        
    }

    public void EnemyDead()
    {
        characterStats.characterData.currentHealth = 0;
    }

    public void CantFound()
    {
        cantFound = !cantFound;
        attackTarget = null;

    }

    public void LevelUp()
    {
    }

    public void EndGame()
    {
        cantFound = true;
        playerDead = true;
        attackTarget = null;

    }
}
