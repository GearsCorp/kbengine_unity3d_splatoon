using CBFrame.Core;
using GameLogic.Configs;
using KBEngine;
using System;
using UnityEngine;


public class CharacterMotion : CBComponent
{
    /// <summary>
    /// 人物移动的走法有两种：一种按圆的轨迹来移动；一种根据对应值的直线来移动
    /// MM_ON_ROUND:表示画圆走法； MM_ON_LINE:表示直线走法；
    /// </summary>
    public enum E_MotionMode
    {
        MM_ON_ROUND,
        MM_ON_LINE
    }

    /// <summary>
    /// 人物前方遇到坡度超过预设坡度值的物体，有两个表现模式：一种是进行跨面，一种是不进行跨面
    /// SM_ON_GROUND:表示非跨面模式；SM_ON_SURFACE: 表示跨面模式；
    /// </summary>
    public enum E_SurfaceMode
    {
        SM_ON_GROUND,
        SM_ON_SURFACE
    }

    /// <summary>
    /// 移动中遇到坡度比较小的面：贴面时的forward朝向处理：一种是按原有forward朝向，一种是按normal处理计算获取forward朝向
    /// FM_ON_FORWARD:表示按原有forward朝向； FM_ON_NORMAL:表示按normal处理计算获取forward朝向
    /// </summary>
    public enum E_ForWardMode
    {
        FM_ON_FORWARD,
        FM_ON_NORMAL
    }

    public FP MoveSpeed = 4f;                     //移动速度
    public FP RotationSpeed = 15f;              //旋转速度
    public FP Radius = 5;                       //设置走圆方式下，圆的半径
    public FP SlopeAngle = 45f;                 //设置最高坡度值

    public float GroundRayLength = 0.9f;           //射线长度
    public float BarrierLimit = 0.75f;             //射线向前检测长度
    public Transform RayPointObj;                  //设置一个射线点
    public Transform Pos;                          //为了后期方便查看圆心的点，扔个物体点进来作为圆心点

    public E_MotionMode MotionMode;                //设置行走方式
    public E_SurfaceMode SurfaceMode;               //设置跨面模式
    public E_ForWardMode ForWardMode;                //设置Forward贴面模式

    private FPVector _center;                       //设置走圆方式下，圆的真正中心点
    private Vector3 _tempCenterVec3 = Vector3.zero; //设置走圆方式下，圆的真正中心点，用于渲染的vec3

    private bool _bHasSpeed = true;                 //处理物体在面上行走突然滑落的速度标志，        

    public bool bMoveFlag = false;                 //是否处于移动状态 
    public bool bJumpFlag = false;                 //是否处于跳跃状态
    public bool bJumpArtFlag = false;             //是否处于跳跃状态
    private FP _jumpSpeed = 6;
    public  float JumpSpeed = 6;                   //设置跳高的向上速度

    private U_FPTransform _childObj;
    public bool MoveByInkFlag;                     //这是开启物体能否在未指定颜料中运动的阀门，物体在大于坡度的面上的为指定颜料中是否可以移动
    private U_FPTransform _ufpTransform;

    private FPVector _destVector = FPVector.zero;  //目标向量

    public bool bOnGround                       //检测是否在地面
    {
        get
        {
            return Physics.Raycast(RayPointObj.position, -transform.up, GroundRayLength);
        }
    }

    public  Transform CurGroundObj = null;     //表示当前脚踩的地面对象
    private int _layerNum = 0;                 //渲染的layer对象层

    private FP g = 9.81f;                      //重力加速度
    private FP _verCurSpeed;                   //增加的惯力速度

    public int AvatarEntityId;                 //移动对象Id


    private void Start()
    {
        init();
    }

    public void init()
    {
        if (_ufpTransform != null) return;
        MoveSpeed = 2f;          //移动速度                          
        _verCurSpeed = MoveSpeed; //设置为移动速度一致
        _childObj = gameObject.transform.Find(PlayerCommonName.ChildName).GetComponent<U_FPTransform>();

        MoveByInkFlag = true;
        _layerNum = LayerMask.NameToLayer("RenderLayer");
        _ufpTransform = gameObject.GetComponent<U_FPTransform>();
    }

    public bool testtest = false;

    public FP _destRoundData = 0;
    public FP _roundData = 0;

    public void Update()
    {
        if (_destVector != FPVector.zero)
        {
            float disData = Vector3.Distance(transform.position, _ufpTransform.position.ToVector());
            FP fixDistan = MoveSpeed * Time.deltaTime;
            if (disData <= fixDistan)
            {
                 UpdatePosition(_ufpTransform.position);
                 UpdateRotation(_ufpTransform.rotation);
                //   Debug.Log("======>arrived_____arrived_____arrived_____arrived_____arrived:::" + _ufpTransform.position);
            }
            else
            {
                transform.position += (_childObj.forward * fixDistan).ToVector();
                // Debug.Log("======>_ufpTransform.position_OnLine:::"+ _ufpTransform.position);
            }
           
           
        }

        if (_destRoundData != 0)
        {
            FP angleData = _destRoundData - _roundData;
            FP _tempData = RotationSpeed * MoveSpeed * Time.deltaTime;

            //Debug.Log("======>angleData:::" + angleData.AsFloat()
            //      + ",_tempData::" + _tempData.AsFloat() + ",_destRoundData::"+ _destRoundData);
            if (FPMath.Abs(angleData) <= FPMath.Abs(_tempData))
            {
                UpdatePosition(_ufpTransform.position);
                UpdateRotation(_ufpTransform.rotation);
                _roundData = _destRoundData;
                //Debug.Log("======>arrived_____round_____arrived_____round_____arrived:::" + _ufpTransform.position
                //      + ",childObj.forward::" + _childObj.forward + ",childObj.forward.toVector3::" + _childObj.forward.ToVector());
            }

            else
            {
                transform.RotateAround(_center.ToVector(), transform.up, _tempData.AsFloat());
                _roundData += _tempData;
                Debug.Log("======>_ufpTransform.round_round_OnLine:::" + _ufpTransform.position + ",_ufpTransform.position.toVector3::" + _ufpTransform.position
                //    + ", ufpTransform.rotation:::" + _ufpTransform.rotation
                //    + ",_ufpTransform.rotation.toVector()::" + _ufpTransform.rotation.ToQuaternion()
                    + ",childObj.forward::" + _childObj.forward + ",childObj.forward.toVector3::" + _childObj.forward.ToVector());
            }
        }


        RaycastHit hitInfo;
        bool bExitBarrier = (RayBarrierInfo(_childObj.forward.ToVector(), out hitInfo));

        RaycastHit _groundHit;
        RaycastHit _hitInfo;
        if (bJumpFlag)  //处理跳跃 
        {
            if (!bJumpArtFlag)//表示第一次开始跳跃
            {
                if (!RayGroundInfo(out _groundHit, true)) bJumpArtFlag = true;
            }
            else
            {
                if (RayGroundInfo(out _groundHit,true))
                {
                    bJumpFlag = false;
                    bJumpArtFlag = false;
                    return;
                }
            }
            _jumpSpeed = _jumpSpeed - g * FrameSyncManager.DeltaTime; //垂直上的初速度随时间的改变
            _ufpTransform.Translate(FPVector.up * _jumpSpeed * FrameSyncManager.DeltaTime, Space.World);//垂直上的运动
            UpdateRotation(_ufpTransform.rotation);
            UpdatePosition(_ufpTransform.position);
            //transform.Translate(Vector3.up * _jumpSpeed * FrameSyncManager.DeltaTime, Space.World);//垂直上的运动


            //if (_jumpSpeed < 0 && RayGroundInfo(out _groundHit, true))   //增加防止坠落的操作
            //{
            //    transform/*.parent*/.position = new Vector3(transform/*.parent*/.position.x, _groundHit.point.y, transform/*.parent*/.position.z);
            //    bJumpFlag = false;
            //    bJumpArtFlag = false;
            //    return;
            //}

            if (bMoveFlag)
            {
                _ufpTransform.Translate(_childObj.forward * MoveSpeed * FrameSyncManager.DeltaTime, Space.World);//水平上的运动
                UpdateRotation(_ufpTransform.rotation);
                UpdatePosition(_ufpTransform.position);
                //  transform.Translate(_childObj.forward.ToVector() * (MoveSpeed/** _frameEntityObj.SpeedRate*/).AsFloat() * Time.fixedDeltaTime, Space.World);//水平上的运动
            }
            return;

        }
        else if (!RayGroundInfo(out _groundHit) && !RayBarrierInfo(_childObj.forward.ToVector(), out _hitInfo) && _bHasSpeed)//空中调整角度
        {
            //Debug.Log("_childObj.forward.ToVector():::" + _childObj.forward.ToVector() 
            //    + ",,,,data__ufpTransform.up::" + _ufpTransform.up.ToVector() + ",,,,,fff::"+ _ufpTransform.forward.ToVector());
            _verCurSpeed = _verCurSpeed - g * FrameSyncManager.DeltaTime; //垂直上的初速度随时间的改变
            _ufpTransform.Translate(_childObj.forward * MoveSpeed * FrameSyncManager.DeltaTime, Space.World);//水平上的运动
            _ufpTransform.Translate(FPVector.up * _verCurSpeed * FrameSyncManager.DeltaTime, Space.World);//垂直上的运动
            UpdateRotation(_ufpTransform.rotation);
            UpdatePosition(_ufpTransform.position);

            //  transform.Translate(_childObj.forward.ToVector() * (MoveSpeed/** _frameEntityObj.SpeedRate*/).AsFloat() * Time.fixedDeltaTime, Space.World);//水平上的运动
            //  transform.Translate(Vector3.up * _verCurSpeed.AsFloat() * Time.fixedDeltaTime, Space.World);//垂直上的运动

            FP angleForward = FPVector.Angle(_ufpTransform.up, FPVector.up);
            if (angleForward == 0) return;
           
            FPVector normal = FPVector.Cross(_ufpTransform.up, FPVector.up);
            // int DirctData = FPMath.Sign(FPVector.Dot(normal, _ufpTransform.up));
            float DirctData = Mathf.Sign(Vector3.Dot(normal.ToVector(), _ufpTransform.up.ToVector()));

            //Debug.Log(" angleForward::" + angleForward.AsFloat() + ",DirctData::"+ DirctData + ",_ufpTransform.up::"+ _ufpTransform.up + "  ,"+  _ufpTransform.up.ToVector() + ",FPVector.up::"+ FPVector.up.ToVector()
            //    + ",normal::" + normal + "," + normal.ToVector()  + ", FPVector.Dot(normal, _ufpTransform.up)::" + FPVector.Dot(normal, _ufpTransform.up).AsFloat());
            //if (DirctData == 0) DirctData = 1;
            angleForward = angleForward * DirctData;

         //   Debug.Log(" FPMath.Sign(FPVector.Dot(normal, _ufpTransform.up)::" + FPVector.Dot(new FPVector(0,0,1), new FPVector(1, 0, 0)));
            FPVector forwardVec3 = FPQuaternion.AngleAxis(angleForward, normal) * _ufpTransform.up;
            FPVector forwardForward = FPQuaternion.AngleAxis(angleForward, normal) * _ufpTransform.forward;

            FPQuaternion qur = FPQuaternion.LookRotation(forwardForward, forwardVec3);
            //Debug.Log("forwardForward:::" + forwardForward.ToVector() + ",,forwardVec3::" + forwardVec3.ToVector()
            //    + ",angleForward::"+ angleForward.AsFloat() 
            //    + ",_ufpTransform.up::" + _ufpTransform.up.ToVector() + ", _ufpTransform.forward::" + _ufpTransform.forward.ToVector()
            //    + ",normal::" + normal.ToVector());
            UpdateRotation(FPQuaternion.Slerp(_ufpTransform.rotation, qur, 0.1f));
            //_ufpTransform.SetRotation();
            //transform.rotation = FPQuaternion.Slerp(_ufpTransform.rotation, qur, 0.1f);

            //将玩家处于空中的状态事件发射出去
            TriggerEvent(DefineEventId.PlayerInAirEvent);
        }
        else
        {
            FP angle = FPVector.Angle(FPVector.up, _ufpTransform.ChangeVec3ToTSVec(_groundHit.normal));
            if (angle > SlopeAngle)
            {
                if (!bMoveFlag)
                {
                    _ufpTransform.Translate(-1*(FPVector.up) * g * FrameSyncManager.DeltaTime, Space.World);
                    UpdateRotation(_ufpTransform.rotation);
                    UpdatePosition(_ufpTransform.position);
                }
            }
            else
            {
                // transform.position = new Vector3(transform/*.parent*/.position.x, _groundHit.point.y, transform/*.parent*/.position.z);
                UpdateRotation(_ufpTransform.rotation);
                UpdatePosition(new FPVector(_ufpTransform.position.x, (FP)(_groundHit.point.y), _ufpTransform.position.z));
            }
        }

    }

    /// <summary>
    /// 一帧渲染时间到后的数据更新
    /// </summary>
    public void RenderEndUpdatePosition()
    {
        if (_destVector != FPVector.zero || _destRoundData != 0)
        {
            _destVector = FPVector.zero;
            _destRoundData = 0;
            _roundData = 0;
            UpdatePosition(_ufpTransform.position);
            UpdateRotation(_ufpTransform.rotation);
        }

     //   Debug.LogError("======>RenderEndUpdatePositionRenderEndUpdatePosition:::" + _ufpTransform.position + "  ," +transform.position
       //      + ",childObj.forward::" + _childObj.forward + ",childObj.forward.toVector3::" + _childObj.forward.ToVector());
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (!bMoveFlag && _ufpTransform)
        {
            RaycastHit _groundHit;
            if (Physics.Raycast(RayPointObj.transform.position, -Vector3.up, out _groundHit, GroundRayLength)) //悬空
            {
                FPVector fpNormal = _ufpTransform.ChangeVec3ToTSVec(_groundHit.normal);
                if (_ufpTransform.up == fpNormal) return;
              
                FP angle              = FPVector.Angle(_ufpTransform.up, fpNormal);
                FPVector fpPoint      = _ufpTransform.ChangeVec3ToTSVec(_groundHit.point);
                FPVector newCrossVec3 = FPVector.Cross(_ufpTransform.up, fpNormal);
                if (_childObj)
                {
                    FPVector forward = FPQuaternion.AngleAxis(angle, newCrossVec3) * (_ufpTransform.forward);
                    FitSurfaceParent(forward, fpPoint, fpNormal);
                }
            }
        }
    }


    #region  1) =========对象移动等相关操作=========
    public void Move()
    {
        if (bJumpFlag) return;
        FPVector groundNormal = FPVector.zero;
        RaycastHit hitInfo;
        if (!RayGroundInfo(out hitInfo))
        {
            if (!bJumpFlag)    bMoveFlag = false;
            return;  //如果离地,直接返回
        }
        else
        {
            groundNormal = new FPVector(hitInfo.normal.x, hitInfo.normal.y, hitInfo.normal.z);
        }

        if (!bMoveFlag) bMoveFlag = true;
        FPVector forward  = _ufpTransform.forward;
        FPVector fpNormal = _ufpTransform.ChangeVec3ToTSVec(hitInfo.normal);

        RaycastHit barrierInfo;
        bool bExitBarrier = (RayBarrierInfo(_childObj.forward.ToVector(), out barrierInfo)); //表示前方检测到物体,暂时把检测到的物体都称之为障碍物
        bool bSlope = false; //默认不是为斜坡标志 
        if (bExitBarrier)
        {
            hitInfo = barrierInfo;
            bSlope = (Vector3.Angle(Vector3.up, hitInfo.normal) > SlopeAngle) ? true : false;

            if (ForWardMode == E_ForWardMode.FM_ON_FORWARD)
            {
                forward = _ufpTransform.forward;
            }
            else
            {
                FP angle = 0;
                fpNormal = _ufpTransform.ChangeVec3ToTSVec(hitInfo.normal);
                if (groundNormal != FPVector.zero)
                {
                    angle = FPVector.Angle(groundNormal, fpNormal);
                    angle = -angle;
                }

                if (_childObj)
                {
                    //Vector3 newCrossVec3 = Vector3.Cross(hitInfo.normal, transform.up);
                    //forward = Quaternion.AngleAxis(angle, newCrossVec3) * (transform.forward);
                    FPVector newCrossVec3 = FPVector.Cross(fpNormal, _ufpTransform.up);
                    forward = FPQuaternion.AngleAxis(angle, newCrossVec3) * _ufpTransform.forward;
                }
            }
        }
        else
        {
            bSlope = ((FPVector.Angle(FPVector.up, fpNormal)) > SlopeAngle) ? true : false;
        }

        if (SurfaceMode == E_SurfaceMode.SM_ON_GROUND)   //非跨面模式：供人型和乌贼模式下使用
        {
            if (bExitBarrier && bSlope) return;
        }
        else if (SurfaceMode == E_SurfaceMode.SM_ON_SURFACE) //跨面模式：供乌贼下潜模式下使用
        {
            if (bSlope)
            {
               
                MotionMode = E_MotionMode.MM_ON_LINE;
                bool flag = CanMoveByInk(ref hitInfo);  //上墙后到边角的处理 ==>TODO
                flag = true;
                if (!flag)
                {
                    FPVector position = _ufpTransform.ChangeVec3ToTSVec(hitInfo.point) + (_childObj.forward * MoveSpeed * 0.1f);
                    FPVector currentPosition = _ufpTransform.position;
                    //_ufpTransform.position = position;
                    //transform.position     = position.ToVector();
                    UpdatePosition(_ufpTransform.position);
                    RayGroundInfo(out barrierInfo);
                    flag = CanMoveByInk(ref barrierInfo);
                    // transform.position = currentPosition.ToVector();
                    UpdatePosition(_ufpTransform.position);
                    if (bExitBarrier || !flag)
                    {
                        return;
                    }
                }
            }
            
        }
        if (hitInfo.transform != null && ForWardMode == E_ForWardMode.FM_ON_NORMAL)
        {
            FitSurfaceParent(forward, _ufpTransform.ChangeVec3ToTSVec(hitInfo.point), fpNormal);  //调整贴面
        }
        else if (hitInfo.transform != null && ForWardMode == E_ForWardMode.FM_ON_FORWARD)
        {
            if (transform.up != Vector3.up)
            {
                FP quaternionSpeed = 0.09f;
                FPQuaternion data = new FPQuaternion(0, _ufpTransform.rotation.y, 0, _ufpTransform.rotation.w);
                _ufpTransform.SetRotation(FPQuaternion.Slerp(_ufpTransform.rotation, data, quaternionSpeed));
            }

        }
        if (MotionMode == E_MotionMode.MM_ON_ROUND) //画圆模式：按照圆的弧度来行走
        {
            _destRoundData = RotationSpeed * MoveSpeed * FrameSyncManager.DeltaTime;
            _ufpTransform.RotateAround(_center, _ufpTransform.up, _destRoundData);
            //_destUfpTransform.position = _ufpTransform.position;
            //_destUfpTransform.rotation = _ufpTransform.rotation;
            //_destUfpTransform.RotateAround(_center, _ufpTransform.up, _destRoundData);
           
            Debug.Log("----------------->_characterMotion:::" + _destRoundData + ",_destRoundData::" + _destRoundData.AsFloat()
              + ",childObj.forward::" + _childObj.forward + ",childObj.forward.toVector3::" + _childObj.forward.ToVector());
        }
        else if (MotionMode == E_MotionMode.MM_ON_LINE)//直走模式：按照其forward的方向行走
        {
            _destVector = _childObj.forward * MoveSpeed * FrameSyncManager.DeltaTime;
            _ufpTransform.Translate(_destVector, Space.World);
            //_destUfpTransform.position = _ufpTransform.position;
            //_destUfpTransform.rotation = _ufpTransform.rotation;
            //_destUfpTransform.Translate(_destVector, Space.World);

        }

       // if (Physics.Raycast(RayPointObj.position + (_destUfpTransform.position - _ufpTransform.position).ToVector(), -transform.up, GroundRayLength))
        if (Physics.Raycast(RayPointObj.position + (_ufpTransform.position.ToVector() - transform.position), -transform.up, GroundRayLength))
        {
            _bHasSpeed = false;
        }
        else
        {
            _bHasSpeed = true;
            RenderEndUpdatePosition();

        }
        _verCurSpeed = MoveSpeed/2;
    }

    public void Idle()
    {
        if (bMoveFlag) bMoveFlag = false;
    }

    public void Jump(bool moveFlag = false)
    {
        if (!bOnGround) return;
        bJumpFlag = true;
        bJumpArtFlag = false;
        _jumpSpeed = JumpSpeed;
    }

    #endregion

    #region  2) =========地面检测和移动数据换算等相关操作=========
    public bool RayGroundInfo(out RaycastHit _groundHit, bool renderLine = false)  //检测是否在地面
    {
      //   Debug.DrawLine(RayPointObj.position, RayPointObj.position +(-transform.up * GroundRayLength), Color.green);
        if (Physics.Raycast(RayPointObj.position, -transform.up, out _groundHit, GroundRayLength))
        {
            if(renderLine)
                Debug.DrawRay(RayPointObj.position, -transform.up * GroundRayLength, Color.yellow);
            return true;
        }
        return false;

    }

    /// <summary>
    /// 检测前方是否有物体，这里暂时把检测的物体都统一叫为障碍物
    /// </summary>
    /// <param name="forward"></param>
    /// <param name="_barrierHit"></param>
    /// <returns></returns>
    public bool RayBarrierInfo(Vector3 forward, out RaycastHit _barrierHit)
    {
        //这里设置两条射线的原因：为了适应不同的墙体，墙体是凹凸不确定的，在凹墙面时，我们将射线点提高，防止冲突（这里只是做了些简单的处理）
        var barrierVec3 = RayPointObj.transform.position + (-RayPointObj.transform.up * BarrierLimit);
        // return (Physics.Raycast(barrierVec3, forward, out _barrierHit, 1f));
        if (Physics.Raycast(barrierVec3, forward, out _barrierHit, 1f))
        {
             Debug.DrawRay(barrierVec3, forward * 1f, Color.red);
            return true;
        }
        else if (Physics.Raycast(barrierVec3 + (-RayPointObj.transform.up * 0.4f), forward, out _barrierHit, 1f))
        {
            //  Debug.DrawRay(barrierVec3 + (-RayPointObj.transform.up * 0.4f), forward * 1f, Color.red);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 跨面操作
    /// </summary>
    /// <param name="forward"></param>
    /// <param name="position"></param>
    /// <param name="normal"></param>
    public void FitSurfaceParent(FPVector forward, FPVector position, FPVector normal)
    {
        FPVector left = FPVector.Cross(forward, normal);   //切线
        FPVector newForward = FPVector.Cross(normal, left);
        if (normal == FPVector.zero)
        {
            Debug.Log("forward:::" + forward);
            return;
        }
        FPQuaternion newRotation = FPQuaternion.LookRotation(newForward, normal);

        UpdateRotation(newRotation);
        UpdatePosition(position);
        //Debug.Log("  _ufpTransform.up::" + _ufpTransform.up.ToVector() + "newRotation::" + newRotation.ToQuaternion()
        //    + ",forward::" + forward.ToVector() + ",normal:::" + normal.ToVector() + ",newForward::" + newForward.ToVector());
    }

    public FPVector GetCenterPoint()
    {
        return _center;
    }
    public void SetCenterPoint(FPVector value)
    {
        _center = value;
    }

    /// <summary>
    /// true:表示地面坡度超过预设值; false:表示地面坡度在坡度预设值之内
    /// </summary>
    /// <returns></returns>
    public bool JudgetGroundSlope()
    {
        RaycastHit hitInfo;
        if (RayGroundInfo(out hitInfo) && JudgetGroundSlope(hitInfo.normal))
        {
            return true;
        }
        return false;
    }

    public bool JudgetGroundSlope(out RaycastHit hitInfo)
    {
        if (RayGroundInfo(out hitInfo) && JudgetGroundSlope(hitInfo.normal))
        {
            return true;
        }
        return false;
    }

    public bool JudgetGroundSlope(Vector3 normal)
    {
        if (Vector3.Angle(Vector3.up, normal) > SlopeAngle)
            return true;
        return false;
    }

    public void ChangeRoundCenter(FP _joystickAngle)
    {
        FPVector rv = _childObj.right;
        if (_joystickAngle > 90 && _joystickAngle < 270)
        {
            rv *= -1;
        }

        FP RvRate = 1; //这是为了根据角度值，求圆心
        if (_joystickAngle > 180)
        {
            RvRate += FPMath.Sin((_joystickAngle - 180) * FPMath.Deg2Rad);
        }
        else
        {
            RvRate += FPMath.Sin((_joystickAngle) * FPMath.Deg2Rad);
        }

        _center = rv * Radius * RvRate;
        _center =_childObj.position + _center;
        RotationSpeed = FPMath.Abs(RotationSpeed);
        if (_joystickAngle > 90 && _joystickAngle < 270)
        {
            RotationSpeed = FPMath.Abs(RotationSpeed) * -1;
        }
    }


    #endregion

    #region  3) =========对像素读取和渲染等相关操作=========

    public bool OnGrouldRayInfoAndRender(out RaycastHit _groundHit)
    {
        if (!RayGroundInfo(out _groundHit)) return false;

        if (_groundHit.transform.gameObject.layer != _layerNum) return false;
        if (CurGroundObj == null || CurGroundObj.name != _groundHit.transform.name)
        {
            CurGroundObj = _groundHit.transform;
            SplatManager renderObj = _groundHit.transform.GetComponent<SplatManager>();
            if (renderObj == null)
            {
                Debug.Log("SplatManager_name_name_name:::" + _groundHit.transform.name);
                return false;
            }

            if (PixelManager.GetInstance() == null)
            {
                Debug.Log("renderObj.ObjectId_PixelManager.GetInstance() is null!!!:::" + renderObj.ObjectId);
                return false;
            }

            //先判断是否已经有玩家对象和渲染对象了,根据情况添加玩家对象和渲染对象
            if (!PixelManager.GetInstance().SetPlayerIdToObjectID(AvatarEntityId, renderObj.ObjectId))
            {
                Texture2D obj = RenderTexturePool.GetInstance().InstanceTexture2D();
                renderObj.SetRenderTexture2D(ref obj);
                PixelManager.GetInstance().SetObjectIDToTexture2D(renderObj.ObjectId, obj);
                Debug.Log("PlayerIDToObjectID.GetInstance():::" + PixelManager.GetInstance().PlayerIDToObjectID.Count + " ," + PixelManager.GetInstance().PlayerIDToObjectID.Keys);
            }
            //  bool data = PixelManager.GetInstance().GetPixelsInfo(PlayerId, _groundHit.textureCoord);
        }
        return true;
    }

    public void AutoRenderSplatData(int entityId)
    {
        init();
        AvatarEntityId = entityId;
        RaycastHit _groundHit;
        OnGrouldRayInfoAndRender(out _groundHit);
    }

    public bool CanMoveByInk(ref RaycastHit rayHitInfo)
    {
        if (rayHitInfo.transform == null) return false;
        if (rayHitInfo.transform.gameObject.layer != _layerNum) return false;
        //if (CurGroundObj == null || CurGroundObj.name != rayHitInfo.transform.name)
        //{
        //    CurGroundObj = rayHitInfo.transform;
        SplatManager renderObj = rayHitInfo.transform.GetComponent<SplatManager>();
        if (renderObj == null)
        {
            Debug.Log("SplatManager_name_name_name:::" + rayHitInfo.transform.name);
            return false;
        }

        if (PixelManager.GetInstance() == null)
        {
            Debug.Log("renderObj.ObjectId_PixelManager.GetInstance() is null!!!:::" + renderObj.ObjectId);
            return false;
        }

        //先判断是否已经有玩家对象和渲染对象了,根据情况添加玩家对象和渲染对象
        if (!PixelManager.GetInstance().ObjectIDToTexture2D.ContainsKey(renderObj.ObjectId))
        {
            Texture2D obj = RenderTexturePool.GetInstance().InstanceTexture2D();
            renderObj.SetRenderTexture2D(ref obj);
            PixelManager.GetInstance().SetObjectIDToTexture2D(renderObj.ObjectId, obj);
            renderObj.RenderTextureToStaticData();
            Debug.Log("PlayerIDToObjectID.GetInstance():::" + PixelManager.GetInstance().PlayerIDToObjectID.Count + " ," + PixelManager.GetInstance().PlayerIDToObjectID.Keys);
        }
        return PixelManager.GetInstance().IsInOneSelfInk(AvatarEntityId, renderObj.ObjectId, rayHitInfo.textureCoord);
        //  bool data = PixelManager.GetInstance().GetPixelsInfo(PlayerId, _groundHit.textureCoord);
        //   }

    }

    #endregion

    #region  4) =========定点数、物理引擎等相关函数=========

    private void UpdateRotation(FPQuaternion rotation)
    {
        _ufpTransform.SetRotation(rotation);

        _childObj.UpdateFpRotation(new FPQuaternion(_childObj.transform.rotation.x, _childObj.transform.rotation.y, _childObj.transform.rotation.z, _childObj.transform.rotation.w));


    }
    private void UpdatePosition(FPVector position)
    {
        _ufpTransform.SetPosition(position);
        _childObj.SetPosition(position);
    }

    #endregion


}



