using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager
{
    const string SAVE_KEY_SFX_STATUS = "SfxStatus";
    const string SAVE_KEY_MUSIC_STATUS = "MusicStatus";
    public static DataManager Instance {
		get {
            if(ins == null) {
                ins = new DataManager();
            }
			return ins;
		}
	}
    private static DataManager ins;

    public int musicStatus{ // trạng thái tắt bật nhạc nền (0: tắt, 1 : bật)
		get{
			if(_musicStatus == -1){
				_musicStatus = PlayerPrefs.GetInt(SAVE_KEY_MUSIC_STATUS, 1);
			}
			return _musicStatus;
		}set{
			_musicStatus = value;
			PlayerPrefs.SetInt(SAVE_KEY_MUSIC_STATUS, _musicStatus);
		}
	} 
	private int _musicStatus = -1;
    public int sfxStatus{ // trạng thái tắt bật âm thanh hiệu ứng (0: tắt, 1 : bật)
		get{
			if(_sfxStatus == -1){
				_sfxStatus = PlayerPrefs.GetInt(SAVE_KEY_SFX_STATUS, 1);
			}
			return _sfxStatus;
		}set{
			_sfxStatus = value;
			PlayerPrefs.SetInt(SAVE_KEY_SFX_STATUS,_sfxStatus);
		}
	} 
	private int _sfxStatus = -1;

    public DataManager() {}
}
