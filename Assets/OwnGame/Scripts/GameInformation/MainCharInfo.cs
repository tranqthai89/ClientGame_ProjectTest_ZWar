using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEnum;
using GlobalClass;

[CreateAssetMenu(fileName = "MainCharInfo", menuName="GameInfo/MainCharInfo")]
public class MainCharInfo : CharInfo
{
    public override CharType charType => CharType.MainChar;

    public float jumpHeight;
    public float detectionRadius;

    public GunValueDetail gunMachineValueDetail;
    public GunValueDetail gunCreateExplosionValueDetail;

    [Header("Prefab")]
    public MainCharController prefab;
}
