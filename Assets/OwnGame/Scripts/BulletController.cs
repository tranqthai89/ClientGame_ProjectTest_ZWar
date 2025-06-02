using System;
using System.Collections;
using System.Collections.Generic;
using DevToolkit;
using UnityEngine;
using GlobalClass;

public class BulletController : MySimplePoolObjectController
{
    [NonSerialized] public GlobalClass.BulletValueDetail bulletValueDetail;
    float speed;
    float maxRangeMove;

    [Header("Prefabs")]
    [SerializeField] ZWar.EffectController effectHitPrefab;

    IEnumerator process_Move;
    
    public override void StopAllActionNow()
    {
        base.StopAllActionNow();
        process_Move = null;
    }

    public void Init(BulletValueDetail _bulletValueDetail, float _speed, float _maxRangeMove)
    {
        if(bulletValueDetail == null){
            bulletValueDetail = new BulletValueDetail();
        }
        bulletValueDetail.Init(_bulletValueDetail);
        speed = _speed;
        maxRangeMove = _maxRangeMove;
    }

    public Coroutine Move(){
        if(process_Move != null){
            StopCoroutine(process_Move);
        }
        process_Move = DoProcess_Move();
        return StartCoroutine(process_Move);
    }
    IEnumerator DoProcess_Move()
    {   
        if(speed == 0){yield break;}

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
    void OnTriggerEnter(Collider other) {
        // Debug.Log("OnTriggerEnter | Va chạm với: " + other.gameObject.name);
        if(effectHitPrefab != null){
            ZWar.EffectController _effectHit = GamePlayManager.Instance.CreateEffect(effectHitPrefab, transform.position, Quaternion.identity);
        }
        if(other.tag.Equals("Obstacle") || other.tag.Equals("Obstacle")){
            SelfDestruction();
        }
    }
    // void OnCollisionEnter(Collision collision) {
    //     Debug.Log("OnCollisionEnter | Va chạm với: " + collision.gameObject.name);
    //     MySimplePoolObjectController _effectHit = GamePlayManager.Instance.CreateEffect(effectHitPrefab, transform.position, Quaternion.identity);
    // }
}
