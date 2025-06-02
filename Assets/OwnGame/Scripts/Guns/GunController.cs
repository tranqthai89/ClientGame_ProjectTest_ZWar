using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEnum;
using GlobalClass;

public class GunController : MonoBehaviour
{
    public GamePlayManager GamePlayManagerInstance{
        get{
            return GamePlayManager.Instance;
        }
    }

    public virtual GunType gunType => GunType.Unknown;

    [SerializeField] protected Transform bulletSpawnPoint;
    [SerializeField] protected Transform myCompass;

    protected float shootDelay;
    protected float bulletSpeed;
    protected float lastShootTime;
    public GunValueDetail GunValueDetail{get;set;}

    public virtual void Init(GunValueDetail _gunValueDetail)
    {
        if (_gunValueDetail == null)
        {
            Debug.LogError("GunInfo is null!");
            return;
        }
        GunValueDetail = new GunValueDetail();
        GunValueDetail.Init(_gunValueDetail);
        if(GunValueDetail.atkSpeed > 0)
        {
            shootDelay = 1f / GunValueDetail.atkSpeed;
        }
        else
        {
            shootDelay = 1f; // Default value if atkSpeed is not set
        }
       
        bulletSpeed = GunValueDetail.bulletSpeed;

        lastShootTime = Time.time + shootDelay; // Initialize to allow immediate shooting
    }

    public virtual void Shoot(){}
    public virtual void Shoot(Vector3 _posSpawn){}
    public bool CheckIfInRangeAttack(Vector3 _posCheck){
        try{
            if(GunValueDetail == null){
                Debug.LogError("gunValueDetail is null!");
                return false;
            }
            float _powRadiusAtkRange = GunValueDetail.rangeShot * GunValueDetail.rangeShot; // Sử dụng bình phương để so sánh khoảng cách
            float _tmpSqrDistance = Vector3.SqrMagnitude(transform.position - _posCheck);
            if(_tmpSqrDistance <= _powRadiusAtkRange){
                return true;
            }
            return false;
        }catch{
            return false;
        }
    }
}
