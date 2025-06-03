using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalClass;
using GlobalEnum;

public class ExplosionController : BulletController
{
    [Header("Transforms")]
    [SerializeField] Transform mainCollider;

    public override void Init(BulletValueDetail _bulletValueDetail)
    {
        base.Init(_bulletValueDetail);
        bulletValueDetail.canPenetrated = true; // Explosion can penetrate
    }
    public override void Move(){
        mainCollider.gameObject.SetActive(true);
    } 
    public override void OnEventTriggerEnter2D(Collider _other){}
}
