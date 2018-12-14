using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderTexturePool
{
    
    private List<Texture2D> _texture2DPools = new List<Texture2D>();//这是纹理管理池，对贴花渲染结果纹理进行管理
    private int defTextureWidth = 1024;
    private int defTextureHeight = 1024;
    private static RenderTexturePool instance;

    public Vector2 scoresTotal = Vector2.zero;
    public Dictionary<int, SplatManager> SplatManagerDict = new Dictionary<int, SplatManager>();
    public static RenderTexturePool GetInstance()
    {
        if (instance == null)
        {
            instance = new RenderTexturePool();
        }
        return instance;
    }

    public Texture2D InstanceTexture2D()
    {
        if (_texture2DPools.Count == 0)
        {
            return new Texture2D(defTextureWidth, defTextureHeight);
        }
        else
        {
            Texture2D obj = _texture2DPools[0];
            _texture2DPools.Remove(obj);
            return obj;
        }

    }

    public void DestoryTexture2D(Texture2D obj)
    {
        _texture2DPools.Add(obj);
    }


    public void OnDestroy()
    {
        for (int i = 0; i < _texture2DPools.Count; i++)
        {
            MonoBehaviour.Destroy(_texture2DPools[i]);
            _texture2DPools[i] = null;
        }
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 将被渲染过的物体上的贴花管理器SplatManager保存起来，方便结算统计
    /// </summary>
    /// <param name="id"></param>
    /// <param name="obj"></param>
    public void AddSplatManagerLst(int id, SplatManager obj)
    {
        if (!SplatManagerDict.ContainsKey(id))
        {
            SplatManagerDict.Add(id, obj);
        }
    }

    /// <summary>
    /// 统计结果
    /// </summary>
    public void StatisticalResult()
    {
        scoresTotal = Vector2.zero;
        Debug.Log(" SplatManagerDict.Values:::" + SplatManagerDict.Values.Count);
        foreach (var item in SplatManagerDict.Values)
        {
            item.StatisticalResult(ref scoresTotal);
        }
        //Debug.Log("scores___red::::" + (scoresTotal.x / (scoresTotal.x + scoresTotal.y)) * 100 + "%");
        //Debug.Log("scores___green::::" + (scoresTotal.y / (scoresTotal.x + scoresTotal.y)) * 100 + "%");
       // Debug.Log("scoresTotal::::" + "   scoresTotal.x:::"  + scoresTotal.x + "  , scoresTotal.y:::" + scoresTotal.y); 
    }

    public void ResetData()
    {
        _texture2DPools.Clear();
        defTextureWidth  = 1024;
        defTextureHeight = 1024;
        scoresTotal      = Vector2.zero;
        SplatManagerDict.Clear();
    }

}


public class PixelManager{

    private static PixelManager instance;

    public enum E_InColorType
    {
        IC_SELF_COLOR = 1,
        IC_OTHER_COLOR = 2,
        IC_BLANK_COLOR = 3,
    }
    public static PixelManager GetInstance()
    {
        if (instance == null)
        {
            instance = new PixelManager();
        }
        return instance;
    }

    //这是每个玩家ID对应所站的物体贴花ID
    public  Dictionary<int, int> PlayerIDToObjectID = new Dictionary<int, int>();
    //玩家ID对应其对于队伍武器喷漆颜色
    public  Dictionary<int, Color> PlayerIDToTeamColor = new Dictionary<int, Color>();
    //这个每个物体贴花ID对应的物体贴花渲染数据
    public Dictionary<int, Texture2D> ObjectIDToTexture2D = new Dictionary<int, Texture2D>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="PlayerId"></param>
    /// <param name="ObjectId"></param>
    /// <returns>如果返回值为false，表示没有找到相对于的Render,需要主动重新渲染一次</returns>
    public bool SetPlayerIdToObjectID(int PlayerId, int ObjectId)
    {
        if (ObjectId == 10)
        {
            Debug.Log("ObjectId::::"+ ObjectId);
        }
       
        if (PlayerIDToObjectID == null)
        {
            Debug.Log("SetPlayerIdToObjectID_PlayerIDToObjectID::::PlayerIDToObjectID is null!");
        }

        if (!PlayerIDToObjectID.ContainsKey(PlayerId)) 
        {
            PlayerIDToObjectID.Add(PlayerId, ObjectId);
        }
        else
        {
            int id = PlayerIDToObjectID[PlayerId];
            PlayerIDToObjectID[PlayerId] = ObjectId;
            if (!PlayerIDToObjectID.ContainsValue(id))//如果当下没有玩家站着或者需要渲染的物体，就将需要渲染的物体标志移除掉
            {
                RenderTexturePool.GetInstance().DestoryTexture2D(ObjectIDToTexture2D[id]);
                ObjectIDToTexture2D.Remove(id);
            }
        }

        if (!ObjectIDToTexture2D.ContainsKey(ObjectId))
        {
            return false;
        }
        else
        {
            return true;
        }
        ////判断是否包含当前玩家数据，1）没有增加需要数据，并看是否需要主动增加Texture2D渲染
        //if (!PlayerIDToObjectID.ContainsKey(PlayerId))
        //{
        //    PlayerIDToObjectID.Add(PlayerId, ObjectId);
        //    if (ObjectIDToTexture2D.ContainsKey(ObjectId)) return true;
        //    return false;
        //}
        //else
        //{
        //    if (PlayerIDToObjectID.ContainsValue(ObjectId))
        //    {
        //        PlayerIDToObjectID[PlayerId] = ObjectId;
        //        return true;
        //    } 
        //    else
        //    {
        //     //   Debug.Log("ddddddddddddddddddddddddddddddddddd");
        //        int id = PlayerIDToObjectID[PlayerId];
        //        PlayerIDToObjectID[PlayerId] = ObjectId;
        //        if (!PlayerIDToObjectID.ContainsValue(id))//如果当下没有玩家站着或者需要渲染的物体，就将需要渲染的物体标志移除掉
        //        {
        //            RenderTexturePool.GetInstance().DestoryTexture2D(ObjectIDToTexture2D[id]);
        //            ObjectIDToTexture2D.Remove(id);
        //        }
        //        return false;
        //    }
        //}
    }


    /// <summary>
    /// 将玩家的武器颜色渲染给划分出来
    /// </summary>
    /// <param name="PlayerSplatColor"></param>
    /// <param name="playerId"></param>
    public void SetPlayerIdToTeamColor(int playerId, Color PlayerTeamColor)
    {
        if (!PlayerIDToTeamColor.ContainsKey(playerId))
        {
            PlayerIDToTeamColor.Add(playerId, PlayerTeamColor);
        }
        else
        {
            PlayerIDToTeamColor[playerId] = PlayerTeamColor;
        }
    }


    /// <summary>
    /// 判断物体是否需要渲染
    /// </summary>
    /// <param name="ObjectId"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool JudgetRenderTexture(int ObjectId, SplatManager obj)
    {
        RenderTexturePool.GetInstance().AddSplatManagerLst(ObjectId, obj);
        if (ObjectIDToTexture2D.ContainsKey(ObjectId)) return true;
        else return false;
    }

    /// <summary>
    /// 设置渲染数据
    /// </summary>
    /// <param name="ObjectId"></param>
    /// <param name="obj"></param>
    public void SetObjectIDToTexture2D(int ObjectId, Texture2D obj)
    {
        ObjectIDToTexture2D.Add(ObjectId, obj);
       // Debug.Log("ObjectIDToTexture2D::"+ ObjectIDToTexture2D.Count);
    }

    /// <summary>
    /// 获取像素信息
    /// </summary>
    /// <param name="PlayerId"></param>
    /// <param name="textureCoord2"></param>
    /// <returns></returns>
    public bool GetPixelInfo(int PlayerId, Vector2 textureCoord2)
    {
        Color data = ObjectIDToTexture2D[PlayerIDToObjectID[PlayerId]].GetPixelBilinear(textureCoord2.x, textureCoord2.y);
        if (data.r > 0.50f && data.r > data.g)
        {
            Debug.Log("PlayerId___" + PlayerId + "textureCoord2.x, textureCoord2.y:::" + textureCoord2.x + " ," + textureCoord2.y + ",    Color:::red_red_red_red!!_____::" + data.r + "  ," + data.g); 
            return true;
        }
        else if(data.g > 0.5f && data.g > data.r)
        {
            Debug.Log("Color:::green_green_green!!");
            return false;
        }
        return false;
      //  Debug.Log("Color:::"+ ObjectIDToTexture2D[PlayerIDToObjectID[PlayerId]].GetPixelBilinear(textureCoord2.x, textureCoord2.y) + " ,textureCoord2::"+ textureCoord2.x + " , " + textureCoord2.y);
        //for (int i = ((int)textureCoord2.x -15); i < (textureCoord2.x + 30); i++)
        //{
        //    for (int j = ((int)textureCoord2.y - 15); j < (textureCoord2.y + 30); j++)
        //    {
        //        Debug.Log("Color:::"+ ObjectIDToTexture2D[PlayerIDToObjectID[PlayerId]].GetPixel(i,j));
        //    }
        //}
       // return true;
        //Color colorData = ObjectIDToTexture2D[PlayerIDToObjectID[PlayerId]].GetPixelBilinear(textureCoord2.x, textureCoord2.y);
        ////    Debug.Log("colorData:::"+ colorData +" , textureCoord2:::" +textureCoord2.x + "," + textureCoord2.y);
        //if (colorData.r == 1 && (colorData.b == 0) && colorData.g == 0) return true;
        //else return false;
    }

    double singleGridNumX = 0.001953125;
    double singleGridNumY = 0.001953125;
    float singleHalfGridNum = 0.0009766f;
    public E_InColorType GetPixelsInfo(int PlayerId, Vector2 textureCoord2)
    {
        if(!PlayerIDToTeamColor.ContainsKey(PlayerId)) return E_InColorType.IC_BLANK_COLOR;
        if(!PlayerIDToObjectID.ContainsKey(PlayerId)) return E_InColorType.IC_BLANK_COLOR;
        Color dataColor = PixelColorDeal(PlayerId, PlayerIDToObjectID[PlayerId], textureCoord2);

      //  Debug.Log("dataColor:::" + dataColor + ",PlayerIDToTeamColor[PlayerId]::" + PlayerIDToTeamColor[PlayerId]);
        if (dataColor == Color.white) return E_InColorType.IC_BLANK_COLOR;

        if(PlayerIDToTeamColor[PlayerId] == dataColor) return E_InColorType.IC_SELF_COLOR;
        else return E_InColorType.IC_OTHER_COLOR;

        //   Debug.Log("GridNumX:::" + GridNumX + " ,GridNumY:::" + GridNumY + " ,,textureCoord2.x::" + textureCoord2.x + "TextureData.GetPixelBilinear(fGridPixelX, fGridPixelY)::" + TextureData.GetPixelBilinear(textureCoord2.x, textureCoord2.y));
        //Debug.Log("GridNumX:::" + GridNumX + " ,GridNumY:::" + GridNumY +
        //        " ,,redData::" + redData + " ,,blueData::" + blueData + " ,,otherData::" + otherData);

    }

    public bool IsInOneSelfInk(int PlayerId, int ObjectId, Vector2 textureCoord2)
    {
        if (!PlayerIDToTeamColor.ContainsKey(PlayerId)) return false;
        Color dataColor = PixelColorDeal(PlayerId, ObjectId, textureCoord2);
     //   Debug.Log("dataColor_IsInOneSelfInk:::" + dataColor +", PlayerIDToTeamColor[PlayerId]::"+ PlayerIDToTeamColor[PlayerId]+ "  ,:::textureCoord2::" + textureCoord2.x + " ,::" + textureCoord2.y);
        if (PlayerIDToTeamColor[PlayerId] == dataColor) return true;
        else return false;
    }

    /// <summary>
    /// 像素颜色处理
    /// </summary>
    /// <param name="PlayerId"></param>
    /// <param name="ObjectID"></param>
    /// <param name="textureCoord2"></param>
    /// <returns></returns>
    public Color PixelColorDeal(int PlayerId, int ObjectID, Vector2 textureCoord2)
    {
        float redData = 0f;
        float blueData = 0f;
        int otherData = 0;

        Texture2D TextureData = ObjectIDToTexture2D[ObjectID];
        int GridNumX = (int)((double)textureCoord2.x / singleGridNumX);
        int GridNumY = (int)((double)textureCoord2.y / singleGridNumY);
        double GridPixelX = GridNumX * singleGridNumX;
        double GridPixelY = GridNumY * singleGridNumY;
        //float GridPixelX = （GridNumX *singleGridNum;
        float fGridPixelX = (float)GridPixelX;
        float fGridPixelY = (float)GridPixelY;
       // Debug.Log("textureCoord2.x__textureCoord2.y::" + textureCoord2.x + "__" + textureCoord2.y + " ,fGridPixelX_fGridPixelY::" + fGridPixelX + "__" + fGridPixelY + " ,,realData::::" + TextureData.GetPixelBilinear(textureCoord2.x, textureCoord2.y)
        //          + " , floatDealData::" + TextureData.GetPixelBilinear(fGridPixelX, fGridPixelY));
        Color data = TextureData.GetPixelBilinear(fGridPixelX, fGridPixelY);
        AnalyzeColor(ref data, ref redData, ref blueData, ref otherData);
        // Debug.Log("TextureData.GetPixelBilinear(fGridPixelX, fGridPixelY)::" + TextureData.GetPixelBilinear(textureCoord2.x, textureCoord2.y));
        data = TextureData.GetPixelBilinear((fGridPixelX + singleHalfGridNum), fGridPixelY);
        AnalyzeColor(ref data, ref redData, ref blueData, ref otherData);
        data = TextureData.GetPixelBilinear((fGridPixelX + singleHalfGridNum * 2), fGridPixelY);
        AnalyzeColor(ref data, ref redData, ref blueData, ref otherData);
        data = TextureData.GetPixelBilinear(fGridPixelX, (fGridPixelY + singleHalfGridNum));
        AnalyzeColor(ref data, ref redData, ref blueData, ref otherData);
        data = TextureData.GetPixelBilinear(fGridPixelX, (fGridPixelY + singleHalfGridNum * 2));
        AnalyzeColor(ref data, ref redData, ref blueData, ref otherData);

        Color dataColor = Color.white;
        if (otherData == 5 && redData < 2f && blueData < 2f)
        {
         //   Debug.Log("111_None_None_None_None!!!" + "==>GridNumX:::" + GridNumX + "  ,GridNumY:::" + GridNumY + "  ,redData::" + redData + "  ,blueData::" + blueData);
        }
        else if (otherData == 5 && redData > 2f && redData > blueData)
        {
          //  Debug.Log("222_None_red_red_red!!!" + "==>GridNumX:::" + GridNumX + "  ,GridNumY:::" + GridNumY + "  ,redData::" + redData + "  ,blueData::" + blueData);
            dataColor = Color.red;
        }
        else if (otherData == 5 && blueData > 2f && blueData > redData)
        {
           //   Debug.Log("333_None_blue_blue_blue!!!" + "==>GridNumX:::" + GridNumX + "  ,GridNumY:::" + GridNumY + "  ,redData::" + redData + "  ,blueData::" + blueData);
            dataColor = Color.blue;
        }
        else if (redData > blueData)
        {
          //   Debug.Log("444_red_red_red!!!" + "==>GridNumX:::" + GridNumX + "  ,GridNumY:::" + GridNumY + "  ,redData::" + redData + "  ,blueData::" + blueData);
            dataColor = Color.red;
        }
        else if (redData < blueData)
        {
          //  Debug.Log("blue_blue_blue!!!" + "==>GridNumX:::" + GridNumX + "  ,GridNumY:::" + GridNumY + "  ,redData::" + redData + "  ,blueData::" + blueData);
            dataColor = Color.blue;
        }

        return dataColor;
    }

    void AnalyzeColor(ref Color data, ref float redData, ref float blueData, ref int otherData)
    {
       //  Debug.Log("data.r::" + data);
        if (/*data.r < 0.4f && data.g < 0.4f*/ data.r != 1  && data.b != 1f)
        {
            otherData += 1;
           
        }
        redData += data.r;
        blueData += data.b;
    }


    public void ResetData()
    {
         PlayerIDToObjectID.Clear();
         PlayerIDToTeamColor.Clear();
         ObjectIDToTexture2D.Clear();
    }

}
