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

    [SerializeField] List<Transform> listSpawnPoints;

    [Header("Containers")]
    public Transform waveContainer;
    
    public MapInfo CurrentMapInfo{get;set;}
    public WaveState CurrentState{get;set;}
    public List<WaveCatchedInfo> ListWaveCatchedInfo{get;set;}
    public WaveController CurrentWave{get;set;}

    public long LevelMap{get;set;} // Mức độ của bản đồ hiện tại
    public int IndexWave{get;set;}

    IEnumerator actionRun;
    public bool IsRunning{
        get{
            if(actionRun != null){
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
        actionRun = null;

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
            WaveCatchedInfo _tmpWave = GetWaveCatched(_indexWave, CurrentMapInfo.listEnemyRandom);
            if(_indexWave == CurrentMapInfo.totalWaves - 1){ // final Wave
                // final Wave
                _startHugeWave = GamePlayManagerInstance.CurrentGameDataControl.collectionInfo.totalEnemy;
            }
            if(_tmpWave != null && _tmpWave.listMiniWaveCatched.Count > 0){
                ListWaveCatchedInfo.Add(_tmpWave);
            }else{
                Debug.LogError("[Error] " + _functionName + ": can not catch wave at index: " + _indexWave);
            }
        }

        GamePlayManagerInstance.CurrentGameDataControl.collectionInfo.ratioStartFinalWave = (float) _startHugeWave / (float) GamePlayManagerInstance.CurrentGameDataControl.collectionInfo.totalEnemy;

        Debug.Log("totalEnemy: " + GamePlayManagerInstance.CurrentGameDataControl.collectionInfo.totalEnemy + " | startHugeWave: " + _startHugeWave);
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
                
                GamePlayManagerInstance.CurrentGameDataControl.collectionInfo.totalEnemy ++;
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
        if(actionRun != null){
            #if TEST
            Debug.LogError("actionRun đang chạy!");
            #endif
            return null;
        }
        IndexWave = 0;
        
        return StartCoroutine(actionRun);
    }

    IEnumerator DoProcess_Test()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            SpawnEnemy();
        }
    }
    void SpawnEnemy()
    {
        if (listSpawnPoints.Count == 0)
        {
            Debug.LogWarning("No spawn points available.");
            return;
        }

        Transform _spawnPoint = listSpawnPoints[UnityEngine.Random.Range(0, listSpawnPoints.Count)];
        EnemyInfo _enemyInfo = GameInformation.Instance.listEnemyInfo[UnityEngine.Random.Range(0, GameInformation.Instance.listEnemyInfo.Count)];
        EnemyController _enemy = GamePlayManager.Instance.CreateEnemy(_enemyInfo.prefab, _spawnPoint.position, _spawnPoint.rotation);
        _enemy.Init(_enemyInfo);
    }
}
