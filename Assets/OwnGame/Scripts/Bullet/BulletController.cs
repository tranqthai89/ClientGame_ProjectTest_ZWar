using System;
using System.Collections;
using System.Collections.Generic;
using DevToolkit;
using UnityEngine;
using GlobalClass;
using GlobalEnum;

public class BulletController : MySimplePoolObjectController
{
    public virtual BulletType BulletType => BulletType.Normal;

    [NonSerialized] public GlobalClass.BulletValueDetail bulletValueDetail;
    protected float speed;
    protected float maxRangeMove;

    [Header("Prefabs")]
    [SerializeField] ZWar.EffectController effectHitPrefab;
    [SerializeField] ExplosionController explosionPrefab;

    IEnumerator process_Move;
    
    public override void StopAllActionNow()
    {
        base.StopAllActionNow();
        process_Move = null;
    }

    public virtual void Init(BulletValueDetail _bulletValueDetail, float _speed, float _maxRangeMove)
    {
        if(bulletValueDetail == null){
            bulletValueDetail = new BulletValueDetail();
        }
        bulletValueDetail.Init(_bulletValueDetail);
        speed = _speed;
        maxRangeMove = _maxRangeMove;
    }
    public virtual void Init(BulletValueDetail _bulletValueDetail)
    {
        if(bulletValueDetail == null){
            bulletValueDetail = new BulletValueDetail();
        }
        bulletValueDetail.Init(_bulletValueDetail);
    }
    public virtual void CreateEffectHit()
    {
        if(effectHitPrefab != null){
            GamePlayManager.Instance.CreateEffect(effectHitPrefab, transform.position, Quaternion.identity);
        }
    }
    public virtual void CreateExplosion(){
        if(explosionPrefab != null)
        {
            MyAudioManager.Instance.PlaySfx(GameInformation.Instance.sfxExplosion);
            ExplosionController _explosion = GamePlayManager.Instance.CreateBullet(explosionPrefab, transform.position, Quaternion.identity);
            _explosion.Init(bulletValueDetail);
            _explosion.Move();
        }else{
            Debug.LogError("explosionPrefab = null");
        }
    }
    public virtual void Move(){
        if(process_Move != null){
            StopCoroutine(process_Move);
        }
        process_Move = DoProcess_Move();
        StartCoroutine(process_Move);
    }
    IEnumerator DoProcess_Move()
    {   
        if(speed == 0){
            process_Move = null;
            yield break;
        }

        Vector3 _startPosition = transform.position;
        Vector3 _hitPoint = _startPosition + transform.forward * maxRangeMove;
        float _distance = Vector3.Distance(transform.position, _hitPoint);
        float _startingDistance = _distance;

        while (_distance > 0)
        {
            transform.position = Vector3.Lerp(_startPosition, _hitPoint, 1 - (_distance / _startingDistance));
            _distance -= speed * Time.deltaTime;
            yield return null;
        }

        transform.position = _hitPoint;

        SelfDestruction();

        // if (_madeImpact)
        // {
        //     MySimplePoolObjectController _effectHit = GamePlayManager.Instance.CreateEffect(effectHitPrefab, _hitPoint, Quaternion.LookRotation(_hitNormal));
        //     // _effectHit.transform.rotation = Quaternion.LookRotation(_hitNormal);
        // }
        // yield return new WaitForSeconds(_trailRenderer.time);
        // _bulletTrail.SelfDestruction();
    }
    void OnTriggerEnter(Collider _other) {
        OnEventTriggerEnter2D(_other);
    }
    public virtual void OnEventTriggerEnter2D(Collider _other) {
        // Debug.Log("OnEventTriggerEnter2D | Va chạm với: " + other.gameObject.name);
        if(_other.tag.Equals("Ground") || _other.tag.Equals("Obstacle")){
            if(bulletValueDetail.canExplosion){
                CreateExplosion();
            }else{
                CreateEffectHit();
            }
            SelfDestruction();
        }
    }
    // void OnCollisionEnter(Collision collision) {
    //     Debug.Log("OnCollisionEnter | Va chạm với: " + collision.gameObject.name);
    //     MySimplePoolObjectController _effectHit = GamePlayManager.Instance.CreateEffect(effectHitPrefab, transform.position, Quaternion.identity);
    // }
}
