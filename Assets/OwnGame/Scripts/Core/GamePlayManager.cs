using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevToolkit;
using Lean.Pool;
using Cinemachine;
using GlobalEnum;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager Instance{get;set;}

    [Header("UIManager")]
    public GamePlay_UIManager UIManager;

    [Header("Components")]
    public WaveManager waveManager;
    public CinemachineVirtualCamera virtualCamera;

    public MySimplePoolManager effectPoolManager;
    public MySimplePoolManager bulletPoolManager;
    public MySimplePoolManager enemyPoolManager;
    public MySimplePoolManager mainCharPoolManager;

    public GameControl currentGameControl;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;

        effectPoolManager = new MySimplePoolManager();
        bulletPoolManager = new MySimplePoolManager();
        enemyPoolManager = new MySimplePoolManager();
    }

    void Start()
    {
        Init();
    }

    public void Init()
    {
        UIManager.RefreshUI_BtnToggleMusic();
        UIManager.RefreshUI_BtnToggleSfx();
        MyAudioManager.Instance.PlayMusic(GameInformation.Instance.bgm);
        currentGameControl = new GameControl();
    }

    #region Ultilities
    public T CreateEffect<T>(T _prefab, Vector3 _pos, Quaternion _quaternion) where T : ZWar.EffectController
    {
        if(effectPoolManager == null){effectPoolManager = new MySimplePoolManager();}
        if (_prefab == null)
        {
            Debug.LogError("Prefab is null!");
            return null;
        }

        T _tmpEffect = LeanPool.Spawn(_prefab, _pos, _quaternion);
        _tmpEffect.onSelfDestruction = (_o) => {
            effectPoolManager.RemoveObjectsNow(_o);
        };
        effectPoolManager.AddObject(_tmpEffect);

        return _tmpEffect;
    }
    public T CreateBullet<T>(T _prefab, Vector3 _pos, Quaternion _quaternion) where T : BulletController
    {
        if(bulletPoolManager == null){bulletPoolManager = new MySimplePoolManager();}
        if (_prefab == null)
        {
            Debug.LogError("Prefab is null!");
            return null;
        }

        T _tmpBullet = LeanPool.Spawn(_prefab, _pos, _quaternion);
        _tmpBullet.onSelfDestruction = (_o) => {
            bulletPoolManager.RemoveObjectsNow(_o);
        };
        bulletPoolManager.AddObject(_tmpBullet);

        return _tmpBullet;
    }
    public T CreateEnemy<T>(T _prefab, Vector3 _pos, Quaternion _quaternion) where T : EnemyController
    {
        if(enemyPoolManager == null){enemyPoolManager = new MySimplePoolManager();}
        if (_prefab == null)
        {
            Debug.LogError("Prefab is null!");
            return null;
        }

        T _tmpEnemy = LeanPool.Spawn(_prefab, _pos, _quaternion);
        _tmpEnemy.onSelfDestruction = (_o) => {
            enemyPoolManager.RemoveObjectsNow(_o);
        };
        enemyPoolManager.AddObject(_tmpEnemy);

        return _tmpEnemy;
    }
    #endregion

    #region  On button click
    public void OnBtnToggleMusic()
    {
        if (DataManager.Instance.musicStatus == 0)
        {
            DataManager.Instance.musicStatus = 1;
            MyAudioManager.Instance.ResumeMusic();
        }
        else
        {
            DataManager.Instance.musicStatus = 0;
            MyAudioManager.Instance.StopMusic();
        }
        UIManager.RefreshUI_BtnToggleMusic();
    }
    public void OnBtnToggleSfx()
    {
        if (DataManager.Instance.sfxStatus == 0)
        {
            DataManager.Instance.sfxStatus = 1;
        }
        else
        {
            DataManager.Instance.sfxStatus = 0;
        }
        UIManager.RefreshUI_BtnToggleSfx();
    }
    public void OnBtnSwitchMachineGun()
    {
        if(currentGameControl.currentState != GamePlayState.PlayGame 
            || currentGameControl.mainChar == null || currentGameControl.mainChar.CurrentState == MainCharState.Die
            || !currentGameControl.mainChar.timeToSwitchGun.CheckIfItsTime()){
            return; 
        }
        currentGameControl.mainChar.SwitchMachinGun();
        UIManager.RefreshUI_GroupBtnSwitchGun();
    }
    public void OnBtnSwitchMissile()
    {
        if(currentGameControl.currentState != GamePlayState.PlayGame 
            || currentGameControl.mainChar == null || currentGameControl.mainChar.CurrentState == MainCharState.Die
            || !currentGameControl.mainChar.timeToSwitchGun.CheckIfItsTime()){
            return; 
        }
        currentGameControl.mainChar.SwitchMissile();
        UIManager.RefreshUI_GroupBtnSwitchGun();
    }
    #endregion
}
