using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEnum;

public class CharInfo : ScriptableObject
{
    public virtual CharType charType => CharType.Unknown;
    public int maxHp;
    public float moveSpeed;
    public float rotationSpeed;
}
