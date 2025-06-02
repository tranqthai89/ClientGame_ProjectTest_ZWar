using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GlobalEnum{
    public enum GunType{
        Unknown = 0,
        MachineGun,
        GunBaseOnAnimation
    }
    public enum CharType{
        Unknown = 0,
        MainChar,
        EnemyChar
    }
    public enum EnemyType{
        Enemy_00 = 0,
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
    public enum WaveState{
        None, CreateWave, Finished
    }
    public enum GamePlayState{
        PrepareToBattle, PlayGame, Finished
    }
}