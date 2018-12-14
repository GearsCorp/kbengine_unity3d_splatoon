using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Excel;
using System.Data;
using GameLogic.Configs;
using GameLogic.ComProperty;


public static class ReadExcel
{
    //读取Excel
    public static DataRowCollection GameReadExcel(string namePath, string sheetName = "")
    {
        string strExcelpath = Application.dataPath + "/" + namePath;
        // Debug.Log("excel__path::" + strExcelpath);
       // string filename = "D:/work/unity/unity_work/code/InkPaintDemo/Assets/StreamingAssets/Config/w.xls";
        FileStream stream = File.Open(strExcelpath, FileMode.Open, FileAccess.Read);
        IExcelDataReader excelReader;
       // Excel中的2007版本要改變
        if (strExcelpath.EndsWith(".xlsx"))
        {
            excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
        }
        else
        {
            excelReader = ExcelReaderFactory.CreateBinaryReader(stream);
        }

        DataSet result = excelReader.AsDataSet();
        if (result == null)
        {
            Debug.Log("Read excelFile_"+ namePath + " is fail!");
            return null;
        }

        if (sheetName == "")
        {
            return result.Tables[0].Rows;
        }
        else
        {
            return result.Tables[sheetName].Rows;
        }

    }

    //读取武器配置列表
    public static Dictionary<int, WeaponProperty> ReadWeaponProperty()
    {
        DataRowCollection dataLst = ReadExcel.GameReadExcel(ExcelPathConfigs.WEAPONP_ROPERTY_PATH);
        Dictionary<int, WeaponProperty> weaponPropertyDict = new Dictionary<int, WeaponProperty>();
        if (dataLst == null)
        {
            Debug.Log("ReadWeaponProperty is failed! DataRowCollection obj is null!");
            return weaponPropertyDict;
        }
        for (int i = 1; i < dataLst.Count; i++)
        {
            WeaponProperty data = new WeaponProperty();
            DataRow excelst = dataLst[i];
            if (excelst[0].ToString() == "" && excelst[1].ToString() == "") continue;

            data.Id = int.Parse(excelst[0].ToString()); //武器id
            data.WeaponName = excelst[1].ToString(); //武器名称

            data.ShootRange = int.Parse(excelst[2].ToString()); //射程
            data.MaxShootRange = int.Parse(excelst[3].ToString()); //最大射程
            data.ATK = float.Parse(excelst[4].ToString());        //攻击力
            data.FireRate = float.Parse(excelst[5].ToString());   //射速
            data.ShootSpeed = float.Parse(excelst[6].ToString()); //子弹射击速度
            data.MinAccumulateForce = float.Parse(excelst[7].ToString()); //蓄力最小数
            data.MaxAccumulateForce = float.Parse(excelst[8].ToString()); //蓄力最大数
            data.MaxAccumulateForceTime = float.Parse(excelst[9].ToString()); //蓄力最大限制时间
            data.MinConsumeEnergy = int.Parse(excelst[10].ToString()); //最小能量消耗数值
            data.MaxConsumeEnergy = int.Parse(excelst[11].ToString()); //最大能量消耗数值
            data.HP = int.Parse(excelst[12].ToString());         //武器的子弹数

            data.PrefabsPath = excelst[13].ToString(); //预置体路
            data.BulletId = int.Parse(excelst[14].ToString());  //子弹Id
            weaponPropertyDict.Add(data.Id, data);

        }
        return weaponPropertyDict;
    }

    /// <summary>
    /// 读取子弹配置列表
    /// </summary>
    /// <returns></returns>
    public static Dictionary<int, BulletProperty> ReadBulletProperty()
    {
        DataRowCollection dataLst = ReadExcel.GameReadExcel(ExcelPathConfigs.BULLET_ROPERTY_PATH);
        Dictionary<int, BulletProperty> bulletPropertyDict = new Dictionary<int, BulletProperty>();
        if (dataLst == null)
        {
            Debug.Log("ReadBulletProperty is failed! DataRowCollection obj is null!");
            return bulletPropertyDict;
        }
        for (int i = 1; i < dataLst.Count; i++)
        {
            BulletProperty data = new BulletProperty();
            DataRow excelst = dataLst[i];
            if (excelst[0].ToString() == "" && excelst[1].ToString() == "") continue;

            data.Id = int.Parse(excelst[0].ToString()); //子弹id
            data.BulletName = excelst[1].ToString(); //子弹名称

            data.Damage      = int.Parse(excelst[2].ToString()); //子弹伤害
            data.MaxDamage   = int.Parse(excelst[3].ToString()); //子弹伤害
            data.PrefabsPath = excelst[4].ToString();            //子弹路径

            bulletPropertyDict.Add(data.Id, data);
        }
        return bulletPropertyDict;
    }

}