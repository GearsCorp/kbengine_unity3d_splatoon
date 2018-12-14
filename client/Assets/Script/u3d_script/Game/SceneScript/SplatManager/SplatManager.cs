using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public struct Splat {
	public Matrix4x4 splatMatrix;
	public Vector4 channelMask;
	public Vector4 scaleBias;  //偏移的比例
}

public struct SplatReciever {

}

public class SplatManagerSystem
{
    public int RenderID = 10;
  

	static SplatManagerSystem m_Instance;
	static public SplatManagerSystem instance {
		get {
			if (m_Instance == null)
				m_Instance = new SplatManagerSystem();
			return m_Instance;
		}
	}

    public Vector4 ChannelMask = new Vector4(1, 0, 0, 0);

    public Camera rtCamera = null;

    public int GameObjectID = 0;

    public int splatsX;
	public int splatsY;

	public Vector4 scores;
    public Dictionary<int, Vector4> scoresDict = new Dictionary<int, Vector4>();
	
	internal List<Splat> m_Splats = new List<Splat>();
    internal Dictionary<int, List<Splat>> m_SplatDict = new Dictionary<int, List<Splat>>();

    public void AddSplat (Splat splat)
	{
		//Debug.Log ("Adding Splat");
		m_Splats.Add (splat);
	}
    public void AddSplat(int id, Splat splat)
    {
     //   Debug.Log ("Adding Splat::"+ id + "  ,Splat::"+ splat.splatMatrix + ",scaleBias::" + splat.scaleBias );
        if (m_SplatDict.ContainsKey(id))
        {
            m_SplatDict[id].Add(splat);
        }
        else
        {
            m_SplatDict.Add(id, new List<Splat>());
            m_SplatDict[id].Add(splat);
        }
    }

    public List<float> SplatSample_X = new List<float> { 0.5f, 0.5f, 0.25f, 0,  0,   0.75f,   0,   0.5f, 0.25f, 0.25f, 0.25f, 0f, 0.75f, 0.75f, 0.75f, 0.75f};
    public List<float> SplatSample_Y = new List<float> { 0.75f, 0f,  0.25f, 0, 0.5f, 0.75f, 0.25f, 0.5f,   0f,  0.5f,  0.75f, 0.75f, 0,  0.5f,  0.25f,  0};

}

public class SplatManager : MonoBehaviour
{
	public int sizeX = 1024;
	public int sizeY = 1024;

	public Texture2D splatTexture;
	public int splatsX = 4;
	public int splatsY = 4;

	public RenderTexture splatTex;
	public RenderTexture splatTexAlt;

    public RenderTexture worldPosTex;
	public RenderTexture worldPosTexTemp;
	public RenderTexture worldTangentTex;
	public RenderTexture worldBinormalTex;
	//private Camera rtCamera;

	private Material splatBlitMaterial;

	private bool evenFrame = false;

	public Vector4 scores = Vector4.zero;

	public RenderTexture RT256;
	public RenderTexture RT4;
	public Texture2D Tex4;


    public Vector3 PositionVec3 = Vector3.zero;

    public Material MaterialRender;

    private Rect _rect;
    Texture2D texture2D;
    public int ObjectId = 0;
    // Use this for initialization


    private void Awake()
    {

        _rect = new Rect(0, 0, sizeX, sizeY);

        SplatManagerSystem.instance.splatsX = splatsX;
        SplatManagerSystem.instance.splatsY = splatsY;

        splatBlitMaterial = new Material(Shader.Find("Splatoonity/SplatBlit"));
        if (splatBlitMaterial == null) Debug.Log("Shader ====> Splatoonity/SplatBlit is null!!");
        splatTex = new RenderTexture(sizeX, sizeY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        splatTex.Create();
        splatTexAlt = new RenderTexture(sizeX, sizeY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        splatTexAlt.Create();
        worldPosTex = new RenderTexture(sizeX, sizeY, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        worldPosTex.Create();
        worldPosTexTemp = new RenderTexture(sizeX, sizeY, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        worldPosTexTemp.Create();
        worldTangentTex = new RenderTexture(sizeX, sizeY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        worldTangentTex.Create();
        worldBinormalTex = new RenderTexture(sizeX, sizeY, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        worldBinormalTex.Create();

        splatBlitMaterial.SetTexture("_SplatTex", splatTex);
        splatBlitMaterial.SetTexture("_WorldPosTex", worldPosTex);
        splatBlitMaterial.SetVector("_SplatTexSize", new Vector4(sizeX, sizeY, 0, 0)); //为所有着色器设置一个全局向量。

        MaterialRender = gameObject.GetComponent<Renderer>().material;
        if (MaterialRender)
        {
            MaterialRender.SetTexture("_SplatTex", splatTex);
        }

        // Textures for tallying scores
        RT256 = new RenderTexture(256, 256, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
        RT256.autoGenerateMips = true;  //自动生成Mip
        RT256.useMipMap = true;  //会生成MipMap
        RT256.Create();
        RT4 = new RenderTexture(4, 4, 0, RenderTextureFormat.ARGB32, RenderTextureReadWrite.Linear);
        RT4.Create();
        Tex4 = new Texture2D(4, 4, TextureFormat.ARGB32, false);

        if (SplatManagerSystem.instance.rtCamera == null)
        {
            GameObject rtCameraObject = new GameObject();

            rtCameraObject.name = "rtCameraObject";
            rtCameraObject.transform.position = PositionVec3;
            rtCameraObject.transform.rotation = Quaternion.identity;
            rtCameraObject.transform.localScale = Vector3.one; //Vectort3==>Vectoo3.one(1,1,1)
            Camera rtCamera = rtCameraObject.AddComponent<Camera>();
            rtCamera.clearFlags = CameraClearFlags.SolidColor;  // 屏幕上的任何空的部分将显示当前相机的背景颜色。
            rtCamera.backgroundColor = new Color(0, 0, 0, 0);
            rtCamera.orthographic = true;
            rtCamera.nearClipPlane = 0.0f;
            rtCamera.farClipPlane = 1.0f;
            rtCamera.orthographicSize = 1.0f;
            rtCamera.aspect = 1.0f;
            rtCamera.useOcclusionCulling = false;
            rtCamera.enabled = false;
            SplatManagerSystem.instance.rtCamera = rtCamera;
        }

        RenderTextures();
        BleedTextures();


        ObjectId = SplatManagerSystem.instance.GameObjectID++;
        //   Debug.Log("ObjectId:::"+ ObjectId   + ",gameObject:::" +gameObject.name);
    }

    void Start()
    { 
       
      //  Debug.Log("splatBlitMaterial:::" + splatBlitMaterial.name + ", id：：：" + ObjectId);
    }
   
    // Render textures with a command buffer.
    // This is more flexible as you can explicitly add more objects to render without worying about layers.
    // You could also have multiple instances for chunks of a scene.
    void RenderTextures() {

        // Set the culling mask to Nothing so we can draw renderers explicitly
        SplatManagerSystem.instance.rtCamera.cullingMask = LayerMask.NameToLayer("Nothing");
		Material worldPosMaterial = new Material (Shader.Find ("Splatoonity/WorldPosUnwrap"));
		Material worldTangentMaterial = new Material (Shader.Find ("Splatoonity/WorldTangentUnwrap"));
		Material worldBiNormalMaterial = new Material (Shader.Find ("Splatoonity/WorldBinormalUnwrap"));

		// You could collect all objects you want rendererd and loop through DrawRenderer
		// but for this example I'm just drawing the one renderer.
		Renderer envRenderer = this.gameObject.GetComponent<Renderer> ();

		// You could also use a multi render target and only have to draw each renderer once.
		CommandBuffer cb = new CommandBuffer();
        cb.SetRenderTarget(worldPosTex);
        cb.ClearRenderTarget(true, true, new Color(0, 0, 0, 0)); //初始颜色设置为灰色
        cb.DrawRenderer(envRenderer, worldPosMaterial);

        //cb.SetRenderTarget(worldTangentTex);
        //cb.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
        //cb.DrawRenderer(envRenderer, worldTangentMaterial);

        //cb.SetRenderTarget(worldBinormalTex);
        //cb.ClearRenderTarget(true, true, new Color(0, 0, 0, 0));
        //cb.DrawRenderer(envRenderer, worldBiNormalMaterial);

        // Only have to render the camera once!
        SplatManagerSystem.instance.rtCamera.AddCommandBuffer (CameraEvent.AfterEverything, cb);
        SplatManagerSystem.instance.rtCamera.Render();
	}

	void BleedTextures() {
        //设置source的_MainTexd在Material上，而且一个全屏的四边形
        Graphics.Blit (Texture2D.blackTexture, splatTex, splatBlitMaterial, 1);		
		Graphics.Blit (Texture2D.blackTexture, splatTexAlt, splatBlitMaterial, 1);

		splatBlitMaterial.SetVector("_SplatTexSize", new Vector2( sizeX, sizeY ) );

		// Bleed the world position out 2 pixels
       
		Graphics.Blit (worldPosTex, worldPosTexTemp, splatBlitMaterial, 2);
		Graphics.Blit (worldPosTexTemp, worldPosTex, splatBlitMaterial, 2);

		// Don't need this guy any more
		worldPosTexTemp.Release();
		worldPosTexTemp = null;
	}


	// Blit the splats
	// This is similar to how a deferred decal would work
	// except instead of getting the world position from the depth
	// use the world position that is stored in the texture.
	// Each splat is tested against the entire world position texture.
	public void PaintSplats() {
        if (!SplatManagerSystem.instance.m_SplatDict.ContainsKey(ObjectId)) return;
		if (SplatManagerSystem.instance.m_SplatDict[ObjectId].Count > 0) {
			Matrix4x4[] SplatMatrixArray = new Matrix4x4[10];
			Vector4[] SplatScaleBiasArray = new Vector4[10];
			Vector4[] SplatChannelMaskArray = new Vector4[10];

			// Render up to 10 splats per frame.
			int i = 0;
			while(SplatManagerSystem.instance.m_SplatDict[ObjectId].Count > 0 && i < 10 ){
				SplatMatrixArray [i] = SplatManagerSystem.instance.m_SplatDict[ObjectId][0].splatMatrix;
				SplatScaleBiasArray [i] = SplatManagerSystem.instance.m_SplatDict[ObjectId][0].scaleBias;
				SplatChannelMaskArray [i] = SplatManagerSystem.instance.m_SplatDict[ObjectId][0].channelMask;
                SplatManagerSystem.instance.m_SplatDict[ObjectId].RemoveAt(0);
				i++;
			}
           // Debug.Log("id::"+ ObjectId + "name::::" + splatBlitMaterial.name);
			splatBlitMaterial.SetMatrixArray ( "_SplatMatrix", SplatMatrixArray );
			splatBlitMaterial.SetVectorArray ( "_SplatScaleBias", SplatScaleBiasArray );
			splatBlitMaterial.SetVectorArray ( "_SplatChannelMask", SplatChannelMaskArray );

			splatBlitMaterial.SetInt ( "_TotalSplats", i );

			splatBlitMaterial.SetTexture ("_WorldPosTex", worldPosTex);

            if (evenFrame)
            {
                splatBlitMaterial.SetTexture("_LastSplatTex", splatTexAlt);
                Graphics.Blit(splatTexture, splatTex, splatBlitMaterial, 0);
                MaterialRender.SetTexture("_SplatTex", splatTex);
                evenFrame = false;
            }
            else
            {
                splatBlitMaterial.SetTexture("_LastSplatTex", splatTex);
                Graphics.Blit(splatTexture, splatTexAlt, splatBlitMaterial, 0);
                MaterialRender.SetTexture("_SplatTex", splatTexAlt);
                evenFrame = true;
            }

            RenderTextureToStaticData();

        }
    }
		

	// Update is called once per frame
	void Update ()
    {
		PaintSplats ();
    }

    public void SetRenderTexture2D(ref  Texture2D texture2D)
    {
        if (evenFrame)
            SetRenderTexture2D(ref texture2D, splatTexAlt);
        else
            SetRenderTexture2D(ref texture2D, splatTex);
    }

    public void SetRenderTexture2D(ref Texture2D tex, RenderTexture rt)
    {
        RenderTexture currentActiveRT = RenderTexture.active;
        RenderTexture.active = rt;
        tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
        RenderTexture.active = currentActiveRT;
    }


    /// <summary>
    /// 统计结果
    /// </summary>
    /// <param name="scores"></param>
    public void StatisticalResult(ref Vector2 scores)
    {

        if (evenFrame)
            Graphics.Blit(splatTexAlt, RT256, splatBlitMaterial, 3);
        else
            Graphics.Blit(splatTex, RT256, splatBlitMaterial, 3);

        Graphics.Blit(RT256, RT4); //RT256 复制到 RT4 上

        RenderTexture.active = RT4;
        Tex4.ReadPixels(new Rect(0, 0, 4, 4), 0, 0);
        Tex4.Apply();

        Color scoresColor = new Color(0, 0, 0, 0);
        for (int i = 0; i < Tex4.GetPixels().Length; i++)
        {
            scoresColor += Tex4.GetPixels()[i];
        }

        Debug.Log( "objectId::::"+ ObjectId+ "  ,xxx, yyy:::" + scoresColor.r + ",scoresColor.b::" +scoresColor.b);
        scores.x += scoresColor.r;
        scores.y += scoresColor.b;
        //scores.z = scoresColor.b;
        //scores.w = scoresColor.a;
    }

    public void RenderTextureToStaticData()
    {
        //对当前对象进行渲染
        if (PixelManager.GetInstance().JudgetRenderTexture(ObjectId, this))
        {
            Debug.Log("Render------ObjectId::" + ObjectId);
            texture2D = PixelManager.GetInstance().ObjectIDToTexture2D[ObjectId];
            SetRenderTexture2D(ref texture2D);
            texture2D.Apply();
        }
    }
}
