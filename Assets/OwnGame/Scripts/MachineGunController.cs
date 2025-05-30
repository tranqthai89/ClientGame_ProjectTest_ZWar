using System.Collections;
using System.Collections.Generic;
using DevToolkit;
using Unity.Mathematics;
using UnityEngine;

public class MachineGunController : GunController
{
    [SerializeField]
    private ParticleSystem particleSystem_Shoot;

    [SerializeField]
    private Transform bulletSpawnPoint;
    
    [Header("Prefabs")]
    // [SerializeField] private MySimplePoolObjectController bulletTrailPrefab;
    // [SerializeField] private MySimplePoolObjectController effectHitPrefab;
    [SerializeField] private BulletController bulletPrefab;

    public override void Shoot()
    {
        if (lastShootTime + shootDelay < Time.time)
        {
            if(particleSystem_Shoot != null)
            {
                particleSystem_Shoot.Play();
            }

            SpawnBullet();

            // Vector3 direction = bulletSpawnPoint.transform.forward;
            // MySimplePoolObjectController _bulletTrail = GamePlayManager.Instance.CreateEffect(bulletTrailPrefab, bulletSpawnPoint.position, Quaternion.identity);

            // if (Physics.Raycast(bulletSpawnPoint.position, direction, out RaycastHit hit, float.MaxValue))
            // {
            //     StartCoroutine(SpawnTrail(_bulletTrail, hit.point, hit.normal, true));
            // }
            // else
            // {
            //     StartCoroutine(SpawnTrail(_bulletTrail, bulletSpawnPoint.position + direction * 100, Vector3.zero, false));
            // }

            lastShootTime = Time.time;
        }
    }

    private void SpawnBullet(){
        BulletController _bullet = GamePlayManager.Instance.CreateBullet(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
        _bullet.Init(gunInfo.bulletValueDetail, bulletSpeed, gunInfo.rangeShot);
        _bullet.Move();
    }

    // private IEnumerator SpawnTrail(MySimplePoolObjectController _bulletTrail, Vector3 _hitPoint, Vector3 _hitNormal, bool _madeImpact)
    // {
    //     Vector3 _startPosition = _bulletTrail.transform.position;
    //     // Vector3 direction = (_hitPoint - _bulletTrail.transform.position).normalized;
    //     TrailRenderer _trailRenderer = _bulletTrail.GetComponent<TrailRenderer>();

    //     float distance = Vector3.Distance(_bulletTrail.transform.position, _hitPoint);
    //     float startingDistance = distance;

    //     while (distance > 0)
    //     {
    //         _bulletTrail.transform.position = Vector3.Lerp(_startPosition, _hitPoint, 1 - (distance / startingDistance));
    //         distance -= Time.deltaTime * bulletSpeed;
    //         yield return null;
    //     }

    //     _bulletTrail.transform.position = _hitPoint;

    //     if (_madeImpact)
    //     {
    //         MySimplePoolObjectController _effectHit = GamePlayManager.Instance.CreateEffect(effectHitPrefab, _hitPoint, Quaternion.LookRotation(_hitNormal));
    //         // _effectHit.transform.rotation = Quaternion.LookRotation(_hitNormal);
    //     }
    //     yield return new WaitForSeconds(_trailRenderer.time);
    //     _bulletTrail.SelfDestruction();
    // }
}
