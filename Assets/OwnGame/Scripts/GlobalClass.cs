using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalClass
{
    [Serializable] public class BulletValueDetail{
        public int damage;
        public BulletValueDetail(){ }
        public void Init(BulletValueDetail _other)
        {
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
