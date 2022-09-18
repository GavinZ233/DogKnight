using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerState
{
    /// <summary>
    /// Ĭ��״̬�������ƶ������������������ܵ�����
    /// </summary>
    Normal,
    /// <summary>
    /// ����״̬�������ƶ������Է��������ܵ�����
    /// </summary>
    Attack,
    /// <summary>
    /// ����״̬�������ƶ������ܹ����������ܻ�
    /// </summary>
    Roll,
    /// <summary>
    /// ѣ��״̬�������ƶ������ܷ��������ܵ�����
    /// </summary>
    Dizzy,
    /// <summary>
    /// ����״̬�������ƶ������ܷ����������ܵ�����
    /// </summary>
    Death

}

/// <summary>
/// ��ɫ״̬�л�����������
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    private  Animator animator;
    public PlayerCamera playerCamera;
    //[SerializeField]
    public Transform followPoint;
    [SerializeField]
    private Transform lookAtPoint;
    private Rigidbody playerRB;

    private  WeaponController weaponController;
    private CharacterStats characterStats;

    public GameObject levelUp;
    public Vector3 currentInput { get; private set; }
    public float maxSpeed;
    /// <summary>
    /// ��ǰת���ٶ�
    /// </summary>
    public float rollForwardSpeed = 1; 
    /// <summary>
    /// ת��ʱ��������
    /// </summary>
    private float rotateMultiplier;  
    /// <summary>
    /// ��������
    /// </summary>
    public float rollForce;
     bool canMove;
     bool canRoll;
    bool isDead;
    public bool unBreakable;
    private PlayerState playerState;
    public PlayerState PlayerState
    {
        get => playerState;
        private set
        {
            playerState = value;
            switch (playerState)
            {
                case PlayerState.Normal:
                    break;
                case PlayerState.Attack:
                    break;
                case PlayerState.Roll:
                    break;
                case PlayerState.Dizzy:
                    break;
                case PlayerState.Death:
                    break;
            }
        }
    }
    private void Awake()
    {
        playerRB = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        weaponController = GetComponent<WeaponController>();
        characterStats = GetComponent<CharacterStats>();

        canMove = true;
        canRoll = true;
        unBreakable = false;
        PlayerState = PlayerState.Normal;
    }
    private void OnEnable()
    {
        GameManager.Instance.RigissterPlayer(characterStats);
        playerCamera.InitCamera(followPoint);

    }
    private void Start()
    {
        SaveManager.Instance.LoadPlayerData();

        weaponController.Init(playerCamera,animator);
        InventoryManager.Instance.RefreshAllUI();//�����������ɺ��һ֡ˢ�±������Լ������״̬
    }
    void Update()
    {
        if (characterStats.characterData.currentHealth==0)
        {
            isDead = true;
            playerState = PlayerState.Death;
            GameManager.Instance.GameEnd();

            animator.SetBool("Death",isDead);
        }

        if (!isDead)
        {
            UpdateMovementInput();
            StateOnUpdate();
            UpdatePlayerInput();
        }

    }
    private void UpdatePlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            GameManager.Instance.CantFound();
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            characterStats.CurrentHealth = 0 ;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            GameManager.Instance.EnemyDead();
        }
        if (MouseManager.Instance.showMouse) return;

        if (canRoll)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                transform.LookAt(transform.position + currentInput);
                animator.SetTrigger("Roll");

            }
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                animator.SetTrigger("BackJump");
            }

        }
        if (Input.GetMouseButtonDown(0))
        {
            animator.SetBool("Critical", weaponController.IsCritical());
            animator.SetTrigger("Attack");

        }
    }

    private void StateOnUpdate()
    {
        switch (PlayerState)
        {
            case PlayerState.Normal:
                canMove = true;
                canRoll = true;
                unBreakable = false;

                break;
            case PlayerState.Attack:
                canMove = false;
                canRoll = true;
                unBreakable = false;

                //Attack();

                break;
            case PlayerState.Roll:
                canMove = false;
                canRoll = false;
                unBreakable = true;

                //�趨��������
                break;
            case PlayerState.Dizzy:
                canMove = false;
                canRoll = false;
                unBreakable = false;
                break;
            case PlayerState.Death:
                canMove = false;
                canRoll = false;
                unBreakable = false;
                break;
        }
    }
    private void FixedUpdate()
    {
        Move();  
    }
    #region �߶�:�ƶ����룬��ת����,�ƶ�����
    /// <summary>
    /// ��������ǰ������
    /// </summary>
    private void UpdateMovementInput()
    {
        //ȷ����ɫǰ�����򡣷ֱ����X,Z������תyaw֮�����������ӵõ���ǰ��ǰ��������
        Quaternion rot = Quaternion.Euler(0, playerCamera.Yaw, 0);
        SetMovementInput(rot *Vector3.forward*Input.GetAxis("Vertical")+
            rot*Vector3.right*Input.GetAxis("Horizontal"));
    }

    private void Move()
    {
        if (canMove)
        {
            rotateBeforeMove();
            playerRB.MovePosition(transform.position + currentInput * maxSpeed * Time.fixedDeltaTime * rotateMultiplier);

        }

    }
    private void rotateBeforeMove()
    {
        //�ж�ǰ��ת
        if (currentInput.magnitude != 0)
        {
            QuestUI.Instance.CloseQuestUI();
            InventoryManager.Instance.CloseAllInventory();//�ж�ʱ�Զ��رմ���
            Quaternion rollForward = Quaternion.LookRotation(currentInput);
            transform.rotation = Quaternion.Lerp(transform.rotation, rollForward, rollForwardSpeed * Time.fixedDeltaTime);
        }
        //��ת����
        rotateMultiplier = Vector3.Angle(transform.forward, currentInput) > 15f ? 0.6f : 1f;
    }
    public void SetMovementInput(Vector3 input)
    {
        if (canMove)
        {
            currentInput = Vector3.ClampMagnitude(input, 1);//����������С
            animator.SetFloat("Speed", currentInput.magnitude);

        }

    }
    #endregion

    #region Attack
 

    /// <summary>
    /// �����������չ��뼼��
    /// </summary>
    //public void Attack()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        animator.SetTrigger("Attack");
    //        playerCamera.Shake();

    //    }


    //}
    public void StartAttack()
    {
        playerState = PlayerState.Attack;
        playerRB.velocity += playerRB.transform.forward * rollForce*0.2f;

    }

    public void EndAttack()
    {
        playerState = PlayerState.Normal;


    }
    #endregion

    #region ����
    public void RollStart()
    {
        playerRB.velocity = Vector3.zero;

        playerRB.velocity += playerRB.transform.forward * rollForce;
        playerState = PlayerState.Roll;
    }

    public void RollEnd()
    {
        playerRB.velocity = Vector3.zero;
        playerState = PlayerState.Normal;
    }

    public void BackJumpStart()
    {
        playerRB.velocity = Vector3.zero;

        playerRB.velocity += playerRB.transform.forward * -rollForce;
        playerState = PlayerState.Roll;

    }

    public void BackJumpEnd()
    {
        playerRB.velocity = Vector3.zero;
        playerState = PlayerState.Normal;

    }
    #endregion
    #region �ܻ�ѣ��

    public void Dizzy()
    {
        animator.SetTrigger("Dizzy");

    }

    public void DizzyStart()
    {
        playerState = PlayerState.Dizzy;
    }

    public void DizzyEnd()
    {
        playerState = PlayerState.Normal;
    }

    #endregion

    public void LevelUp()
    {
        StartCoroutine(LevelUpRing());
    }

    IEnumerator LevelUpRing()
    {
        levelUp.SetActive(true);
        yield return new WaitForSeconds(1);
        levelUp.SetActive(false);
    }
}
