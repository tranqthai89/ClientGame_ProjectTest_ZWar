using System.Collections;
using System.Collections.Generic;
using GlobalClass;
using UnityEngine;

[CreateAssetMenu(fileName = "GunInfo", menuName="GameInfo/GunInfo")]
public class GunInfo : ScriptableObject
{
    public BulletValueDetail bulletValueDetail;
    public float atkSpeed;
    public float bulletSpeed;
    public float rangeShot;
}
