using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMapInfo", menuName="GameInfo/MapInfo")]
public class MapInfo : ScriptableObject
{
    //Note:
    // - Tạm thời cho random enemy trong listEnemyRandom, không random theo weight

    public int level; // level bắt đầu từ 1
    public int totalWaves; // tổng số wave trong 1 level

    [Serializable] public class EnemyInWave{
        public EnemyInfo enemyInfo;
        public int weight;
        [Tooltip("Nếu = -1 thì ko có giới hạn")] public int limitAppearInWave = -1; // giới hạn số lượng quái cùng loại này xuất hiện cùng lúc
    }
    public List<EnemyInWave> listEnemyRandom;

    [Header("Wave")]
    [SerializeField] int wavePowBase;
    [SerializeField] float wavePowFactor;
    [SerializeField] float wavePowLambda;

    [Header("Mini Wave")]
    [SerializeField] int miniWavePowBase;
    [SerializeField] float miniWavePowFactor;
    [SerializeField] float miniWavePowLambda;

    [Header("Map Prefab")]
    public MapController mapPrefab;
    
    public long GetPowerWave(int _wave){ // level bắt đầu từ 1, wave bắt đầu = 0
        string _functionName = "GetPowerWave";
        try{
            if(_wave < 0){
                Debug.LogError("[Error] " + _functionName + ": invalid wave");
                return wavePowBase;
            }
            // WaveNumber = ((level - 1) * totalwaves) + _wave
            // WavePower = minWavePow + wavePowFactor * WaveNumber ^ wavePowLambda
            double _currentPower = (wavePowBase + wavePowFactor * Mathf.Pow(_wave, wavePowLambda));
            if(_currentPower < wavePowBase){
                _currentPower = wavePowBase;
            }
            return (long) _currentPower;
        }catch(Exception _ex){
            Debug.LogError("[Error] " + _functionName + ": " + _ex);
            return wavePowBase;
        }
    }
    public long GetPowerMiniWave(int _wave){ // level bắt đầu từ 1, wave bắt đầu = 0
        string _functionName = "GetPowerMiniWave";
        try{
            if(_wave < 0){
                Debug.LogError("[Error] " + _functionName + ": invalid wave");
                return miniWavePowBase;
            }
            // WaveNumber = ((level - 1) * totalwaves) + _wave
            // MiniWavePower = minMiniWavePow + miniWavePowFactor * WaveNumber ^ miniWavePowLambda
            double _currentPower = (miniWavePowBase + miniWavePowFactor * Mathf.Pow(_wave, miniWavePowLambda));
            if(_currentPower < miniWavePowBase){
                _currentPower = miniWavePowBase;
            }
            return (long) _currentPower;
        }catch(Exception _ex){
            Debug.LogError("[Error] " + _functionName + ": " + _ex);
            return miniWavePowBase;
        }
    }
    [ContextMenu("Test GetPowerWave")]
    public void Test()
    {
        for(int i = 0; i < totalWaves; i ++){
            long _powerWave = GetPowerWave(i);
            long _powerMiniWave = GetPowerMiniWave(i);
            int _totalMiniWaves = (int) Math.Ceiling((double) _powerWave / _powerMiniWave);
            Debug.Log("Wave " + i + " | Power: " + _powerWave + " | MiniWave Power: " + _powerMiniWave + " | Total MiniWaves: " + _totalMiniWaves);
        }
    }
}
