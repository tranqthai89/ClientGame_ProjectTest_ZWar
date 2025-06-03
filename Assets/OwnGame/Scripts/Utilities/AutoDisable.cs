using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    public float time = 1f;
    public bool useUnScaleTime;

    private void OnEnable() {
        StartCoroutine (Disable ());
    }

    IEnumerator Disable (){
        if(useUnScaleTime){
            yield return new WaitForSecondsRealtime(time);
        }else{
            yield return new WaitForSeconds(time);
        }
        gameObject.SetActive (false);
    }
}
