using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalClass
{
    [Serializable] public class BulletValueDetail{
        public int damage;
        public BulletValueDetail(){ }
        public void Init(BulletValueDetail _other)
        {
            damage = _other.damage;    
        }
    }
}
