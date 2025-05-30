using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DevToolkit;
using Lean.Pool;

public class GamePlayManager : MonoBehaviour
{
    public static GamePlayManager Instance{get;set;}
    public MySimplePoolManager effectPoolManager;
    public MySimplePoolManager bulletPoolManager;


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
    }

    public T CreateEffect<T>(T _prefab, Vector3 _pos, Quaternion _quaternion) where T : MySimplePoolObjectController
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
}
