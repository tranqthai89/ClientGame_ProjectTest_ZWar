using System.Collections;
using System.Collections.Generic;
using GlobalEnum;
using Unity.Mathematics;
using UnityEngine;

public class GunBaseOnAnimationController : GunController
{
    public override GunType gunType => GunType.GunBaseOnAnimation;

    [Header("Prefabs")]
    [SerializeField] private BulletController bulletPrefab;

    public override void Shoot(Vector3 _posSpawn){
        BulletController _bullet = GamePlayManagerInstance.CreateBullet(bulletPrefab, _posSpawn, Quaternion.identity);
        _bullet.Init(GunValueDetail.bulletValueDetail, bulletSpeed, GunValueDetail.rangeShot);
        _bullet.Move();
    }
        
}
