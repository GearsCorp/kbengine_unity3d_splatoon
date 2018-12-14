using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GameLogic.ComProperty;
using GameLogic.Configs;
using CBFrame.Sys;

public class Bullet: MonoBehaviour
{

    private int _damage;       //子弹的伤害值
    private Vector3 _lastPoint;
    private Vector3 _last2Point;
    private Vector3 _beforePoint;
    private GameObject _parentObj;

    private Collider _colliderObj;


    //贴花相关的数据
    private int _splatsX = 1;
    private int _splatsY = 1;
    private float _splatSampleX = 0.5f;
    private float _splatSampleY = 0.75f;
    private float _splatScale   = 1.0f;
    private GameObject _hitObj;
    private int _id = 0;  //子弹射击的对象Id
    private Vector4 _splatColor = new Vector4(0,0,0,0);

    private void Awake()
    {
        _beforePoint = Vector3.zero;
        _last2Point  = Vector3.zero;
        _lastPoint   = gameObject.transform.position;
        _colliderObj = gameObject.GetComponent<Collider>();
    }

    public void Init(int id, GameObject parentObj, int Damage = -1)
    {
        _parentObj = parentObj;
        if (Damage == -1)
        {
            _damage = GameManager.Instance.GetBulletProPerty(id).Damage;
        }
        else _damage = Damage;
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
        _beforePoint = _last2Point;
        _last2Point  = _lastPoint;
        _lastPoint = gameObject.transform.position;
    }

    public void OnTriggerEnter(Collider other)
    {
     //   Debug.Log("other_OnTriggerEnterOnTriggerEnter::" + other.name);
        if(_parentObj == other.gameObject || other.CompareTag(Tags.Weaponry) || !gameObject.activeInHierarchy) return;

        //1)如果是子弹射击到玩家
        if (other.CompareTag(Tags.Player))
        {
            Avatar AvatarObj = other.GetComponent<Avatar>();
            if (AvatarObj && (AvatarObj.ItemType != (int)GameManager.Instance.TeamId))
                AvatarObj.TargetAttack(_damage);
            Destroy(AvatarObj);
            return;
        }
 
        RaycastHit hit;
        Ray ray = new Ray(_beforePoint, (gameObject.transform.position - _beforePoint));
        int layerMask = LayerMask.GetMask(Layers.Render);
        //layerMask = ~layerMask;
        Debug.DrawRay(_beforePoint,/* Vector3.up*/ (gameObject.transform.position - _beforePoint), Color.yellow, 20f);
        if (!Physics.Raycast(ray, out hit,
                   (Vector3.Distance(gameObject.transform.position, _beforePoint)), layerMask)) return;

        if (hit.collider == null) 
        {
            Debug.LogError("bullet_onTriggerEnter未检测到数据!::"+ other.name);
            return;
        }

        //!开始油漆喷射
        //CBGlobalEventDispatcher.Instance.TriggerEvent<RaycastHit>(DefineEventId.SprayPaintEvent, hitInfo);
        //GameObjPool.GetInstance().DestroyObject(gameObject);
        //ResetData();
        ////  Destroy(gameObject);
        //Resources.UnloadUnusedAssets();

        _splatsX = SplatManagerSystem.instance.splatsX;
        _splatsY = SplatManagerSystem.instance.splatsY;

        Vector3 leftVec = Vector3.Cross(hit.normal, Vector3.up);
        float randScale = Random.Range(0.5f, 1.5f);

        GameObject newSplatObject = new GameObject();
        newSplatObject.transform.position = hit.point;
        if (leftVec.magnitude > 0.001f)
        {
            newSplatObject.transform.rotation = Quaternion.LookRotation(leftVec, hit.normal);
        }
        // newSplatObject.transform.RotateAround(hit.point, hit.normal, Random.Range(-180, 180));
        // newSplatObject.transform.localScale = new Vector3(randScale, randScale * 0.5f, randScale) * _splatScale;
        newSplatObject.transform.RotateAround(hit.point, hit.normal, -180);
        newSplatObject.transform.localScale = new Vector3(4f, 0.75f, 4f) * _splatScale;

        Splat newSplat;
        newSplat.splatMatrix = newSplatObject.transform.worldToLocalMatrix;
        newSplat.channelMask = _splatColor;

        // Debug.Log(" newSplat.splatMatrix::::" + newSplat.splatMatrix);

        float splatscaleX = 1.0f / _splatsX;
        float splatscaleY = 1.0f / _splatsY;
        float splatsBiasX = Mathf.Floor(Random.Range(0, _splatsX * 0.99f)) / _splatsX;
        float splatsBiasY = Mathf.Floor(Random.Range(0, _splatsY * 0.99f)) / _splatsY;

        //固定模式
        newSplat.scaleBias = new Vector4(splatscaleX, splatscaleY, _splatSampleX, _splatSampleY);
        //  Debug.Log("newSplat.scaleBias:::" + newSplat.scaleBias + ",::"+ splatsBiasY);

        if (_hitObj != hit.transform.gameObject && !hit.transform.gameObject.tag.Contains("Weaponry"))
        {
            // Debug.Log("name::" + hit.transform.gameObject.name);
            _id = hit.transform.GetComponent<SplatManager>().ObjectId;
            _hitObj = hit.transform.gameObject;
        }

        SplatManagerSystem.instance.AddSplat(_id, newSplat);

        GameObjPool.GetInstance().DestroyObject(gameObject);
        ResetData();
        GameObject.Destroy(newSplatObject);

    }


    private void ResetData()
    {
        _beforePoint = Vector3.zero;
        _last2Point = Vector3.zero;
        _lastPoint = Vector3.zero;
    }

    /// <summary>
    /// 设置贴花相关数据
    /// </summary>
    /// <param name="splatSampleId"></param>
    /// <param name="splatScaleId"></param>
    public void SetSplatData(int splatSampleId, int splatScaleId, Vector4 splatColor)
    {
       
        _splatSampleX = SplatManagerSystem.instance.SplatSample_X[splatSampleId];  //设置贴花模型
        _splatSampleY = SplatManagerSystem.instance.SplatSample_Y[splatSampleId];  //设置贴花模型
        _splatScale = splatScaleId;  //设置贴花范围，即子弹的ShootRange
        _splatColor = splatColor;
        //if (splatScaleId == 0)
        //{
        //    _splatScale = 2f;
        //}
        //else
        //{
        //    _splatScale = 5f;
        //}
     //   Debug.Log("splatSampleX::" + _splatSampleX + ",_splatSampleY::" + _splatSampleY + ",splatScaleId::" + splatScaleId + ",splatColor::" + splatColor);
    }
}

