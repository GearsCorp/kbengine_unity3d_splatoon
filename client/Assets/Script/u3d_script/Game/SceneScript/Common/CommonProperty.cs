using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLogic.ComProperty
{
    public class PlayerAccountInfo
    {
        public int PlayerID;  //玩家唯一标识
        public string NameID;    //玩家昵称ID
        public int RoleID;    //玩家角色ID
        public int WeaponID;  //玩家武器ID

        public int RoomID;       //玩家房间号ID
        public int PositionID;   //玩家位置ID
        public int TeamID;       //玩家队伍

        public bool IsNull()
        {
            if (NameID == "") return true;
            else return false;
        }
    }

    public class WeaponProperty
    {
        public int Id = -1; //武器ID
        public string WeaponName;  //武器名称
        public int   ShootRange;   //射程
        public int   MaxShootRange;//最大射程
        public float ATK;         //攻击力
        public float FireRate;    //射速
        public float ShootSpeed;  //子弹射击速度
        public float MinAccumulateForce;     //蓄力最小数
        public float MaxAccumulateForce;     //蓄力最大数
        public float MaxAccumulateForceTime; //蓄力最长时间
        public int   MinConsumeEnergy;       //最小能量消耗数值
        public int   MaxConsumeEnergy;       //最大能量消耗数值
        public int   HP;           //武器的子弹数    
        public string PrefabsPath; //预置体路径
        public int BulletId;       //子弹ID
                                   // Use this for initialization

 
        public bool IsNull()
        {
            if (Id == -1) return true;
            else return false;
        }

        public void Release()
        {
            WeaponName = "";
            PrefabsPath = ""; //预置体路径
        }
    }

    public class BulletProperty
    {
        public int Id = -1;           //子弹ID
        public string BulletName;     //子弹名称
        public int  Damage;           //伤害
        public int  MaxDamage;        //最大伤害
        public string PrefabsPath;    //射速
    
        public bool IsNull()
        {
            if (Id == -1) return true;
            else return false;
        }
    }
}
