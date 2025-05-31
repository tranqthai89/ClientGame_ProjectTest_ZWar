using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEnum;

[CreateAssetMenu(fileName = "MainCharInfo", menuName="GameInfo/MainCharInfo")]
public class MainCharInfo : CharInfo
{
    public override CharType charType => CharType.MainChar;

    public float jumpHeight;
    public float detectionRadius;

    public GunInfo gunMachineInfo;

    [Header("Prefab")]
    public MainCharController prefab;
}
