using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEnum;
using GlobalClass;

[CreateAssetMenu(fileName = "EnemyInfo", menuName="GameInfo/EnemyInfo")]
public class EnemyInfo : CharInfo
{
    public override CharType charType => CharType.EnemyChar;

    public EnemyType enemyType;
    public BulletValueDetail bulletValueDetail;

    [Header("Summon Settings")]
    public double power;

    [Header("Prefabs")]
    public EnemyController prefab;
}
