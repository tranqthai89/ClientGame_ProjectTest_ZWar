using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable] public class GamePlay_UIManager
{
    public GamePlayManager GamePlayManagerInstance{
        get{
            return GamePlayManager.Instance;
        }
    }
    [SerializeField] CanvasGroup canvasGroup_PanelStartGame;
    [SerializeField] CanvasGroup canvasGroup_PanelFinishGame;
    [SerializeField] CanvasGroup canvasGroup_PanelFailure;
    [SerializeField] Text txtLevelMap;
    [SerializeField] Text txtIndexWaveInfo;
    [SerializeField] Text txtEnemiesInfo;
    [SerializeField] Text txtMainCharInfo_Damage;
    [SerializeField] Text txtMainCharInfo_AtkSpeed;

    IEnumerator process_ShowPanelStartGame;
    
    public void RefreshAllUI()
    {
        RefreshUI_LevelMapInfo();
        RefreshUI_IndexWaveInfo();
        RefreshUI_EnemyInfo();
        RefreshUI_MainCharInfo();
    }
    public void RefreshUI_LevelMapInfo()
    {
        txtLevelMap.text = "Level: " + GamePlayManagerInstance.waveManager.LevelMap;
    }
    public void RefreshUI_IndexWaveInfo()
    {
        txtIndexWaveInfo.text = "Wave: " + (GamePlayManagerInstance.waveManager.IndexWave + 1) + "/" + GamePlayManagerInstance.waveManager.ListWaveCatchedInfo.Count;
    }
    public void RefreshUI_EnemyInfo()
    {
        txtEnemiesInfo.text = "Enemy: " + GamePlayManagerInstance.currentGameControl.collectionInfo.totalEnemyDie + "/" + GamePlayManagerInstance.currentGameControl.collectionInfo.totalEnemy;
    }
    public void RefreshUI_MainCharInfo()
    {
        txtMainCharInfo_Damage.text = "Damage: " + GamePlayManagerInstance.currentGameControl.mainChar.CurrentDamage;
        txtMainCharInfo_AtkSpeed.text = "Atk Speed: " + GamePlayManagerInstance.currentGameControl.mainChar.CurrentAtkSpeed;
    }
    public void HideAllPanels()
    {
        if(process_ShowPanelStartGame != null)
        {
            GamePlayManagerInstance.StopCoroutine(process_ShowPanelStartGame);
        }
        HidePanelStartGame();
        HidePanelFinishGame();
        HidePanelFailure();
    }
    public void ShowPanelStartGame()
    {
        HideAllPanels();

        canvasGroup_PanelStartGame.alpha = 1f;
        canvasGroup_PanelStartGame.blocksRaycasts = true;

        process_ShowPanelStartGame = DoProcess_ShowPanelStartGame();
        GamePlayManagerInstance.StartCoroutine(process_ShowPanelStartGame);
    }
    IEnumerator DoProcess_ShowPanelStartGame()
    {
        yield return new WaitForSeconds(2f);
        HidePanelStartGame();
    }
    public void HidePanelStartGame()
    {
        canvasGroup_PanelStartGame.alpha = 0f;
        canvasGroup_PanelStartGame.blocksRaycasts = false;
    }
    public void ShowPanelFinishGame()
    {
        HideAllPanels();
        canvasGroup_PanelFinishGame.alpha = 1f;
        canvasGroup_PanelFinishGame.blocksRaycasts = true;
    }
    public void HidePanelFinishGame()
    {
        canvasGroup_PanelFinishGame.alpha = 0f;
        canvasGroup_PanelFinishGame.blocksRaycasts = false;
    }
    public void ShowPanelFailure()
    {
        HideAllPanels();
        canvasGroup_PanelFailure.alpha = 1f;
        canvasGroup_PanelFailure.blocksRaycasts = true;
    }
    public void HidePanelFailure()
    {
        canvasGroup_PanelFailure.alpha = 0f;
        canvasGroup_PanelFailure.blocksRaycasts = false;
    }
}
