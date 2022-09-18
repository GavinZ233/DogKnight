using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerState
{
    /// <summary>
    /// 默认状态：可以移动，攻击，翻滚，会受到攻击
    /// </summary>
    Normal,
    /// <summary>
    /// 攻击状态：不能移动，可以翻滚，会受到攻击
    /// </summary>
    Attack,
    /// <summary>
    /// 翻滚状态：不能移动，不能攻击，不会受击
    /// </summary>
    Roll,
    /// <summary>
    /// 眩晕状态：不能移动，不能翻滚，会受到攻击
    /// </summary>
    Dizzy,
    /// <summary>
    /// 死亡状态：不能移动，不能翻滚，不会受到攻击
    /// </summary>
    Death

}

/// <summary>
/// 角色状态切换，动作处理
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
    /// 向前转向速度
    /// </summary>
    public float rollForwardSpeed = 1; 
    /// <summary>
    /// 转向时行走速率
    /// </summary>
    private float rotateMultiplier;  
    /// <summary>
    /// 翻滚力量
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
        InventoryManager.Instance.RefreshAllUI();//在玩家生成完成后第一帧刷新背包，以检查武器状态
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

                //设定不会受伤
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
    #region 走动:移动输入，旋转方向,移动人物
    /// <summary>
    /// 更新人物前方向量
    /// </summary>
    private void UpdateMovementInput()
    {
        //确定角色前方方向。分别计算X,Z轴与旋转yaw之后的向量，相加得到当前的前方向量。
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
        //行动前旋转
        if (currentInput.magnitude != 0)
        {
            QuestUI.Instance.CloseQuestUI();
            InventoryManager.Instance.CloseAllInventory();//行动时自动关闭窗口
            Quaternion rollForward = Quaternion.LookRotation(currentInput);
            transform.rotation = Quaternion.Lerp(transform.rotation, rollForward, rollForwardSpeed * Time.fixedDeltaTime);
        }
        //旋转降速
        rotateMultiplier = Vector3.Angle(transform.forward, currentInput) > 15f ? 0.6f : 1f;
    }
    public void SetMovementInput(Vector3 input)
    {
        if (canMove)
        {
            currentInput = Vector3.ClampMagnitude(input, 1);//限制向量大小
            animator.SetFloat("Speed", currentInput.magnitude);

        }

    }
    #endregion

    #region Attack
 

    /// <summary>
    /// 进攻，包含普攻与技能
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

    #region 翻滚
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
    #region 受击眩晕

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
