using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalClass
{
    [Serializable] public class GunValueDetail{
        public BulletValueDetail bulletValueDetail;
        public float atkSpeed;
        public float bulletSpeed;
        public float rangeShot;
        
        public void Init(GunValueDetail _other)
        {
            if(bulletValueDetail == null){
                bulletValueDetail = new BulletValueDetail();
            }
            bulletValueDetail.Init(_other.bulletValueDetail);
            atkSpeed = _other.atkSpeed;
            bulletSpeed = _other.bulletSpeed;    
            rangeShot = _other.rangeShot;    
        }
    }
    [Serializable] public class BulletValueDetail{
        public bool canExplosion;
        public bool canPenetrated; // xuyên mục tiêu, không tự hủy khi chạm vào mục tiêu
        public int damage;
        public BulletValueDetail(){ }
        public void Init(BulletValueDetail _other)
        {
            canExplosion = _other.canExplosion;
            canPenetrated = _other.canPenetrated;
            damage = _other.damage;    
        }
    }
    [System.Serializable] public class CollectionInfo {
        public int totalEnemy;
        public int totalEnemySpawned;
        public int totalEnemyDie;
        public float ratioStartFinalWave;

        public void ResetData(){
            totalEnemy = 0;
            totalEnemySpawned = 0;
            totalEnemyDie = 0;
            ratioStartFinalWave = 0;
        }
    }
}
