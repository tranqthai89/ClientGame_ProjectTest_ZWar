using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEnum;

public class EnemyAnimationController : MonoBehaviour
{
    [SerializeField] EnemyController enemyController;
    public int currentIdAnimation;
    public Animator animator;
    public System.Action onCreateDmg;

    public Enemy_StateAnimation currentStateAnimation{
        get{
            return (Enemy_StateAnimation) currentIdAnimation;
        }
        set{
            currentIdAnimation = (int) value;
        }
    }
    
    void Awake(){
        animator.keepAnimatorStateOnDisable = true;
    }

    public void SetAnimByState(Enemy_StateAnimation _state){
        if(currentStateAnimation != _state){
            animator.ResetTrigger(currentStateAnimation.ToString()); 
            currentStateAnimation = _state;
            animator.SetTrigger(currentStateAnimation.ToString());
        }
    }
    public void OnCreateDmg(){
        if(onCreateDmg != null){
            onCreateDmg();
        }
    }
    public void OnFinishAttack(){
        if(enemyController != null){
            if(enemyController.CurrentState != EnemyState.Idle){
                SetAnimByState(Enemy_StateAnimation.Idle);
                enemyController.CurrentState = EnemyState.Idle;
            }
        }
    }
}
