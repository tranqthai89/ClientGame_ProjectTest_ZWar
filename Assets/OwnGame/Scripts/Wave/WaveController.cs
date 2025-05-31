using System.Collections;
using System.Collections.Generic;
using GlobalEnum;
using UnityEngine;

public class WaveController
{
    public GamePlayManager GamePlayManagerInstance{
        get{
            return GamePlayManager.Instance;
        }
    }
    public WaveManager WaveManagerInstance{
        get{
            return GamePlayManagerInstance.waveManager;
        }
    }
    IEnumerator actionRun;
    WaveManager.WaveCatchedInfo waveCatchedInfo;
    int indexWave;

    public void Init(WaveManager.WaveCatchedInfo _waveCatchedInfo, int _indexWave){
        waveCatchedInfo = _waveCatchedInfo;
        indexWave = _indexWave;
    }
    public bool IsRunning(){
        if(actionRun != null){
            return true;
        }
        return false;
    }
    public Coroutine Run(){
        if(actionRun != null){
            return null;
        }
        actionRun = DoActionRun();
        return WaveManagerInstance.StartCoroutine(actionRun);
    }
    public virtual IEnumerator DoActionRun(){
        if(waveCatchedInfo.listMiniWaveCatched.Count == 0){
            actionRun = null;
            yield break;
        }
        Debug.Log("Start Wave " + indexWave + " | pow: " + waveCatchedInfo.powCatched);

        bool _firstSummonned = false;

        Vector3 _posEnemy = Vector3.zero;
        float _tmpPosZ = 0f;
        EnemyController _tmpMonsterPrefab = null;
        EnemyController _tmpEnemyController = null;
        List<WaveManager.EnemyInWaveCatched> _listEnemyInWaveCollected = new List<WaveManager.EnemyInWaveCatched>();
        WaveManager.EnemyInWaveCatched _currentEnemyInWave = null;

        // for(int i = 0; i < waveCatchedInfo.listMiniWaveCatched.Count; i ++){
        //     // - Create enemy - //
        //     Debug.Log("Start MiniWave " + indexWave + " - " + i + " | pow: " + waveCatchedInfo.listMiniWaveCatched[i].powCatched);

        //     while(waveCatchedInfo.listMiniWaveCatched[i].listEnemyInWaveCatched.Count > 0 || _listEnemyInWaveCollected.Count > 0){
        //         yield return null;
        //         if(WaveManagerInstance.CurrentState == WaveState.Finished){
        //             break;
        //         }
        //         if(_listEnemyInWaveCollected.Count == 0){
        //             _listEnemyInWaveCollected.Add(waveCatchedInfo.listMiniWaveCatched[i].listEnemyInWaveCatched[0]);
        //             waveCatchedInfo.listMiniWaveCatched[i].listEnemyInWaveCatched.RemoveAt(0);

        //             for(int j = 0; j < waveCatchedInfo.listMiniWaveCatched[i].listEnemyInWaveCatched.Count; j ++){
        //                 _listEnemyInWaveCollected.Add(waveCatchedInfo.listMiniWaveCatched[i].listEnemyInWaveCatched[j]);

        //                 waveCatchedInfo.listMiniWaveCatched[i].listEnemyInWaveCatched.RemoveAt(j);
        //                 j--;
        //                 continue;
        //             }
        //         }
        //         if(!_firstSummonned){
        //             _timeSummon += Time.deltaTime;
        //         }else{
        //             if(GamePlayManagerInstance.enemyPoolManager.listObjects.Count == 0){
        //                 if(_timeSummon + Time.deltaTime < _nextTimeToSummon){
        //                     _timeSummon += Time.deltaTime;
        //                 }else{
        //                     _timeSummon = _nextTimeToSummon;
        //                 }
        //             }else{
        //                 _timeSummon += Time.deltaTime;
        //             }
        //         }
        //         if(_timeSummon >= _nextTimeToSummon){
        //             for (int j = 0; j < _listEnemyInWaveCollected.Count; j++){
        //                 // - Giới hạn tối đa trên màn hình chỉ 50 con quái
        //                 if(GamePlayManagerInstance.enemyPoolManager.listObjects.Count > 50){
        //                     yield return null;
        //                     continue;
        //                 }
        //                 _currentEnemyInWave = _listEnemyInWaveCollected[j];

        //                 _tmpMonsterPrefab = (EnemyController) _currentEnemyInWave.enemyInfo.modelPrefab.Load();

        //                 // - Set vị trí của enemy cách vị trí của main 5 unit theo hình tròn
        //                 int _rdAngle = MyConstant.random.Next(0, 360);
        //                 Quaternion _desiredRot = Quaternion.Euler (0, 0, _rdAngle);
        //                 _posEnemy = GamePlayManagerInstance.placeHolder_MainChar.transform.position;
        //                 Vector3 _velocity = new Vector3 (0, 4f, 0f); // set theo trục y
        //                 _posEnemy += _desiredRot * _velocity;
        //                 _posEnemy.z = _tmpPosZ;
                        
        //                 // - Spawn Enemy
        //                 _tmpEnemyController = GamePlayManagerInstance.CreateSimplePoolObject(_tmpMonsterPrefab, _posEnemy, GamePlayManagerInstance.enemyPoolManager, GamePlayManagerInstance.enemyContainer);
        //                 _tmpEnemyController.Init(_currentEnemyInWave.enemyInfo, _currentEnemyInWave.level);
        //                 _tmpEnemyController.Run();
                        
        //                 if(GamePlayManagerInstance.currentGameModeControl.OnEnemySpawned != null){
        //                     GamePlayManagerInstance.currentGameModeControl.OnEnemySpawned(_tmpEnemyController);
        //                 }
                        
        //                 _tmpPosZ += 0.1f;
        //                 _firstSummonned = true;
        //             }

        //             _listEnemyInWaveCollected.Clear();
        //         }
        //     }
        //     yield return new WaitUntil(()=>GamePlayManagerInstance.enemyPoolManager.listObjects.Count == 0);
        // }
        
        actionRun = null;
        yield break;
    }
}
