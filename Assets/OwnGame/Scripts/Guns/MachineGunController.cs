using System.Collections;
using System.Collections.Generic;
using DevToolkit;
using Unity.Mathematics;
using UnityEngine;
using GlobalEnum;
using GlobalClass;

public class MachineGunController : GunController
{
    public override GunType gunType => GunType.MachineGun;

    [SerializeField]
    private ParticleSystem particleSystem_Shoot;
    
    [Header("Prefabs")]
    // [SerializeField] private MySimplePoolObjectController bulletTrailPrefab;
    // [SerializeField] private MySimplePoolObjectController effectHitPrefab;
    [SerializeField] private BulletController bulletPrefab;

    int indexSfxShoot = 0;

    public override void Init(GunValueDetail _gunValueDetail)
    {
        base.Init(_gunValueDetail);
        indexSfxShoot = 0;
    }

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
        MyAudioManager.Instance.PlaySfx(GameInformation.Instance.sfxListMachineGunShoot[indexSfxShoot]);
        indexSfxShoot++;
        if(indexSfxShoot >= GameInformation.Instance.sfxListMachineGunShoot.Count)
        {
            indexSfxShoot = 0;
        }

        BulletController _bullet = GamePlayManager.Instance.CreateBullet(bulletPrefab, bulletSpawnPoint.position, myCompass.rotation);
        _bullet.Init(GunValueDetail.bulletValueDetail, bulletSpeed, GunValueDetail.rangeShot);
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
