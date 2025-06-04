using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    IEnumerator process_Run;
    WaveManager.WaveCatchedInfo waveCatchedInfo;
    int indexWave;

    public void Init(WaveManager.WaveCatchedInfo _waveCatchedInfo, int _indexWave){
        waveCatchedInfo = _waveCatchedInfo;
        indexWave = _indexWave;
    }
    public bool IsRunning(){
        if(process_Run != null){
            return true;
        }
        return false;
    }
    public Coroutine Run(){
        if(process_Run != null){
            return null;
        }
        process_Run = DoProcess_Run();
        return WaveManagerInstance.StartCoroutine(process_Run);
    }
    private IEnumerator DoProcess_Run(){
        if(waveCatchedInfo.listMiniWaveCatched.Count == 0){
            process_Run = null;
            yield break;
        }
        Debug.Log("Start Wave " + indexWave + " | pow: " + waveCatchedInfo.powCatched);

        Vector3 _posEnemy = Vector3.zero;
        EnemyController _tmpMonsterPrefab = null;
        EnemyController _tmpEnemyController = null;
        List<WaveManager.EnemyInWaveCatched> _listEnemyInWaveCollected = null;
        WaveManager.EnemyInWaveCatched _currentEnemyInWave = null;

        for(int i = 0; i < waveCatchedInfo.listMiniWaveCatched.Count; i ++){
            // - Create enemy - //
            Debug.Log("Start MiniWave " + indexWave + " - " + i + " | pow: " + waveCatchedInfo.listMiniWaveCatched[i].powCatched);

            if(WaveManagerInstance.CurrentState == WaveState.Finished){
                break;
            }
            if(waveCatchedInfo.listMiniWaveCatched[i].listEnemyInWaveCatched.Count == 0){
                Debug.Log("[0] No enemy in miniwave " + i + " | indexWave: " + indexWave);
                break;
            }
            if(_listEnemyInWaveCollected == null){
                _listEnemyInWaveCollected = new List<WaveManager.EnemyInWaveCatched>();
            }else{
                _listEnemyInWaveCollected.Clear();
            }
            for(int j = 0; j < waveCatchedInfo.listMiniWaveCatched[i].listEnemyInWaveCatched.Count; j ++){
                _listEnemyInWaveCollected.Add(waveCatchedInfo.listMiniWaveCatched[i].listEnemyInWaveCatched[j]);

                waveCatchedInfo.listMiniWaveCatched[i].listEnemyInWaveCatched.RemoveAt(j);
                j--;
                continue;
            }
            // - 1 lần summon sẽ spawn enemy trong wave này, random 10 cửa
            // - delay 1 khoảng thời gian rồi spawn tiếp
            // - Giới hạn tối đa trên màn hình chỉ 50 con quái
            if(_listEnemyInWaveCollected.Count == 0){
                Debug.Log("[1] No enemy in miniwave " + i + " | indexWave: " + indexWave);
                break;
            }
            List<Transform> _listSpawnPoints = GamePlayManagerInstance.currentGameControl.currentMap.listEnemySpawnPoints.ShallowCopy();
            if(_listSpawnPoints.Count == 0){
                Debug.LogError("No spawn points available.");
                break;
            }
            int _maxSpawnPointCount = Mathf.Min(10, _listSpawnPoints.Count);
            int _tmpCount = 0;
            for (int j = 0; j < _listEnemyInWaveCollected.Count; j++){
                // - Giới hạn tối đa trên màn hình chỉ 50 con quái
                if(GamePlayManagerInstance.enemyPoolManager.listObjects.Count >= 50){
                    yield return null;
                    continue;
                }
                _currentEnemyInWave = _listEnemyInWaveCollected[j];
                _tmpMonsterPrefab = _currentEnemyInWave.enemyInfo.prefab;

                int _indexSpawnPoint = UnityEngine.Random.Range(0, _listSpawnPoints.Count);
                Transform _spawnPoint = _listSpawnPoints[_indexSpawnPoint];
                _tmpEnemyController = GamePlayManager.Instance.CreateEnemy(_tmpMonsterPrefab, _spawnPoint.position, _spawnPoint.rotation);
                _tmpEnemyController.Init(_listEnemyInWaveCollected[j].enemyInfo);
                _tmpEnemyController.Run();

                _listSpawnPoints.RemoveAt(_indexSpawnPoint); // Xóa spawn point đã sử dụng
                
                if(GamePlayManagerInstance.currentGameControl.OnEnemySpawned != null){
                    GamePlayManagerInstance.currentGameControl.OnEnemySpawned(_tmpEnemyController);
                }
                
                _tmpCount ++;

                if(_listSpawnPoints.Count == 0){
                    _listSpawnPoints = GamePlayManagerInstance.currentGameControl.currentMap.listEnemySpawnPoints.ShallowCopy();
                    yield return new WaitForSeconds(1f); // Chờ một khoảng thời gian trước khi spawn tiếp
                }else if(_tmpCount >= _maxSpawnPointCount){ // Giới hạn tối đa 10 con quái mỗi lần spawn
                    _listSpawnPoints = GamePlayManagerInstance.currentGameControl.currentMap.listEnemySpawnPoints.ShallowCopy();
                    _tmpCount = 0;
                    yield return new WaitForSeconds(1f); // Chờ một khoảng thời gian trước khi spawn tiếp
                }else if(GamePlayManagerInstance.enemyPoolManager.listObjects.Count >= 50){
                    _listSpawnPoints = GamePlayManagerInstance.currentGameControl.currentMap.listEnemySpawnPoints.ShallowCopy();
                }
            }
            yield return new WaitUntil(()=>GamePlayManagerInstance.enemyPoolManager.listObjects.Count == 0);
        }
        
        process_Run = null;
        yield break;
    }

    public void Stop(){
        if(process_Run != null){
            WaveManagerInstance.StopCoroutine(process_Run);
            process_Run = null;
        }
    }
}
