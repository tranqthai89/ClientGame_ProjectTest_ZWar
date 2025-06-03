using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEnum;

public class MainCharAnimationController : MonoBehaviour
{
    [SerializeField] MainChar_StateAnimation defaultStateAnimation = MainChar_StateAnimation.Idle;
    MainCharController mainCharController;
    int currentIdAnimation;
    public Animator animator;
    public System.Action onCreateDmg;

    public MainChar_StateAnimation currentStateAnimation{
        get{
            return (MainChar_StateAnimation) currentIdAnimation;
        }
        set{
            currentIdAnimation = (int) value;
        }
    }
    
    void Awake(){
        mainCharController = transform.parent.GetComponent<MainCharController>();
        if(mainCharController == null){
            Debug.LogError("EnemyController not found on parent of " + gameObject.name);
        }
        animator.keepAnimatorStateOnDisable = true;
        currentIdAnimation = (int) defaultStateAnimation;
    }

    public void SetAnimByState(MainChar_StateAnimation _state){
        if(currentStateAnimation != _state){
            animator.ResetTrigger(currentStateAnimation.ToString()); 
            currentStateAnimation = _state;
            animator.SetTrigger(currentStateAnimation.ToString());
        }
    }
}
