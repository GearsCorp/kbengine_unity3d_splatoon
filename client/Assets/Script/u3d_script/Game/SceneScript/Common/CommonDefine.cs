using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.Configs
{
    public enum E_InputId
    {
        E_IDEL                  = 0,
        E_MOVE_X                = 1,
        E_MOVE_Y                = 2,
        E_SHOOT_DOWN            = 3,
        E_SHOOT_UP              = 4,
        E_JUMP                  = 5,
        E_CHANGE_MODEL          = 6,
        E_REPLACE_WEAPON        = 7,
        E_THROW_GRENADE         = 8,
        E_SHOOT_FORWARD         = 9, //这个表示射击时的forward
        E_CHANGE_AVATAR_FORWARD = 10, //改变avatar的forward
        E_DRAGGING_CAMERA       = 11, //拖拽摄像头

        E_MOUSE_DRAGGING        = 12, //鼠标拖拽
        E_MOUSE_DRAGEND         = 13, //鼠标拖拽
        E_MOUSE_DRAG_LEFT       = 14, //鼠标拖拽
        E_MOUSE_DRAG_RIGHT      = 15, //鼠标拖拽
      
    };

    public class CommonConfigs
    {
        public const int DISCONNECT_COUNT = 2;
        // 红队ID
        public const Byte RED_TEAM_ID = 1;
        // 蓝队ID
        public const Byte BLUE_TEAM_ID = 2;

        // 一个房间最大人数
        public const Byte ROOM_MAX_PLAYER = 8;
        // 一个房间最少人数
        public const Byte ROOM_MIN_PLAYER = 2;



    }

    public class ExcelPathConfigs
    {
        public const string WEAPONP_ROPERTY_PATH = "StreamingAssets/Config/Weapon.xls";
        public const string BULLET_ROPERTY_PATH  = "StreamingAssets/Config/Bullet.xls";
    }

    public class SceneComNames : MonoBehaviour
    {
        public const string Login          = "Login";  //开始游戏场景
        public const string StartScene     = "StartScene";  //开始游戏场景
        public const string CreateRole     = "CreateRole"; //创建角色场景
        public const string GameRoom       = "GameRoom";   //游戏大厅场景
        public const string StartGame      = "StartGame";  //开始游戏场景
        public const string SelectMode     = "SelectMode"; //选择模式场景
        public const string PlayerMatching = "PlayerMatching";     //玩家匹配场景
        public const string Battle         = "Battle";         //对战场景
        public const string Ending         = "Ending";         //结算场景
    }

    public class PlayerCommonName
    {
        public const string AvatarPlayerName  = "AvatarPlayer";
        public const string PlayerName        = "Player";
        public const string ChildName         = "playerChild";
        public const string ComponentPoint    = "ComponentPoint";
        public const string PersonModelName   = "personModel";
        public const string FishModelName     = "fishModel";
        public const string FishDiveModelName = "fishDiveModel";
        public const string WeaponName        = "wuqi";
        public const string QiangKouName      = "qiangkou_";
        public const string QiangKouName1     = "qiangkou";
        public const string BackPackName      = "backpack";
        public const string BulletsUIName     = "BulletsShow";
    }

    public class ArtEventIds
    {

        public enum ArtEvent
        {
            E_AE_IDEL = 1,
            E_AE_MOVE = 2,
            E_AE_MOVEJUMP = 3,
            E_AE_JUMP = 4,
            E_AE_DIVE = 5,

            E_AE_FISH_IDEL = 6,
            E_AE_FISH_MOVE = 7,
            E_AE_FISH_MOVEJUMP = 8,
            E_AE_FISH_JUMP = 9,
            E_AE_FISH_DIVE = 10,

            E_AE_FISHDIVE_IDEL = 11,
            E_AE_FISHDIVE_MOVE = 12,
            E_AE_FISHDIVE_MOVEJUMP = 13,
            E_AE_FISHDIVE_JUMP = 14,
            E_AE_FISHDIVE_LEAP = 15,

            //E_AE_FISHDIVE_DOWN  = 16,
            //E_AE_FISHDIVE_UP    = 17,

            E_AE_SHOOT = 101,
            E_AE_MOVESHOOT1 = 102,
            E_AE_MOVESHOOT2 = 103,
            E_AE_MOVESHOOT3 = 104,
            E_AE_MOVESHOOT4 = 105,
        }


        public const string ArtIdel = "idel";
        public const string ArtMove = "move";
        public const string ArtMoveJump = "moveJump";
        public const string ArtJump = "jump";
        public const string ArtDive = "dive";

        public const string ArtFishIdel = "fishIdel";
        public const string ArtFishMove = "fishMove";
        public const string ArtFishMoveJump = "fishMoveJump";
        public const string ArtFishJump = "fishJump";
        public const string ArtFishDive = "fishDive";

        public const string ArtFishDiveIdel = "fishDiveIdel";
        public const string ArtFishDiveMove = "fishDiveMove";
        public const string ArtFishDiveMoveJump = "fishDiveMoveJump";
        public const string ArtFishDiveJump = "fishDiveJump";

        public const string ArtFishDiveUp = "fishDiveLeap";


        public const string ArtShoot = "shoot";
        public const string ArtMoveShoot = "moveshoot1";



        public static Dictionary<int, string> ArtNameDict;

        public static object ArtNameDirt { get; private set; }

        public static Dictionary<int, string> GetArtNameDict()
        {
            if (ArtNameDict == null)
            {
                ArtNameDict = new Dictionary<int, string>();
                ArtNameDict.Add((int)ArtEvent.E_AE_IDEL, ArtIdel);
                ArtNameDict.Add((int)ArtEvent.E_AE_MOVE, ArtMove);
                ArtNameDict.Add((int)ArtEvent.E_AE_MOVEJUMP, ArtMoveJump);
                ArtNameDict.Add((int)ArtEvent.E_AE_JUMP, ArtJump);
                ArtNameDict.Add((int)ArtEvent.E_AE_DIVE, ArtDive);

                ArtNameDict.Add((int)ArtEvent.E_AE_FISH_IDEL, ArtFishIdel);
                ArtNameDict.Add((int)ArtEvent.E_AE_FISH_MOVE, ArtFishMove);
                ArtNameDict.Add((int)ArtEvent.E_AE_FISH_MOVEJUMP, ArtFishMoveJump);
                ArtNameDict.Add((int)ArtEvent.E_AE_FISH_JUMP, ArtFishJump);
                ArtNameDict.Add((int)ArtEvent.E_AE_FISH_DIVE, ArtFishDive);

                ArtNameDict.Add((int)ArtEvent.E_AE_FISHDIVE_IDEL, ArtFishDiveIdel);
                ArtNameDict.Add((int)ArtEvent.E_AE_FISHDIVE_MOVE, ArtFishDiveMove);
                ArtNameDict.Add((int)ArtEvent.E_AE_FISHDIVE_MOVEJUMP, ArtFishDiveMoveJump);
                ArtNameDict.Add((int)ArtEvent.E_AE_FISHDIVE_JUMP, ArtFishDiveJump);
                ArtNameDict.Add((int)ArtEvent.E_AE_FISHDIVE_LEAP, ArtFishDiveUp);

                ArtNameDict.Add((int)ArtEvent.E_AE_SHOOT, ArtShoot);
                ArtNameDict.Add((int)ArtEvent.E_AE_MOVESHOOT1, ArtMoveShoot);
            }

            return ArtNameDict;
        }

    }
    public class AnimatorConstName
    {
        public const string Dive            = "Dive";
        public const string ModelState      = "ModelState";
        public const string Jump            = "Jump";
        public const string Shoot           = "Shoot";
        public const string Grenade         = "Grenade";
        public const string AttackDirection = "AttackDirection";
        public const string Move            = "Move";
        public const string Death           = "Death";

        public const string PEASON_AVATAR_PATH     = "models/character/fbx/pensezhanshi01_TPOSE";
        public const string PEASON_CONTROLLER_PATH = "animators/AvatarController";

        public const string FISH_AVATAR_PATH       = "models/character/fbx/zhangyu";
        public const string FISH_CONTROLLER_PATH   = "animators/FishAvatarController";
       
    }

    public class EffectConstName
    {
        public const string FISH_DIVE_EFFECT_PATH        = "effect/prefab/fishDiveEffect";
        public const string FISH_DIVE_EFFECT_NOLOOP_PATH = "effect/prefab/fishDiveEffect_noLoop";
        public const string WEAPON_EFFECT_PATH           = "effect/prefab/wuqi_kaihuo";
    }

    public class DefineEventId
    {
        //before Battle
        public const int PlayerMatchInfoChangedEvent = 1;     //玩家匹配信息改变

        //Battle
        public const int PlayerStateChangedEvent = 101;       //玩家形态改变事件

        public const int PlayerShootEvent =  102;             //玩家开枪事件
        public const int PlayerInAirEvent =  103;             //玩家处于空中事件
        public const int SprayPaintEvent  =  104;             //喷漆事件(子弹发射到碰到物体后，进行喷漆)
        public const int PaintColorEvent  =  105;             //喷漆颜色设置事件


        public const int ChangeWeaponEvent       = 106;        //换枪事件
        public const int ChangeBrushBaseArgEvent = 107;        //射击颜料基本参数事件

        public const int PlayerInSelfInkEvent    = 200;        //玩家处于友方油漆中


        //暂时保留的Id,仅供测试
        public const int JoystickOperationEvent = 1000;        //虚拟遥感操作事件
    }

    public class OperationID
    {//关于游戏中的一些相关操作值

        public const int None = 0;
        public const int JoystickMoveEvent = 1;     //移动操作
        public const int JoystickIdleEvent = 2;     //暂停操作
    }


    public class ButtonEventId
    {
        public const int ButtonClickEvent  = 1;     //按钮点击事件
        public const int ButtonDownEvent   = 2;     //按钮点下事件
        public const int ButtonPressEvent  = 3;     //按钮按着事件
        public const int ButtonUpEvent     = 4;     //按钮释放事件

        public const int ShootButtonEventId   = 11;     //开枪按钮事件
        public const int JumpButtonEventId    = 12;     //跳跃按钮事件
        public const int GrenadeButtonEventId = 13;     //手雷按钮事件

        //public const int ShootClickEvent   = 11;     //开枪按钮点击事件
        //public const int ShootDownEvent    = 12;     //开枪按钮点下事件
        //public const int ShootPressEvent   = 13;     //开枪按钮按着事件
        //public const int ShootUpEvent      = 14;     //开枪按钮释放事件

        //public const int JumpClickEvent    = 21;     //跳跃按钮点击事件
        //public const int JumpDownEvent     = 22;     //跳跃按钮点下事件
        //public const int JumpPressEvent    = 23;     //跳跃按钮按着事件
        //public const int JumpUpEvent       = 24;     //跳跃按钮释放事件

        //public const int GrenadeClickEvent = 31;     //手雷按钮点击事件
        //public const int GrenadeDownEvent  = 32;     //手雷按钮点下事件
        //public const int GrenadePressEvent = 33;     //手雷按钮按下事件
        //public const int GrenadeUpEvent    = 34;     //手雷按钮释放事件
    }

    public class BaseArgConfigIds
    {
        //KindID
        public static Color RedTeamColorInk = new Color(1, 0, 0);     //红队喷漆颜色
        public static Color BlueTeamColorInk = new Color(0, 0, 1);     //蓝队喷漆颜色 
    }

    public class Layers
    {
        public const string Player     = "Player";     //玩家
        public const string Weaponry   = "Weaponry";   //武器装备
        public const string Render     = "RenderLayer";//贴花渲染
    }
    public class Tags
    {
        public const string SelfPlayer   = "SelfPlayer"; //玩家
        public const string Player       = "Player";
        public const string Weaponry     = "Weaponry";   //武器装备
        public const string PaintSurface = "PaintSurface";   //能被喷漆的面
    }

    public class WeaponComConfig
    {
        //统一的弹药数
        public const int UNIFORM_AMMUNITION = 100;
        public const int NOR_RESTORE_SPEED  = 10;
        public const int INK_RESTORE_SPEED  = 30;
    }

    public class PrefabsName
    {
        public const string BulletPrefabName = "Bullets";
    }

}
