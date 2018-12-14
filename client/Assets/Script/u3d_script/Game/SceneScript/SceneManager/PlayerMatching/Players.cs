using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameLogic.Configs;
using CBFrame.Sys;

public class Players : MonoBehaviour
{
    public  GameObject[] PlayerPrefabs;  //角色玩家预制体model
    public  Transform[] PlayerPositions; //角色玩家位置
    public  Text[] PlayerNameTexts;      //角色玩家昵称ID

    private Dictionary<string,GameObject> MatchingPlayerDic = new Dictionary<string, GameObject>();  //匹配的玩家model  <玩家昵称，生成的对象>
    private Dictionary<int, GameObject>   NoMatchingPlayerDic = new Dictionary<int, GameObject>();   //未匹配的玩家model <玩家模型位置，生成的对象>

    private Dictionary<int, string> PlayerRankingInfo = new Dictionary<int, string>();  //玩家房间位置和其昵称ID的映射

    private List<int> NoMatchingPlayersLst = new List<int>();  //未匹配的玩家位置
    private List<int> RankingLst = new List<int>() {0,1,2,3};  //默认的玩家房间位置排序
    private void Awake()
    {
        AcquireRankingInfo();
        CreatePlayerUI();
        CBGlobalEventDispatcher.Instance.AddEventListener((int)DefineEventId.PlayerMatchInfoChangedEvent, UpdateMatchInfo);
    }

    private void OnDestroy()
    {

        foreach(KeyValuePair<string, GameObject> pair in MatchingPlayerDic)
        {
            Destroy(pair.Value);
        }
        MatchingPlayerDic.Clear();

        foreach (KeyValuePair<int, GameObject> pair in NoMatchingPlayerDic)
        {
            Destroy(pair.Value);
        }
        NoMatchingPlayerDic.Clear();

        PlayerRankingInfo.Clear();
        NoMatchingPlayersLst.Clear();
        RankingLst.Clear();


    }


    /// <summary>
    /// 获取排位信息
    /// </summary>
    void AcquireRankingInfo()
    {
        NoMatchingPlayersLst.Clear();
        PlayerRankingInfo.Clear();
        List<int> tmpRangking = new List<int>(RankingLst);
        foreach (int id in GameManager.Instance.TeamInfosDict.Keys)
        {
            int positionId = GameManager.Instance.TeamInfosDict[id].roomPosition;
            if ((positionId % 2) == 0 && positionId != 0)
            {
                positionId -= 1;
            }
            positionId = positionId / 2;
            if (tmpRangking.Contains(positionId))
            {
                tmpRangking.Remove(positionId);
            }
            PlayerRankingInfo.Add(positionId, GameManager.Instance.TeamInfosDict[id].name);
        }
        NoMatchingPlayersLst = tmpRangking;
    }

    void CreatePlayerUI()
    {
        //先加载已经匹配的队员model
        foreach (int positionId in PlayerRankingInfo.Keys) 
        {
            if (positionId < PlayerPositions.Length)
            {
                GameObject obj = GameObject.Instantiate(PlayerPrefabs[0], PlayerPositions[positionId].position, PlayerPositions[positionId].rotation);
                MatchingPlayerDic.Add(PlayerRankingInfo[positionId], obj);
                string name = PlayerRankingInfo[positionId];
                PlayerNameTexts[positionId].text = name;
            }
        }

        //加载未匹配的队员model
        for (int i = 0; i < NoMatchingPlayersLst.Count; i++) 
        {
            GameObject obj = GameObject.Instantiate(PlayerPrefabs[1], PlayerPositions[NoMatchingPlayersLst[i]].position, PlayerPositions[NoMatchingPlayersLst[i]].rotation);
            NoMatchingPlayerDic.Add(NoMatchingPlayersLst[i], obj);
            PlayerNameTexts[NoMatchingPlayersLst[i]].text = "";
        }
    }

    void ClearModelObjData()
    {
        //清除匹配加载的model
        foreach(GameObject obj in MatchingPlayerDic.Values)
        {
            Destroy(obj);
        }
        MatchingPlayerDic.Clear();

        //清除未匹配加载的model
        foreach (GameObject obj in NoMatchingPlayerDic.Values)
        {
            Destroy(obj);
        }
        NoMatchingPlayerDic.Clear();
    }

    void UpdateMatchInfo() 
    {
        //if (GameManager.Instance.TeamId == CommonConfigs.RED_TEAM_ID)
        //{
        //    if (PlayerRankingInfo.Count != GameManager.Instance.TeamInfosDict.Count)
        //}

        if (PlayerRankingInfo.Count != GameManager.Instance.TeamInfosDict.Count)  //匹配玩家人数发生变化,暂时不考虑匹配时退出
        {
            Debug.Log("PlayerRankingInfo.Count::"+ PlayerRankingInfo.Count + ",GameManager.Instance.TeamInfosDict.Count:"+ GameManager.Instance.TeamInfosDict.Count);
            ClearModelObjData();
            AcquireRankingInfo();
            CreatePlayerUI();
        }
    }
}
