using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.Events
{
    public class AvatarEvent_In
    {
        public class EventName
        {
            public const string enterStartGame    = "enterStartGame";
            public const string statisticalResult = "statisticalResult";
            public const string endOfStatistics   = "endOfStatistics";
            public const string weaponChanged     = "weaponChanged";
            //public const string operationMove     = "operationMove";
            //public const string operationIdel     = "operationIdel";
            //public const string operationJump     = "operationJump";
            //public const string operationShoot    = "operationShoot";
        }
    }

    public class AvatarEvent_Out
    {
        public class EventName
        {
            public const string onReqPlayersInfo    = "onReqPlayersInfo";
            public const string onPlayerQuitMatch   = "onPlayerQuitMatch";
            public const string onPlayerEnterWorld  = "onPlayerEnterWorld";
            public const string onReturnHalls       = "onReturnHalls";
            public const string onReadyForBattle    = "onReadyForBattle";
            public const string onEnding            = "onEnding";
            public const string onStatisticalResult = "onStatisticalResult";

            //public const string otherAvataronMove = "otherAvataronMove";
            //public const string otherAvataronIdel = "otherAvataronIdel";
            //public const string otherAvataronJump = "otherAvataronJump";
            //public const string otherAvataronShoot = "otherAvataronShoot";
        }
    }
}
