namespace KBEngine
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using GameLogic.Events;

    public class Account : AccountBase
    {
        public Account() : base()
        {
        }
        public override void __init__()
        {
            base.__init__();
            installEvents();

            // 触发登陆成功事件
            Event.fireOut(AccountEvent_Out.EventName.onLoginSuccessfully);
           
            //登录成功后请求角色列表
             reqAvatarList();
        }

        public void installEvents()
        {
            Event.registerIn(AccountEvent_In.EventName.reqAvatarList, this, "reqAvatarList");
            Event.registerIn(AccountEvent_In.EventName.reqCreateAvatar, this, "reqCreateAvatar");
            Event.registerIn(AccountEvent_In.EventName.enterGameRoom, this, "enterGameRoom");
        }

        public override void onDestroy()
        {
            base.onDestroy();
            KBEngine.Event.deregisterIn(this);
        }


        /// <summary>
        /// 向服务端请求获得角色列表
        /// </summary>
        public void reqAvatarList()
        {
            baseEntityCall.reqAvatarList();
        }
        public override void onReqAvatarList(AVATAR_INFOS_LIST infos)
        {
            Dbg.DEBUG_MSG("Account::onReqAvatarList: avatarsize=" + infos.values.Count);
            Event.fireOut(AccountEvent_Out.EventName.onReqAvatarList, new object[] { infos });
        } 

        public void reqCreateAvatar(Byte arg1, string arg2)
        {
            baseEntityCall.reqCreateAvatar(arg1, arg2);
        }
        public override void onCreateAvatarResult(AVATAR_INFOS arg1)
        {
            Event.fireOut(AccountEvent_Out.EventName.onReqCreateAvatar, new object[] {arg1 });
        }

        public void enterGameRoom()
        {
            baseEntityCall.enterGameRoom(GameManager.Instance.SelectRoteId);
        }

    }
}