using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using DevToolkit;

public class MyAudioManager : MonoBehaviour {

	public static MyAudioManager Instance{
		get{
			if(ins == null){
				MyAudioManager _prefab = Resources.Load<MyAudioManager>("AudioManager");
				ins = Instantiate(_prefab);
				DontDestroyOnLoad (ins.gameObject);
			}
			return ins;
		}
	}
	static MyAudioManager ins;

	[SerializeField] AudioSource musicSource;
	[SerializeField] Transform pool;

	[Header("Prefabs")]
	[SerializeField] SfxObjectController sfxObjectPrefab;

	MySimplePoolManager sfxObjectPoolManager;
	
	void Awake() {
        if (ins != null && ins != this){
            Destroy(this.gameObject);
            return;
        }
		ins = this;
        DontDestroyOnLoad(this.gameObject);
		
		sfxObjectPoolManager = new MySimplePoolManager(15);
    }

	#region Music, Playback
	public void SetMusic(AudioClip _audioClip){
		if (_audioClip.Equals (musicSource.clip) && musicSource.isPlaying) {
			return;
		}
		musicSource.clip = _audioClip;
	}
	public void PlayMusic (AudioClip _audioClip){
		if (_audioClip == null) {
			#if TEST
			Debug.LogError ("PlayMusic | Audio Clip sound not found");
			#endif
			return;
		}

		SetMusic(_audioClip);

		if(DataManager.Instance.musicStatus == 0){
			return;
		}
		musicSource.Play ();
	}

	public void RestartMusic (){
		if(musicSource.clip == null){
			return;
		}
		if(DataManager.Instance.musicStatus == 0){
			return;
		}
		musicSource.Play ();
	}

	public void PauseMusic (){
		if(musicSource.clip == null){
			return;
		}
		musicSource.Pause ();
	}

	public void ResumeMusic (){
		if(musicSource.clip == null){
			return;
		}
		if(DataManager.Instance.musicStatus == 0){
			return;
		}
		musicSource.volume = 1f;
		if(musicSource.isPlaying){
			musicSource.UnPause ();
		}else{
			musicSource.Play();
		}
	}

	public void StopMusic (){
		if(musicSource.clip == null){
			return;
		}
		musicSource.Stop ();
	}
	#endregion

	#region SFX
	public void PlaySfx(AudioClip _audioClip){
		if(DataManager.Instance.sfxStatus == 0){
			return;
		}
		if (_audioClip == null) {
			#if TEST
			Debug.LogError ("PlaySfx | Audio Clip sound not found");
			#endif
			return;
		}
		// Spawn object chứa audioclip
		SfxObjectController _sfxObject = LeanPool.Spawn(sfxObjectPrefab, Vector3.zero, Quaternion.identity, pool.transform);
		sfxObjectPoolManager.AddObject(_sfxObject);
		_sfxObject.Play(_audioClip);
	}

	public void StopAllSfx (){
		sfxObjectPoolManager.ClearAllObjectsNow();
	}
	#endregion

	public void PauseAll(){
		PauseMusic();
		StopAllSfx();
	}

	public void ResumeAll(){
		ResumeMusic();
	}

	public void StopAll(){
		StopMusic();
		StopAllSfx();
	}
}
