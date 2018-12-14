using KBEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameLogic.Events;
using System.IO;
using Excel;
using System.Data;
using GameLogic.Configs;
using CBFrame.Sys;
using GameLogic.ComProperty;

public class GameManager : MonoBehaviour
{
    public enum E_GameState
    {
        GS_WAIT_FOR_BEGINE,
        GS_RUNING,
        GS_ENDING
    };
    public static GameManager Instance;
    public E_GameState gameState;

    public int reLoginCount = 0;    //重登次数

    #region ===== 当前玩家自身账号数据及相关属性 =====
    //Avatar id 
    public Int32 AvatarId = -1;
    public Byte TeamId = 0;

    //用户账号下的角色列表信息
    public UInt64 SelectRoteId = 0;
    public Dictionary<UInt64, AVATAR_INFOS> AvatarBaseInfos = new Dictionary<UInt64, AVATAR_INFOS>();

    //队伍匹配信息
    public Dictionary<Int32, MATCHING_INFOS> EnemyInfosDict = new Dictionary<Int32, MATCHING_INFOS>();
    public Dictionary<Int32, MATCHING_INFOS> TeamInfosDict = new Dictionary<Int32, MATCHING_INFOS>();
    #endregion

    /// <summary>
    /// 武器库
    /// </summary>
    public int CurWeapon;          //当前角色选择的游戏武器
    public int GrenadesId;         //手雷的武器ID

    public List<int> WeaponQueue = new List<int>(); //武器加载顺序
    public Dictionary<int, WeaponProperty> WeaponProPertyDic = new Dictionary<int, WeaponProperty>(); //保存武器属性数组
    public Dictionary<int, BulletProperty> BulletPropertyDic = new Dictionary<int, BulletProperty>(); //保存子弹属性

    public List<KBEngine.Entity> BeforeEnterBattleObjs = new List<KBEngine.Entity>();  //保存未跳转界面前进入的entity


    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);

        SceneManager.LoadScene(SceneComNames.Login);
       
        Instance = this;
        installEvents();
        gameState = E_GameState.GS_WAIT_FOR_BEGINE;
        ReadBulletPropertyByExcel();
        ReadWeaponPropertyByExecl();
    }

    public void installEvents()
    {
        // common
        KBEngine.Event.registerOut("onKicked", this, "onKicked");
        KBEngine.Event.registerOut("onDisconnected", this, "onDisconnected");
        KBEngine.Event.registerOut("onConnectionState", this, "onConnectionState");
        KBEngine.Event.registerOut("onReloginBaseappFailed", this, "onReloginBaseappFailed");
        KBEngine.Event.registerOut("onReloginBaseappSuccessfully", this, "onReloginBaseappSuccessfully");
        KBEngine.Event.registerOut("onEnterWorld", this, "onEnterWorld");

        KBEngine.Event.registerOut(AccountEvent_Out.EventName.onReqAvatarList, this, "onReqAvatarList");
        KBEngine.Event.registerOut(AccountEvent_Out.EventName.onReqCreateAvatar, this, "onReqCreateAvatar");
        KBEngine.Event.registerOut(AvatarEvent_Out.EventName.onReqPlayersInfo, this, "onReqPlayersInfo");
        KBEngine.Event.registerOut(AvatarEvent_Out.EventName.onPlayerQuitMatch, this, "onPlayerQuitMatch");
        KBEngine.Event.registerOut(AvatarEvent_Out.EventName.onReturnHalls, this, "onReturnHalls");
        KBEngine.Event.registerOut(AvatarEvent_Out.EventName.onReadyForBattle, this, "onReadyForBattle");
        KBEngine.Event.registerOut(AvatarEvent_Out.EventName.onEnding, this, "onEnding");
    }

    private void OnDestroy()
    {
        KBEngine.Event.deregisterOut(this);

        ReleaseData();
    }

    public void OnSwitchScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    
    #region  0) =========当前玩家武器数据配置及相关属性操作赋值=========
    /// <summary>
    /// 子弹库数据读取和相关函数操作操作,读取子弹ExcelData
    /// </summary>
    public void ReadBulletPropertyByExcel()
    {
        BulletPropertyDic = ReadExcel.ReadBulletProperty();
    }

    /// <summary>
    /// 通过定义好的子弹ID,来获取子弹的详细配置信息
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public BulletProperty GetBulletProPerty(int Id)
    {
        if (BulletPropertyDic.ContainsKey(Id))
        {
            return BulletPropertyDic[Id];
        }
        return new BulletProperty();
    }

    /// <summary>
    /// 武器库数据读取和相关函数操作操作,读取武器库ExcelData
    /// </summary>
    public void ReadWeaponPropertyByExecl()
    {
        WeaponProPertyDic = ReadExcel.ReadWeaponProperty();
        foreach (WeaponProperty data in WeaponProPertyDic.Values)
        {
            if (data.WeaponName == "手雷") GrenadesId = data.Id;  //保存手雷的ID
            WeaponQueue.Add(data.Id);
        }
    }

    /// <summary>
    /// 通过定义好的武器ID,来获取武器的详细配置信息
    /// </summary>
    /// <param name="Id"></param>
    /// <returns></returns>
    public WeaponProperty GetWeaponProPerty(int Id)
    {
        if (WeaponProPertyDic.ContainsKey(Id)) {
            return WeaponProPertyDic[Id];
        }
        return new WeaponProperty();
    }

    /// <summary>
    /// 改变当前武器装备
    /// </summary>
    public void ChangeCurrentWeapon()
    {
        if (WeaponQueue.Count <= 1 || !WeaponQueue.Contains(CurWeapon)) return;
        int index = WeaponQueue.IndexOf(CurWeapon);

        Debug.Log("ChangeCurrentWeapon_index::" + index + ", " + WeaponQueue.Count + ", " + CurWeapon);
        //!index的武装队列
        if (index == (WeaponQueue.Count - 2)) //不将手雷包含在内
        {
            CurWeapon = WeaponQueue[0];
        }
        else
        {
            CurWeapon = WeaponQueue[++index];
        }
    }

    #endregion

    #region  1) =========角色列表属性操作赋值=========
    public Dictionary<UInt64, AVATAR_INFOS> GetAvatarBaseInfos()
    {
        return AvatarBaseInfos;
    }
    public AVATAR_INFOS GetAvatarBaseInfo(UInt64 id)
    {
        return AvatarBaseInfos[id];
    }
    public void SetAvatarBaseInfo(AVATAR_INFOS info)
    {
        AvatarBaseInfos.Add(info.dbid, info);
        SelectRoteId = info.dbid;
        CurWeapon = info.weaponId;
    }
    public void SetAvatarBaseInfo(UInt64 dbid, AVATAR_INFOS info)
    {
        AvatarBaseInfos.Add(dbid, info);
        SelectRoteId = dbid;
    }

    public void UpdateAvatarBaseInfos(Dictionary<UInt64, AVATAR_INFOS> infoDict)
    {
        AvatarBaseInfos = infoDict;
    }
    public void UpdateAvatarBaseInfos(AVATAR_INFOS_LIST infos)
    {
        AvatarBaseInfos.Clear();
        SelectRoteId = 0;
        Debug.Log("Account::onReqAvatarList:infos.len():" + infos.values.Count);
        for (int i = 0; i < infos.values.Count; i++)
        {
            AVATAR_INFOS info = infos.values[i];
            Dbg.DEBUG_MSG("Account::onReqAvatarList: name" + i + "=" + info.name);
            SetAvatarBaseInfo(info);
        }
    }

    #endregion

    #region  2) =========队伍匹配信息=========
    public void UpdateMatchingInfos(MATCHING_INFOS_LIST infos)
    {
        for (int i = 0; i < infos.values.Count; i++)
        {
            MATCHING_INFOS info = infos.values[i];
            Dbg.DEBUG_MSG("Account::onReqAvatarList: name" + i + "=" + info.name + ",infos.id" + infos.values[i] + " ,teamdId::" + info.teamId + ",TeamId::" + TeamId + ",AvatarId:" + AvatarId);
            if (infos.values[i].teamId == TeamId)
                TeamInfosDict[info.id] = info;
            else
                EnemyInfosDict[info.id] = info;
        }
        if (!ifStartBattle())
        {
            //之后在事件通知系统中，可主动通知更新数据
            CBGlobalEventDispatcher.Instance.TriggerEvent((int)DefineEventId.PlayerMatchInfoChangedEvent);
        }
    
    }

    public void UpdateMatchingInfos(Int32 id)
    {
        if (TeamInfosDict.ContainsKey(id))
        {
            TeamInfosDict.Remove(id);
        }
        else if (EnemyInfosDict.ContainsKey(id))
        {
            EnemyInfosDict.Remove(id);
        }
        if (!ifStartBattle())
        {
            //之后在事件通知系统中，可主动通知更新数据
            CBGlobalEventDispatcher.Instance.TriggerEvent((int)DefineEventId.PlayerMatchInfoChangedEvent);
        }
    }

    #endregion

    #region 3) =========从服务器上获取数据=========

    /// <summary>
    /// 获取当前账号下在服务器上的角色列表
    /// </summary>
    /// <param name="infos"></param>
    public void onReqAvatarList(AVATAR_INFOS_LIST infos)
    {
        UpdateAvatarBaseInfos(infos);
    }

    /// <summary>
    /// 获取当前账号下玩家基础信息
    /// </summary>
    /// <param name="arg1"></param>
    public void onReqCreateAvatar(AVATAR_INFOS arg1)
    {
        SetAvatarBaseInfo(arg1);
    }

    /// <summary>
    /// 获取玩家们匹配信息
    /// </summary>
    /// <param name="avatarId"></param>
    public void onReqPlayersInfo(Int32 avatarId, MATCHING_INFOS_LIST infos)
    {
        //!先获取avatar的ID
        if (AvatarId < 0)
        {
            AvatarId = avatarId;
        }

        //!再获取avatar所在队伍的ID
        if (0 == TeamId)
        {
            for (int i = 0; i < infos.values.Count; i++)
            {
                if (AvatarId == infos.values[i].id)
                {
                    TeamId = infos.values[i].teamId;
                    break;
                }
            }
        }

        UpdateMatchingInfos(infos);
    }

    /// <summary>
    /// 获取指定玩家匹配信息
    /// </summary>
    /// <param name="avatarId"></param>
    public void onPlayerQuitMatch(Int32 avatarId)
    {
        UpdateMatchingInfos(avatarId);
    }

    /// <summary>
    /// 切换到战斗场景
    /// </summary>
    public void onReadyForBattle()
    {
        gameState = E_GameState.GS_RUNING;
        SceneManager.LoadScene(SceneComNames.Battle, LoadSceneMode.Single);
        Debug.Log("Battle_Scene start.");
    }

    /// <summary>
    /// 切换到结束场景
    /// </summary>
    public void onEnding()
    {
        gameState = E_GameState.GS_ENDING;
        for (int i = 0; i < BeforeEnterBattleObjs.Count; i++)
        {
            FrameSyncManager.RemovePlayer(BeforeEnterBattleObjs[i].id);
        }
        SceneManager.LoadScene(SceneComNames.Ending, LoadSceneMode.Single);
        Debug.Log("Ending_Scene start.");
    }

    /// <summary>
    /// 返回大厅
    /// </summary>
    public void onReturnHalls()
    {
        Debug.Log("onReturnHalls_GameRoom .");
        gameState = E_GameState.GS_WAIT_FOR_BEGINE;
        resetMatchingData();
        BeforeEnterBattleObjs.Clear();
        Debug.Log("resetAllServerDataresetAllServerDataresetAllServerData");
        SceneManager.LoadScene(SceneComNames.GameRoom, LoadSceneMode.Single);
        Debug.Log("GameRoom_Scene start.");
    }

    /// <summary>
    /// 判断是否在battle场景
    /// </summary>
    /// <returns></returns>
    public bool ifStartBattle()
    {
        return (gameState == E_GameState.GS_RUNING) ? true : false;
    }

    #endregion

    #region 4) =========客户端重登断线等一系列操作=========
    //当前客户端被踢掉操作
    public void onKicked(UInt16 failedcode)
    {
        SceneManager.LoadScene(SceneComNames.Login);
        resetAllServerData();
        KBEngine.KBEngineApp.app.reset();
        Debug.LogError("onKicked_onKicked_onKicked::" + failedcode.ToString());
    }

    public void onDisconnected()
    {
        Debug.Log("服务器掉线处理:: onDisconnected " + ",reLoginCount::" + reLoginCount);

        //如果当前处于Login场景的话,直接返回
        if (SceneManager.GetActiveScene().name == SceneComNames.Login)
            return;
        InvokeRepeating("onReloginTimer", 0.1f, 1f);
    }

    public void onConnectionState(bool success)
    {
        if (!success)
            Debug.Log("connect(" + KBEngineApp.app.getInitArgs().ip + ":" + KBEngineApp.app.getInitArgs().port + ") is error! (连接错误)");
        else
            Debug.Log("connect successfully, please wait...(连接成功，请等候...)");
    }

    public void onReloginBaseappFailed(UInt16 failedcode)
    {
        Debug.Log("KBEngineApp.app_onReloginBaseappFailed:" + failedcode + ",reLoginCount::" + reLoginCount);
        if (reLoginCount >= CommonConfigs.DISCONNECT_COUNT)
        {
            //关闭掉延时
            this.CancelInvoke("onReloginTiimer");
            reLoginCount = 0;
            Debug.Log("CancelInvoke_reLogin_count::" + reLoginCount);

            onKicked(failedcode);
        }
    }

    public void onReloginBaseappSuccessfully()
    {
        Debug.Log("onReloginBaseappSuccessfully onReloginBaseappSuccessfully!");
        reLoginCount = 0;
        //关闭掉延时
        this.CancelInvoke("onReloginTiimer");
    }

    //处理重复登录循环
    public void onReloginTimer()
    {
        // 判断重连次数
        if (reLoginCount > CommonConfigs.DISCONNECT_COUNT)
        {
            reLoginCount = 0;
            //关闭掉延时
            this.CancelInvoke();
        }

        Debug.Log("onReloginTimer onReloginTimer!" + "reLoginCount");
        KBEngineApp.app.reloginBaseapp();
        reLoginCount++;


    }

    //重置数据
    public void resetAllServerData()
    {
        gameState = E_GameState.GS_WAIT_FOR_BEGINE;
        AvatarId = -1;

        //用户账号下的角色列表信息
        SelectRoteId = 0;
        AvatarBaseInfos.Clear();

        //重置匹配信息
        resetMatchingData();
    }

   
    //重置匹配信息
    public void resetMatchingData()
    {
        TeamId = 0;
        //队伍匹配信息
        EnemyInfosDict.Clear();
        TeamInfosDict.Clear();
    }

    #endregion

    #region 5)=========玩家服务器初始化等一系列操作=========

    public void onEnterWorld(KBEngine.Entity entity)
    {

        Debug.Log("onEnterWorld_____onEnterWorld!!!! + entity::" + entity.className);
        if (SceneManager.GetActiveScene().name != SceneComNames.Battle)
        {
            BeforeEnterBattleObjs.Add(entity);
        }
        //之后在事件通知系统中，可主动通知更新数据
        KBEngine.Event.fireOut(AvatarEvent_Out.EventName.onPlayerEnterWorld, new object[] { entity });
        //else
        //{
        //    KBEngine.Event.deregisterIn("onEnterWorld", this, "onEnterWorld");
        //}

        //if (entity.className == "Avatar" && gameState != E_GameState.GS_RUNING)
        //{
        //    KBEngine.Avatar avatar = (KBEngine.Avatar)entity;
        //    Debug.Log("entity:::"+ avatar.teamID + ",name::::" + avatar.name);
        //}



        //if (entity.isPlayer())
        //{

        //}

    }

    #endregion

    #region 6) =========关于喷漆颜料等其他一系列的操作=========

    /// <summary>
    /// 获取队伍颜色Id
    /// </summary>
    /// <param name="TeamId"></param>
    /// <returns></returns>
    public Color GetTeamColor(int TeamId)
    {
        if (TeamId == CommonConfigs.BLUE_TEAM_ID)
        {
            return BaseArgConfigIds.BlueTeamColorInk;
        }
        else
        {
            return BaseArgConfigIds.RedTeamColorInk;
        }
    }

    /// <summary>
    /// 通过EntityId获取队伍Id
    /// </summary>
    /// <param name="EntityID"></param>
    /// <returns></returns>
    public Color GetTeamColorByEntityId(int EntityID)
    {
        int teamId = 1;
        if (EnemyInfosDict.ContainsKey(EntityID))
            teamId = EnemyInfosDict[EntityID].teamId;
        else if (TeamInfosDict.ContainsKey(EntityID))
            teamId = TeamInfosDict[EntityID].teamId;

        return GetTeamColor(teamId);
    }


    public int GetWeaponIdByEntityId(int EntityID)
    {
        int weaponId = 1;
        if (EnemyInfosDict.ContainsKey(EntityID))
            weaponId = EnemyInfosDict[EntityID].weaponId;
        else if (TeamInfosDict.ContainsKey(EntityID))
            weaponId = TeamInfosDict[EntityID].weaponId;

        return weaponId;
    }

    #endregion

    #region 7) =========释放数据=========

    public void ReleaseData()
    {
        AvatarBaseInfos.Clear();
        AvatarBaseInfos = null;
        EnemyInfosDict.Clear();
        EnemyInfosDict = null;
        TeamInfosDict.Clear();
        TeamInfosDict = null;
        WeaponProPertyDic.Clear();
        WeaponProPertyDic = null;
        WeaponQueue.Clear();
        WeaponQueue = null;
        BulletPropertyDic.Clear();
        BulletPropertyDic = null;
        BeforeEnterBattleObjs.Clear();
        BeforeEnterBattleObjs = null;
    }


    #endregion

}
