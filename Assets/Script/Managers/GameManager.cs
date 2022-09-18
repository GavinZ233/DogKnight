using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerState;
    private PlayerCamera playerCamera;

    List<IEnemyControl> enemyControls = new List<IEnemyControl>();
    List<IPlayerState> iPlayerState = new List<IPlayerState>();
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this);

    }
    /// <summary>
    /// 玩家向管理器注册，获得跟随相机
    /// </summary>
    /// <param name="player"></param>
    public void RigissterPlayer(CharacterStats player)
    {
        playerState = player;

        playerCamera = FindObjectOfType<PlayerCamera>();
        if (playerCamera!=null)
        {
            player.GetComponent<PlayerController>().playerCamera = playerCamera;
        }
    }

    #region 接口
    public void AddControl(IEnemyControl control)
    {
        enemyControls.Add(control);
    }
    public void RemoveControl(IEnemyControl control)
    {
        enemyControls.Remove(control);
    }

    public void CantFound()
    {
        foreach (var control in enemyControls)
        {
            control.CantFound();
        }
    }
    public void EnemyDead()
    {
        foreach (var control in enemyControls)
        {
            control.EnemyDead();
        }
    }

    public void AddIPlayerState(IPlayerState iPlayer)
    {
        iPlayerState.Add(iPlayer);
    }
    public void RemoveIPlayerState(IPlayerState iPlayer)
    {
        iPlayerState.Remove(iPlayer);
    }

    public void PlayerLevelUp()
    {
        foreach (var iPlayer in iPlayerState)
        {
            iPlayer.LevelUp();
        }
    }
    public void GameEnd()
    {
        foreach (var iPlayer in iPlayerState)
        {
            iPlayer.EndGame();
        }
    }
    #endregion
}
