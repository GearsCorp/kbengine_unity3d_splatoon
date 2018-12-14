using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRender : MonoBehaviour
{

    public Camera shotCam;
    public RenderTexture texture;
    private Texture2D tex = null;
    void OnPostRender()
    {
        //在每次相机渲染完成时再删除上一帧的texture
        if (tex != null)
        {
            Destroy(tex);
        }
        //设定当前RenderTexture为快照相机的targetTexture
        RenderTexture rt = shotCam.targetTexture;
        RenderTexture.active = rt;
        tex = new Texture2D(rt.width, rt.height);
        //读取缓冲区像素信息
        tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
        tex.Apply();
        //    texture.mainTexture = tex;

    }
}
