using System.Collections;
using System.Collections.Generic;
using DevToolkit;
using UnityEngine;

public class MapController : MySimplePoolObjectController
{
    public List<Transform> listEnemySpawnPoints;
    public Transform mainCharSpawnPoint;
    public MapInfo MyMapInfo{get;set;}

    void Reset(){
        // Tìm đối tượng có tên "MainCharSpawner" trong các con của GameObject hiện tại
        mainCharSpawnPoint = transform.Find("MainCharSpawner");
        if(mainCharSpawnPoint == null){
            Debug.LogError("Không tìm thấy GameObject mang tên là MainCharSpawner");
        }

        // Tìm đối tượng có tên "EnemySpawners" và lấy tất cả các con của nó
        Transform _enemySpawnersParent = transform.Find("EnemySpawners");
        if (_enemySpawnersParent != null)
        {
            listEnemySpawnPoints.Clear(); // Đảm bảo danh sách rỗng trước khi thêm mới
            foreach (Transform _child in _enemySpawnersParent)
            {
                if(!_child.gameObject.activeSelf){continue;}
                listEnemySpawnPoints.Add(_child);
            }
            if(listEnemySpawnPoints.Count == 0){
                Debug.LogError("Không có điểm EnemySpawner nào");
            }
        }else{
            Debug.LogError("Không tìm thấy GameObject mang tên là EnemySpawners");
        }
    }
    public override void ResetData()
    {
        base.ResetData();
        MyMapInfo = null;
    }
    public void Init(MapInfo _mapInfo){
        MyMapInfo = _mapInfo;
    }
}
