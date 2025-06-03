using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEnum;
using GlobalClass;
    
public class BulletCanCreateExplosionController : BulletController
{
    override public BulletType BulletType => BulletType.CanCreateExplosion;

    [Header("Explosion Prefab")]
    [SerializeField] private ExplosionController explosionPrefab;

    public void CreateExplosion(){
        if(explosionPrefab != null)
        {
            MyAudioManager.Instance.PlaySfx(GameInformation.Instance.sfxExplosion);
            ExplosionController explosion = GamePlayManager.Instance.CreateBullet(explosionPrefab, transform.position, Quaternion.identity);
            explosion.Init(bulletValueDetail);
            explosion.Move();
        }
    }

    public override void OnEventTriggerEnter2D(Collider _other)
    {
        if(_other.tag.Equals("Ground") || _other.tag.Equals("Obstacle")){
            CreateExplosion();
            SelfDestruction();
        }
    }
}
