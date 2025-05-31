using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEnum;
using DevToolkit;

public class CharController : MySimplePoolObjectController
{
    public virtual CharType charType => CharType.Unknown;

    public int CurrentHp{get;set;}

    public bool CanBeDamaged{get;set;} // biến này để set logic nhận damge, khi = false thì chỉ có hiệu ứng xảy ra nhưng ko gây damage
    public bool IsInstalled {get;set;}
    public System.Action<CharController> OnDie;

    public override void ResetData(){
        base.ResetData();
        IsInstalled = false;
        CanBeDamaged = false;

        OnDie = null;
    }
}
