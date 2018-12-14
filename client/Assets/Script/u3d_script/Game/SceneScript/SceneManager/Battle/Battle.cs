using CBFrame.Core;
using CBFrame.Sys;
using GameLogic.Configs;
using GameLogic.Events;
using System;
using System.Collections;
using KBEngine;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class Battle : CBComponent {

    public UnityEngine.GameObject AvatarPlayer = null;
    private Avatar _avatarObj; 
    private void Start()
    {
        installEvents();
        AcquireUnLoadObj();
        _avatarObj = gameObject.GetComponent<Avatar>();
    }

    private void installEvents()
    {

        AddEventListener<FP, FP>(OperationID.JoystickMoveEvent, AvataronMove);
        AddEventListener<FP, FP>(OperationID.JoystickIdleEvent, AvataronIdle);
        KBEngine.Event.registerOut(AvatarEvent_Out.EventName.onPlayerEnterWorld, this, "onPlayerEnterWorld");
    }


    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// 获取在跳转battle场景之前，未加载的对象
    /// </summary>
    public void AcquireUnLoadObj()
    {
        for (int i = 0; i < GameManager.Instance.BeforeEnterBattleObjs.Count; i++)
        {
            InstanceUnLoadObj(GameManager.Instance.BeforeEnterBattleObjs[i]);
        }
        Debug.Log("AcquireUnLoadObj.count::" + GameManager.Instance.BeforeEnterBattleObjs.Count);
    }

    private void InstanceUnLoadObj(KBEngine.Entity entity)
    {
        if(entity.className == "Avatar")
        {
            KBEngine.Avatar avatar = (KBEngine.Avatar)entity;

            GameObject obj = (GameObject)Resources.Load("Prefabs/AvatarPlayer");
            obj = Instantiate(obj, avatar.position, Quaternion.identity) as UnityEngine.GameObject;
            SpaceData.Instance.SpacePlayers.Add(((KBEngine.Avatar)entity).componentFrameSync);
            SpaceData.Instance.SpacePlayers = SpaceData.Instance.SpacePlayers.OrderBy(s => s.seatNo).ToList();

           
            if (avatar.teamID == CommonConfigs.RED_TEAM_ID)
            {
                //变换头发
            }
            obj.transform.forward = avatar.direction;
        
            if (entity.isPlayer())
            {
                AvatarPlayer = obj;
                AvatarPlayer.name = PlayerCommonName.AvatarPlayerName;

                AddCameraFllow();
                AddController();
               // Debug.Log("entity_entity_entity:::" + entity.id + ",CurWeapon:::" + GameManager.Instance.CurWeapon);
            }
            FrameSyncManager.InitPlayerBehaviour(obj, avatar);
            entity.renderObj = obj;
            AddBattleRelateData(entity, obj);
            Debug.Log("name::"+ avatar.name + " ,entity.id::" + entity.id + " ,position::" + avatar.position + ",direction::" + avatar.direction);
            return;
        }
    }

    private void AddCameraFllow()
    {
        if (AvatarPlayer != null)
        {
            Transform Player = AvatarPlayer.transform.Find(PlayerCommonName.PlayerName);
            AvatarPlayer.transform.Find(PlayerCommonName.ComponentPoint).GetComponent<ComMoveController>().SetCameraObj();
        }
    }

    private void AddController()
    {
        if (AvatarPlayer != null)
        {
            AvatarPlayer.AddComponent<LocalController>().SetTarget(AvatarPlayer.transform);
        }
    }

    /// <summary>
    /// 进入战斗相关数据赋值：例如武器渲染颜色赋值等相关数据
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="obj"></param>
    private void AddBattleRelateData(KBEngine.Entity entity, GameObject obj)
    {
        Avatar avaterObj = obj.transform.Find(PlayerCommonName.PlayerName).GetComponent<Avatar>();
        avaterObj.EnterBattleRelateData(entity.id);//将entityId保存到各自的render对象中，方便读取
    }


    #region 1）==========关于外部控制数据传递============

    public void onPlayerEnterWorld(KBEngine.Entity entity)
    {
        InstanceUnLoadObj(entity);
    }

    #endregion


    #region 2）==========关于帧数据传递============

    public void AvataronMove(FP speedRate, FP angle)
    {
        _avatarObj.Move(/*speedRate, */angle);
        CBGlobalEventDispatcher.Instance.TriggerEvent<FP>(OperationID.JoystickMoveEvent, angle);
    }

    public void AvataronIdle(FP speedRate, FP angle)
    {
        _avatarObj.Idle();
    }

    #endregion


}