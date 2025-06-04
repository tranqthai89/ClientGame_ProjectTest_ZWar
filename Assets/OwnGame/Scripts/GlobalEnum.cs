using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalEnum{
    public enum GunType{
        Unknown = 0,
        MachineGun,
        GunBaseOnAnimation
    }
    public enum BulletType{
        Normal
	}
    public enum CharType{
        Unknown = 0,
        MainChar,
        EnemyChar
    }
    public enum EnemyType{
        Enemy_00 = 0,
        Enemy_01,
    }
    public enum MainCharState{
        Idle, Move, Die
    }
    public enum EnemyState{
        Idle, Move, Die, Attack
    }
    public enum Enemy_StateAnimation{
        Idle, Move, Die, Attack
    }
    public enum MainChar_StateAnimation{
        Idle, Move, Die
    }
    public enum WaveState{
        None, CreateWave, Finished
    }
    public enum GamePlayState{
        PrepareToBattle, PlayGame, Finished
    }
}