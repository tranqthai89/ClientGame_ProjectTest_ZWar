using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameInformation", menuName="GameInfo/CoreGameInformation")]
public class GameInformation : ScriptableObject
{
    public static GameInformation Instance{
		get{
			if(ins == null){
				ins = Resources.Load<GameInformation>("GameInformation");
			}
			return ins;
		}
	}
	private static GameInformation ins;

	public MainCharInfo mainCharInfo;
	public List<MapInfo> listMapInfo;

	[Header("Audio Info")]
	public AudioClip bgm;
	public List<AudioClip> sfxListMainCharStep;
	public AudioClip sfxMainCharHit;
	public AudioClip sfxMainCharDie;
	public AudioClip sfxEnemyDie;
	public List<AudioClip> sfxListMachineGunShoot;
	public AudioClip sfxExplosion;
}
