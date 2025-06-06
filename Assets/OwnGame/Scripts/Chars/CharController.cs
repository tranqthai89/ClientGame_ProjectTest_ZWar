using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEnum;
using DevToolkit;

public class CharController : MySimplePoolObjectController
{
    public GamePlayManager GamePlayManagerInstance{
        get{
            return GamePlayManager.Instance;
        }
    }
    public virtual CharType charType => CharType.Unknown;

    [SerializeField] protected Vector3 posLocalDetect;
    public Vector3 PosOfDetect{
        get{
            return  transform.position + posLocalDetect;
        }
    }

    public int CurrentHp{get;set;}
    public bool CanBeDamaged{get;set;} // biến này để set logic nhận damge, khi = false thì chỉ có hiệu ứng xảy ra nhưng ko gây damage
    public bool IsInstalled {get;set;}
    public virtual bool IsRunning {get;}
    public System.Action<CharController> OnDie;

    public override void ResetData(){
        base.ResetData();
        IsInstalled = false;
        CanBeDamaged = false;

        OnDie = null;
    }
    public virtual void Run(){
        CanBeDamaged = true;
    }
    public virtual void OnEventTriggerEnter2D(Collider _other) {}
}
