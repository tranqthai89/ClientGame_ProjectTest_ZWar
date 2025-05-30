using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

namespace DevToolkit
{
	public class MySimplePoolObjectController : MonoBehaviour, IPoolable {

		[SerializeField] bool autoSelfDestruction;
		public float timeAutoSelfDestruction;

		public System.Action<MySimplePoolObjectController> onSelfDestruction;

		public virtual void ResetData(){}
		
		protected bool hasSpawned;
		
		public virtual IEnumerator DoActionAutoSelfDestruction(){
			yield return new WaitForSecondsRealtime(timeAutoSelfDestruction);
			SelfDestruction();
		}

		public virtual void SelfDestruction(){
			if(!hasSpawned){
				return;
			}
	//		Debug.Log ("SelfDestruction");
			StopAllActionNow();
			if(onSelfDestruction != null){
				onSelfDestruction(this);
				onSelfDestruction = null;
			}
			LeanPool.Despawn(gameObject);
		}

		public virtual void StopAllActionNow(){
			StopAllCoroutines();
			// LeanTween.cancel(gameObject);
		}

		public void OnSpawn(){
			hasSpawned = true;
			if(autoSelfDestruction){
				StartCoroutine(DoActionAutoSelfDestruction());
			}
		}

		public void OnDespawn(){
			hasSpawned = false;
			ResetData ();
		}

		protected void OnDestroy(){
			StopAllCoroutines();
		}
	}
}