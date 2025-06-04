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

public class GameControl
{
    public GamePlayManager GamePlayManagerInstance{
        get{
            return GamePlayManager.Instance;
        }
    }
    public CollectionInfo collectionInfo;
    public GamePlayState currentState;

    public MainCharController mainChar;
    public MapController currentMap;

    public int indexMapInfo;

    public System.Action OnStartGame;
    public System.Action OnStartMap;
    public System.Action<long, int> OnStartNewWave;
    public System.Action<long, int> OnCurrentWaveFinished;
    public System.Action OnRunMapFinished;
    public System.Action OnSetDataWhenFinish;
    public System.Action OnWaveManagerFinished;
    public System.Action<EnemyController> OnEnemySpawned;
    public System.Action<EnemyController> OnEnemyDie;

    public GameControl(){
        collectionInfo = new CollectionInfo();
        indexMapInfo = 0;
        currentState = GamePlayState.PrepareToBattle;
        GamePlayManagerInstance.StartCoroutine(DoActionPrepareToBattle());
    }

    public void InitCallback(){
        ResetAllCallback();

        OnStartMap = ()=>{
            Debug.Log("Start Map");
            GamePlayManagerInstance.UIManager.RefreshAllUI();
            GamePlayManagerInstance.UIManager.ShowPanelStartGame();
        };

        OnStartGame = ()=>{
            currentState = GamePlayState.PlayGame;
            mainChar.Run();
            GamePlayManagerInstance.waveManager.Run();
        };
        OnStartNewWave = (_levelMap, _indexWave) =>{
            Debug.Log("Start New Wave: " + _levelMap + " - " + _indexWave);
            GamePlayManagerInstance.UIManager.RefreshUI_IndexWaveInfo();
        };
        OnCurrentWaveFinished = (_levelMap, _indexWave) => {
            Debug.Log("Wave Finished: " + _levelMap + " - " + _indexWave);
        };
        OnEnemySpawned = (_enemy)=>{
            Debug.Log("Spawn Enemy: " + _enemy.MyCharInfo.enemyType + " | pow : " + _enemy.MyCharInfo.power);
            GamePlayManagerInstance.currentGameControl.collectionInfo.totalEnemySpawned ++;
        };
        OnEnemyDie = (_enemy)=>{
            GamePlayManagerInstance.currentGameControl.collectionInfo.totalEnemyDie ++;

            GamePlayManagerInstance.UIManager.RefreshUI_EnemyInfo();
        };
        OnRunMapFinished = ()=>{
            Debug.Log("Map Finished");
            currentState = GamePlayState.Finished;

            if(mainChar.CurrentState != MainCharState.Die){
                GamePlayManagerInstance.UIManager.ShowPanelFinishGame();
            }else{
                GamePlayManagerInstance.UIManager.ShowPanelFailure();
            }
        };
        OnSetDataWhenFinish = ()=>{
            Debug.Log("Set Data When Finish");
            
            if(mainChar.CurrentState != MainCharState.Die){
                if(indexMapInfo + 1 < GameInformation.Instance.listMapInfo.Count){
                    indexMapInfo ++; // tăng level map lên
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
            GamePlayManagerInstance.UIManager.HideAllPanels();
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

        GamePlayManagerInstance.UIManager.HideAllPanels();

        collectionInfo = new CollectionInfo();
        if(GamePlayManagerInstance.mainCharPoolManager == null){
            GamePlayManagerInstance.mainCharPoolManager = new MySimplePoolManager();
        }
        if(GamePlayManagerInstance.effectPoolManager == null){
            GamePlayManagerInstance.effectPoolManager = new MySimplePoolManager();
        }
        if(GamePlayManagerInstance.bulletPoolManager == null){
            GamePlayManagerInstance.bulletPoolManager = new MySimplePoolManager();
        }
        if(GamePlayManagerInstance.enemyPoolManager == null){
            GamePlayManagerInstance.enemyPoolManager = new MySimplePoolManager();
        }
        if(GamePlayManagerInstance.mapPoolManager == null){
            GamePlayManagerInstance.mapPoolManager = new MySimplePoolManager();
        }

        InitCallback();

        yield return GamePlayManagerInstance.StartCoroutine(DoActionInitMap());
        yield return GamePlayManagerInstance.StartCoroutine(DoActionInitMainChar());
        yield return GamePlayManagerInstance.waveManager.Init(currentMap.MyMapInfo);
        
        if(OnStartGame != null){
            OnStartGame();
        }
    }
    protected IEnumerator DoActionInitMap(){
        yield return new WaitForEndOfFrame();
        if(currentMap != null && currentMap.MyMapInfo.level == GameInformation.Instance.listMapInfo[indexMapInfo].level){
            yield break;
        }
        if(currentMap == null || currentMap.MyMapInfo.level != GameInformation.Instance.listMapInfo[indexMapInfo].level){
            GamePlayManagerInstance.mapPoolManager.ClearAllObjectsNow();
        }

        currentMap = LeanPool.Spawn(GameInformation.Instance.listMapInfo[indexMapInfo].mapPrefab, GamePlayManagerInstance.transform);
        currentMap.Init(GameInformation.Instance.listMapInfo[indexMapInfo]);
        currentMap.onSelfDestruction = (_o) => {
            GamePlayManagerInstance.mapPoolManager.RemoveObjectsNow(_o);
        };
        GamePlayManagerInstance.mapPoolManager.AddObject(currentMap);
    }
    protected IEnumerator DoActionInitMainChar(){
        yield return new WaitForEndOfFrame();
        mainChar = LeanPool.Spawn(GameInformation.Instance.mainCharInfo.prefab, currentMap.mainCharSpawnPoint.position, currentMap.mainCharSpawnPoint.rotation);
        mainChar.Init(GameInformation.Instance.mainCharInfo);
        mainChar.onSelfDestruction = (_o) => {
            GamePlayManagerInstance.mainCharPoolManager.RemoveObjectsNow(_o);
        };
        GamePlayManagerInstance.mainCharPoolManager.AddObject(mainChar);

        GamePlayManagerInstance.virtualCamera.Follow = mainChar.transform;
        GamePlayManagerInstance.virtualCamera.LookAt = mainChar.transform;
    }
}