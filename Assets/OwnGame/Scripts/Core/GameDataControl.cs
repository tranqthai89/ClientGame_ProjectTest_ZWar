using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using GlobalClass;
using GlobalEnum;
using Lean.Pool;
using System.Collections;
using DevToolkit;

public class GameDataControl
{
    public GamePlayManager GamePlayManagerInstance{
        get{
            return GamePlayManager.Instance;
        }
    }
    public CollectionInfo collectionInfo;
    public GamePlayState currentState;

    public int currentLevelMap;

    public System.Action OnStartGame;
    public System.Action OnStartMap;
    public System.Action<long, int> OnStartNewWave;
    public System.Action<long, int> OnCurrentWaveFinished;
    public System.Action OnRunMapFinished;
    public System.Action OnSetDataWhenFinish;
    public System.Action OnWaveManagerFinished;
    public System.Action<EnemyController> OnEnemySpawned;
    public System.Action<EnemyController> OnEnemyDie;

    public GameDataControl(){
        collectionInfo = new CollectionInfo();
        currentLevelMap = 0;
        currentState = GamePlayState.PrepareToBattle;
        GamePlayManagerInstance.StartCoroutine(DoActionPrepareToBattle());
    }

    public void InitCallback(){
        ResetAllCallback();

        OnStartGame = ()=>{
            currentState = GamePlayState.PlayGame;
            GamePlayManagerInstance.waveManager.Run();
        };
        OnStartNewWave = (_levelMap, _indexWave) =>{
            Debug.Log("Start New Wave: " + _levelMap + " - " + _indexWave);
        };
        OnCurrentWaveFinished = (_levelMap, _indexWave) => {
            Debug.Log("Wave Finished: " + _levelMap + " - " + _indexWave);
        };
        OnEnemySpawned = (_enemy)=>{
            Debug.Log("Spawn Enemy: " + _enemy.MyCharInfo.enemyType + " | pow : " + _enemy.MyCharInfo.power);
            GamePlayManagerInstance.CurrentGameDataControl.collectionInfo.totalEnemySpawned ++;
        };
        OnEnemyDie = (_enemy)=>{
            GamePlayManagerInstance.CurrentGameDataControl.collectionInfo.totalEnemyDie ++;
        };
        OnRunMapFinished = ()=>{
            Debug.Log("Map Finished");
            currentState = GamePlayState.Finished;
        };
        OnSetDataWhenFinish = ()=>{
            Debug.Log("Set Data When Finish");
            
            if(GamePlayManagerInstance.MainChar.CurrentState != MainCharState.Die){
                if(currentLevelMap + 1 < GameInformation.Instance.listMapInfo.Count){
                    currentLevelMap ++; // tăng level map lên
                }
            }

            collectionInfo.ResetData();
            GamePlayManagerInstance.mainCharPoolManager.ClearAllObjectsNow();
            GamePlayManagerInstance.effectPoolManager.ClearAllObjectsNow();
            GamePlayManagerInstance.bulletPoolManager.ClearAllObjectsNow();
            GamePlayManagerInstance.enemyPoolManager.ClearAllObjectsNow();
        };
        OnWaveManagerFinished = ()=>{
            // Khi kết thúc thì cho lặp lại từ đầu, data đã xử lý ở trên
            Debug.Log("Wave Manager Finish");
            GamePlayManagerInstance.StartCoroutine(DoActionPrepareToBattle());
        };
    }
    public void ResetAllCallback(){
        OnStartGame = null;
        OnStartMap = null;
        OnStartNewWave = null;
        OnCurrentWaveFinished = null;
        OnRunMapFinished = null;
        OnSetDataWhenFinish = null;
        OnWaveManagerFinished = null;
        OnEnemySpawned = null;
        OnEnemyDie = null;
    }
    protected IEnumerator DoActionPrepareToBattle(){
        currentState = GamePlayState.PrepareToBattle;

        collectionInfo = new CollectionInfo();
        GamePlayManagerInstance.mainCharPoolManager = new MySimplePoolManager();
        GamePlayManagerInstance.effectPoolManager = new MySimplePoolManager();
        GamePlayManagerInstance.bulletPoolManager = new MySimplePoolManager();
        GamePlayManagerInstance.enemyPoolManager = new MySimplePoolManager();

        InitCallback();

        yield return GamePlayManagerInstance.StartCoroutine(DoActionInitMainChar());
        yield return GamePlayManagerInstance.waveManager.Init(GameInformation.Instance.listMapInfo[currentLevelMap]);
        
        if(OnStartGame != null){
            OnStartGame();
        }
    }
    protected IEnumerator DoActionInitMainChar(){
        yield return new WaitForEndOfFrame();
        GamePlayManagerInstance.MainChar = LeanPool.Spawn(GameInformation.Instance.mainCharInfo.prefab, Vector3.zero, Quaternion.identity);
        GamePlayManagerInstance.MainChar.Init(GameInformation.Instance.mainCharInfo);
        GamePlayManagerInstance.MainChar.onSelfDestruction = (_o) => {
            GamePlayManagerInstance.mainCharPoolManager.RemoveObjectsNow(_o);
        };
        GamePlayManagerInstance.mainCharPoolManager.AddObject(GamePlayManagerInstance.MainChar);

        GamePlayManagerInstance.virtualCamera.Follow = GamePlayManagerInstance.MainChar.transform;
        GamePlayManagerInstance.virtualCamera.LookAt = GamePlayManagerInstance.MainChar.transform;
    }
}