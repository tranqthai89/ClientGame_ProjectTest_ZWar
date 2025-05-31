using System.Collections;
using System.Collections.Generic;
using DevToolkit;
using UnityEngine;

namespace ZWar
{   
    public class EffectController : MySimplePoolObjectController
    {
        protected IEnumerator process_Follow;

        public override void StopAllActionNow(){
            base.StopAllActionNow();
            process_Follow = null;
        }

        public Coroutine SetFollow(GameObject _target, Vector2 _localPos){
            if(process_Follow == null){
                process_Follow = DoProcess_Follow(_target, _localPos);
                return StartCoroutine(process_Follow);
            }
            return null;
        }

        IEnumerator DoProcess_Follow(GameObject _target, Vector2 _localPos){
            while(true){
                yield return null;
                if(_target != null && _target.activeSelf){
                    transform.position = _target.transform.position + (Vector3)_localPos;
                }
            }
        }
    }
}
