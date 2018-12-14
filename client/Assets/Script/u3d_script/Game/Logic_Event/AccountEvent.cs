using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.Events
{
    public class AccountEvent_In
    {
        public class EventName
        {
            public const string login = "login";
            public const string enterGame = "EnterGame";
            public const string helloInfo = "helloInfo";
            public const string reqAvatarList   = "reqAvatarList";
            public const string reqCreateAvatar = "reqCreateAvatar";
            public const string enterGameRoom   = "enterGameRoom";
            
        }
    }

    public class AccountEvent_Out
    {
        public class EventName
        {
            public const string onLoginSuccessfully = "OnLoginSuccessfully";
            public const string onLoginFailed = "onLoginFailed";
            public const string onLoginBaseappFailed = "onLoginBaseappFailed";
            public const string onReqAvatarList = "onReqAvatarList";
            public const string onReqCreateAvatar = "onReqCreateAvatar";
            public const string onEnterGameSuccess = "onEnterGameSuccess";
            public const string onEnterGameFailed = "onEnterGameFailed";
            public const string onHelloInfo = "onHelloInfo";
        }

    }

}


