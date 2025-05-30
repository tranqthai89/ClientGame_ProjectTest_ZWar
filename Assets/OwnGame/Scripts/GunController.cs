using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    protected float shootDelay;
    protected float bulletSpeed;
    protected float lastShootTime;
    protected GunInfo gunInfo;

    public virtual void Init(GunInfo _gunInfo)
    {
        if (_gunInfo == null)
        {
            Debug.LogError("GunInfo is null!");
            return;
        }
        gunInfo = _gunInfo;
        shootDelay = 1f / gunInfo.atkSpeed;
        bulletSpeed = gunInfo.bulletSpeed;

        lastShootTime = Time.time + shootDelay; // Initialize to allow immediate shooting
    }

    public virtual void Shoot(){}
}
