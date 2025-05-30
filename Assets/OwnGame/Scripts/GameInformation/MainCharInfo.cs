using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MainCharInfo", menuName="GameInfo/MainCharInfo")]
public class MainCharInfo : ScriptableObject
{
    public int maxHealth;
    public float moveSpeed;
    public float rotationSpeed;
    public float jumpHeight;
    public float detectionRadius;

    public GunInfo gunMachineInfo;
}
