namespace KBEngine
{
    using GameLogic.Events;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public class Avatar : AvatarBase
    {
        public override void __init__()
        {
            base.__init__();
            installEvents();
        }
        public void installEvents()
        {
            Event.registerIn(AvatarEvent_In.EventName.enterStartGame, this, "enterStartGame");
            Event.registerIn(AvatarEvent_In.EventName.statisticalResult, this, "statisticalResult");
            Event.registerIn(AvatarEvent_In.EventName.endOfStatistics, this, "endOfStatistics");
            Event.registerIn(AvatarEvent_In.EventName.weaponChanged, this, "weaponChanged");
        }

        public override void onDestroy()
        {
            base.onDestroy();
            KBEngine.Event.deregisterIn(this);
        }
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        
        public override void onResPlayersInfo(MATCHING_INFOS_LIST matchingInfosLst)
        {
            Dbg.DEBUG_MSG("Avatar::onReqPlayersInfo: playersize=" + matchingInfosLst.values.Count);
            Event.fireOut(AvatarEvent_Out.EventName.onReqPlayersInfo, new object[] { baseEntityCall.id, matchingInfosLst });
        }

        public override void onPlayerQuitMatch(Int32 id) 
        {
            Dbg.DEBUG_MSG("Avatar::onResPlayersInfo: playerId=" + id);
            Event.fireOut(AvatarEvent_Out.EventName.onPlayerQuitMatch, new object[] {id });

        }
        public override void onTeamMateChange()
        {
        }

        public void enterStartGame()
        {
            if (baseEntityCall == null)
            {
                Debug.Log("baseEntityCall_enterStartGame is null!:::");
                return;
            }
            baseEntityCall.enterStartGame();
        }

        //游戏结束后,上传统计结果
        public void statisticalResult(Byte team, float value)
        {
            Debug.Log("Room_statisticalResult_statisticalResult");
            if (baseEntityCall == null) {
                Debug.Log("baseEntityCall_statisticalResult is null!:::");
                return;
            }
            baseEntityCall.statisticalResult(team, value);
        }

        //向服务器传递信息：统计页面显示完成,已结束
        public void endOfStatistics()
        {
            Debug.Log("Room_endOfStatistics_endOfStatistics");
            if (baseEntityCall == null)
            {
                Debug.Log("baseEntityCall_endOfStatistics is null!:::");
                return;
            }
            baseEntityCall.endOfStatistics();
        }

        public override void onReadyForBattle()
        {
            Event.fireOut(AvatarEvent_Out.EventName.onReadyForBattle);
            Debug.Log("onReadyForBattle_onReadyForBattle_onReadyForBattle");
        }

        public override void onReturnHalls()
        {
            Event.fireOut(AvatarEvent_Out.EventName.onReturnHalls);
            Debug.Log("onReturnHalls_onReturnHalls_onReturnHalls");
        }

        public override void onEnding()
        {
            Event.fireOut(AvatarEvent_Out.EventName.onEnding);
            Debug.Log("onEnding_onEnding_onEnding");
        }

        public override void onStatisticalResult(Byte arg1, float arg2)
        {
            Event.fireOut(AvatarEvent_Out.EventName.onStatisticalResult, new object[]{ arg1, arg2 });
            Debug.Log("onStatisticalAccount_onStatisticalAccount_onStatisticalAccount");
        }


        public void weaponChanged(Int32 weaponId)
        {
            if (baseEntityCall == null)
            {
                Debug.Log("baseEntityCall_weaponChanged is null!:::");
                return;
            }
          //  Debug.Log("baseEntityCall_weaponChanged::" + weaponId);
            baseEntityCall.weaponChanged(weaponId);
        }

 
    }

}
