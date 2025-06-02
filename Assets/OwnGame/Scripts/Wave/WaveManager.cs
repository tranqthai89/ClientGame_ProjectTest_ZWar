using System;
using System.Collections;
using System.Collections.Generic;
using GlobalEnum;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public GamePlayManager GamePlayManagerInstance{
        get{
            return GamePlayManager.Instance;
        }
    }

    #region Inner Classes
    public class WaveCatchedInfo{
        public List<MiniWaveCatchedInfo> listMiniWaveCatched;
        public double powCatched;
        public WaveCatchedInfo(){
            listMiniWaveCatched = new List<MiniWaveCatchedInfo>();
            powCatched = 0;
        }
        public void AddMiniWaveCatched(MiniWaveCatchedInfo _miniWaveCatched){
            listMiniWaveCatched.Add(_miniWaveCatched);
            powCatched += _miniWaveCatched.powCatched;
        }
    }
    public class MiniWaveCatchedInfo{
        public List<EnemyInWaveCatched> listEnemyInWaveCatched;
        public double powCatched;
        public MiniWaveCatchedInfo(){
            listEnemyInWaveCatched = new List<EnemyInWaveCatched>();
            powCatched = 0;
        }
        public void AddEnemyCatched(EnemyInWaveCatched _enemyCatched){
            listEnemyInWaveCatched.Add(_enemyCatched);
            powCatched += _enemyCatched.powCatched;
        }
    }
    public class EnemyInWaveCatched{
        public EnemyInfo enemyInfo;
        public double powCatched;

        public EnemyInWaveCatched(EnemyInfo _enemyInfo, double _powCatched){
            enemyInfo = _enemyInfo;
            powCatched = _powCatched;
        }
    }
    #endregion

    public List<Transform> listSpawnPoints;
    
    public MapInfo CurrentMapInfo{get;set;}
    public WaveState CurrentState{get;set;}
    public List<WaveCatchedInfo> ListWaveCatchedInfo{get;set;}
    public WaveController CurrentWave{get;set;}

    public long LevelMap{get;set;} // Mức độ của bản đồ hiện tại
    public int IndexWave{get;set;}

    IEnumerator process_Run;
    public bool IsRunning{
        get{
            if(process_Run != null){
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        ResetData();
    }
    public void ResetData(){
        StopAllCoroutines();
        process_Run = null;

        CurrentState = WaveState.None;
        IndexWave = 0;

        if(ListWaveCatchedInfo == null){
            ListWaveCatchedInfo = new List<WaveCatchedInfo>();
        }else{
            ListWaveCatchedInfo.Clear();
        }
    }

    public Coroutine Init(MapInfo _mapInfo){
        if (_mapInfo == null)
        {
            Debug.LogError("MapInfo is null!");
            return null;
        }
        CurrentMapInfo = _mapInfo;
        CurrentState = WaveState.None;
        LevelMap = _mapInfo.level;
        IndexWave = 0;

        if(ListWaveCatchedInfo == null){
            ListWaveCatchedInfo = new List<WaveCatchedInfo>();
        }else{
            ListWaveCatchedInfo.Clear();
        }
        
        return StartCoroutine(DoProcessInit());
    }
    IEnumerator DoProcessInit(){
        CatchWave();
        yield break;
    }
    void CatchWave(){
        string _functionName = "WaveManager | CatchWave";
        int _startHugeWave = 0;
        for(int _indexWave = 0; _indexWave < CurrentMapInfo.totalWaves; _indexWave ++){
            if(_indexWave == CurrentMapInfo.totalWaves - 1){ // final Wave
                // final Wave
                _startHugeWave = GamePlayManagerInstance.currentGameControl.collectionInfo.totalEnemy;
            }
            
            WaveCatchedInfo _tmpWave = GetWaveCatched(_indexWave, CurrentMapInfo.listEnemyRandom);
            if(_tmpWave != null && _tmpWave.listMiniWaveCatched.Count > 0){
                ListWaveCatchedInfo.Add(_tmpWave);
            }else{
                Debug.LogError("[Error] " + _functionName + ": can not catch wave at index: " + _indexWave);
            }
        }

        GamePlayManagerInstance.currentGameControl.collectionInfo.ratioStartFinalWave = (float) _startHugeWave / (float) GamePlayManagerInstance.currentGameControl.collectionInfo.totalEnemy;

        Debug.Log("totalEnemy: " + GamePlayManagerInstance.currentGameControl.collectionInfo.totalEnemy + " | startHugeWave: " + _startHugeWave);
    }
    WaveCatchedInfo GetWaveCatched(int _indexWave, List<MapInfo.EnemyInWave> _listEnemyRandom){
        if(_listEnemyRandom == null || _listEnemyRandom.Count == 0){
            Debug.LogError("listEnemyRandom == null || listEnemyRandom.Count == 0");
            return null;
        }
        WaveCatchedInfo _tmpWave = new WaveCatchedInfo();
        // - Lấy pow của wave này
        long _powWave = Mathf.CeilToInt(CurrentMapInfo.GetPowerWave(_indexWave));
        long _miniWavePow = CurrentMapInfo.GetPowerMiniWave(_indexWave);
        int _totalMiniWaves = (int) Math.Ceiling((double) _powWave / _miniWavePow);

        for(int i = 0; i < _totalMiniWaves; i ++){
            MiniWaveCatchedInfo _miniWaveCatched = new MiniWaveCatchedInfo();
            List<int> _listIndexReject = new List<int>();
            while(_miniWaveCatched.powCatched < _miniWavePow){
                int _rdIndex = (int) UnityEngine.Random.Range(0, _listEnemyRandom.Count);
                // ---- Kiểm tra xem đủ số lượng trong mini wave đó chưa ---- //
                if(_listEnemyRandom[_rdIndex].limitAppearInWave == 0){
                    _listIndexReject.Add(_rdIndex);
                    if(_listIndexReject.Count >= _listEnemyRandom.Count){
                        break;
                    }else{
                        continue;
                    }
                }else if(_listEnemyRandom[_rdIndex].limitAppearInWave > 0){
                    int _numExist = 0;
                    for(int j = 0; j < _miniWaveCatched.listEnemyInWaveCatched.Count; j ++){
                        if(_miniWaveCatched.listEnemyInWaveCatched[j].enemyInfo.enemyType == _listEnemyRandom[_rdIndex].enemyInfo.enemyType){
                            _numExist ++;
                        }
                    }
                    if(_numExist >= _listEnemyRandom[_rdIndex].limitAppearInWave){
                        _listIndexReject.Add(_rdIndex);
                        if(_listIndexReject.Count >= _listEnemyRandom.Count){
                            break;
                        }else{
                            continue;
                        }
                    }
                }
                // ----------------------------------------------------------- //
                
                double _powEnemy = _listEnemyRandom[_rdIndex].enemyInfo.power;

                EnemyInWaveCatched _enemyCatched = new EnemyInWaveCatched(_listEnemyRandom[_rdIndex].enemyInfo, _powEnemy);
                _miniWaveCatched.AddEnemyCatched(_enemyCatched);
                
                GamePlayManagerInstance.currentGameControl.collectionInfo.totalEnemy ++;
            }

            _tmpWave.AddMiniWaveCatched(_miniWaveCatched);
        }        
        return _tmpWave;
    }

    public Coroutine Run(){
        if(CurrentMapInfo == null){
            #if TEST
            Debug.LogError("CurrentMapInfo is NULL!");
            #endif
            return null;
        }
        if(process_Run != null){
            #if TEST
            Debug.LogError("actionRun đang chạy!");
            #endif
            return null;
        }

        process_Run = DoProcess_Run();
        return StartCoroutine(process_Run);
    }

    IEnumerator DoProcess_Run()
    {
        if(GamePlayManagerInstance.currentGameControl.OnStartMap!= null){
            GamePlayManagerInstance.currentGameControl.OnStartMap();
        }

        IndexWave = 0;
        CurrentWave = null;
        CurrentState = WaveState.CreateWave;

        while(IndexWave < ListWaveCatchedInfo.Count){
            if(CurrentWave == null){
                CurrentWave = new WaveController();
                CurrentWave.Init(ListWaveCatchedInfo[IndexWave], IndexWave);
                if(GamePlayManagerInstance.currentGameControl.OnStartNewWave != null){
                    GamePlayManagerInstance.currentGameControl.OnStartNewWave(LevelMap, IndexWave);
                }
            }
            CurrentWave.Run();

            yield return new WaitUntil(()=>(!CurrentWave.IsRunning() || GamePlayManagerInstance.currentGameControl.mainChar.CurrentState == MainCharState.Die));
            if(GamePlayManagerInstance.currentGameControl.mainChar.CurrentHp <= 0){
                break;
            }
            if(GamePlayManagerInstance.currentGameControl.OnCurrentWaveFinished != null){
                GamePlayManagerInstance.currentGameControl.OnCurrentWaveFinished(LevelMap, IndexWave);
            }
            CurrentWave.Stop();
            CurrentWave = null;
            IndexWave ++;
            if(IndexWave == ListWaveCatchedInfo.Count - 1){
                Debug.Log("<color=red>Final wave: " + IndexWave + " : " +  ListWaveCatchedInfo.Count+ "</color>");
                // yield return Yielders.Get(1f);
                // gamePlayManager.UIManager.panelWarning.Show("Final Wave", false);
                // yield return Yielders.Get(1f);
                // gamePlayManager.UIManager.panelWarning.Hide(false);
                // yield return Yielders.Get(0.5f);
            }
        }
        CurrentState = WaveState.Finished;
        if(GamePlayManagerInstance.currentGameControl.OnRunMapFinished!= null){
            GamePlayManagerInstance.currentGameControl.OnRunMapFinished();
        }
        yield return new WaitForSeconds(2f);
        if(GamePlayManagerInstance.currentGameControl.OnSetDataWhenFinish!= null){
            GamePlayManagerInstance.currentGameControl.OnSetDataWhenFinish();
        }
        yield return new WaitForSeconds(1f);

        process_Run = null;

        if(GamePlayManagerInstance.currentGameControl.OnWaveManagerFinished != null){
            GamePlayManagerInstance.currentGameControl.OnWaveManagerFinished();
        }
    }
    
}
