using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevToolkit;
using Lean.Pool;
using Cinemachine;

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
}
