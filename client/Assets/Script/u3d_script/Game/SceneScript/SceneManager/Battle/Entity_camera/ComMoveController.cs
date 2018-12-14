using CBFrame.Sys;
using GameLogic.Configs;
using KBEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComMoveController : FrameSyncBehaviour
{

    //对象与目标距离及滚轮相关参数
    public FP Distance = 7.0f; //当前对象与目标物体之间的距离 
    public float Height = 1.5f;
    //水平滚动相关  
    public float DistanceMax = 10.0f;  //当前对象与目标物体之间的最大距离  
    public float DistanceMin = 3.0f;   //当前对象与目标物体之间的最小距离  
    //垂直滚动相关  
    public float MouseScrollWheelSensitivity = 1.0f; //鼠标滚轮灵敏度（备注：鼠标滚轮滚动后将调整当前对象与目标物体之间的间隔）  


    //左右遥感控制
    public  enum RockerControl { None = 0, LeftControl, RightControl, UpControl, DownControl, OtherControl, ClimbControl, AirControl }
    public RockerControl _rockerControl = RockerControl.OtherControl;
    public  float RockerSpeed = 200f;
    public  float LerpSpeed = 5f;
    public  bool  RotateFollowFlag = true;

    public  bool _bTouchMouse = false;      //是否触碰摇杆
    public  bool _bBeforeDragging = false;  //标志之前是否有划过屏幕，调节镜头
    public  bool _bVaildDrag = false;       //有效的拖拽/排除标题栏拖动事件信息触发的消息
    private bool _dragging = false;         //标记是否鼠标在滑动 
    private bool _directorToLeft = false;
    private bool _directorToRight = false;

    private Vector2 _firstPoint = Vector2.zero;     //记录鼠标点击的初始位置  
    private Vector2 _secondPoint = Vector2.zero;    //记录鼠标移动时的位置  


    //虚拟摇杆相关参数
    public  float Angle;              //摇杆传递的角度
    private FP _currentAngle = 0;  //对象旋转的角度
    private FP _offsetAngle  = 0;  //对象产生的偏转角度
    public  Rect TouchRect;           //当前虚拟摇杆的UI范围大小


    //外部对象传递
    public  Transform       Target;   //跟踪的目标
    private CharacterMotion _motionObj;
    private Avatar          _avatarObj;
    private U_FPTransform  _childTransformObj;
    private U_FPTransform _playerTransformObj;
    private Transform      _rayPointObj;
   // private bool           _hasCamera = false;   //看是否挂载摄像机

    //数据存储
    private FPQuaternion _rotation;                          //存储当前摄像机的rotation数据
    private ComMoveController.RockerControl _beforeControl = ComMoveController.RockerControl.None; //保持上一次镜头的控制方式


    //爬墙相关的数据
    private bool _climbOtherWall = false;
    private FPVector _beforeGroundNormalVec3 = FPVector.zero;
    private FPVector _beforeWallNormalVec3  = FPVector.zero;
    private FPVector _finishWallNormalVec3  = FPVector.zero;

    private U_FPTransform _ufpTransform;   

    private void Awake() 
    {
        _motionObj = Target.GetComponent<CharacterMotion>();
        _avatarObj = Target.GetComponent<Avatar>();
        _childTransformObj  = Target.transform.Find(PlayerCommonName.ChildName).GetComponent<U_FPTransform>();
        _playerTransformObj = Target.GetComponent<U_FPTransform>();
        _rayPointObj        = _childTransformObj.transform.Find("RayPoint");
        _ufpTransform       = gameObject.GetComponent<U_FPTransform>();
        Debug.Log("SetTarget is success!!");

        //test ..先主动加载摄像机
        if (gameObject.transform.parent.transform.name == "AvatarPlayer")
            SetCameraObj();

    }
    private void Start()
    {

    }
    private void OnDestroy()
    {
        Destroy(_avatarObj);
        Destroy(_motionObj);
        Destroy(_childTransformObj);
        Destroy(_rayPointObj);

        Resources.UnloadUnusedAssets();
    }


    public void Update()
    {

        if (Target == null) return;
      //  ChangeDirectionByDragging();
        //if (_dragging && _directorToLeft && !_bTouchMouse)  //左遥感
        //{
        //    _rockerControl = ComMoveController.RockerControl.LeftControl;
        //    _ufpTransform.RotateAround(_playerTransformObj.position, FPVector.up, FrameSyncManager.DeltaTime * RockerSpeed);
        //    _ufpTransform.UpdateAllData();
        //    Debug.Log("_ufpTransform:::" + _ufpTransform.forward.ToVector());
        //    //transform.RotateAround(_childObj.position, Vector3.up, Time.deltaTime * RockerSpeed);
        //    return;
        //}
        //else if (_dragging && _directorToRight && !_bTouchMouse) //右遥感
        //{
        //    _rockerControl = ComMoveController.RockerControl.RightControl;
        //    _ufpTransform.RotateAround(_playerTransformObj.position, FPVector.up, FrameSyncManager.DeltaTime * -RockerSpeed);
        //    _ufpTransform.UpdateAllData();

        //    // transform.RotateAround(_childObj.position, Vector3.up, Time.deltaTime * -RockerSpeed);
        //    return;
        //}


        //Debug.Log("_currentAngle:::" + _currentAngle + ",_childObj.eulerAngles.y::" + _childObj.eulerAngles.y 
        //    + ",transform.rotation::"+ transform.rotation);

        RaycastHit GroundHitInfo;
        RaycastHit BarrieHitrInfo;
        bool bGroundInfoFlag  = _motionObj.RayGroundInfo(out GroundHitInfo);
        bool bBarrierInfoFlag = _motionObj.RayBarrierInfo(_childTransformObj.forward.ToVector(), out BarrieHitrInfo);

        FPVector fpGroundInfoNormal = FPVector.zero;
        if (bGroundInfoFlag)
            fpGroundInfoNormal = GroundHitInfo.normal.ToFPVector();

        if (!_dragging && !_bTouchMouse && _bBeforeDragging)
        {
            _rockerControl = ComMoveController.RockerControl.None;
            return;
        }
        else if (!_dragging)
        {  
            if (_bBeforeDragging)  //在这里设置值，为了保证childObj的forward朝向跟摄像机的一致，防止先后差值
            {
                _bBeforeDragging = false;
                return;
            }

            if (!bGroundInfoFlag)  //在空中
            {
                _rockerControl = ComMoveController.RockerControl.AirControl;
                if (_motionObj.bJumpFlag && false)  //这是处理不跟随着对象跳跃的逻辑部分
                {
                   
                    Distance = FPMath.Clamp(Distance - (Input.GetAxis("Mouse ScrollWheel") * MouseScrollWheelSensitivity), DistanceMin, DistanceMax);
                    FPVector DataVec3 = new FPVector(_playerTransformObj.position.x, transform.position.y - Height, _playerTransformObj.position.z);
                    _ufpTransform.position = (_ufpTransform.rotation * new FPVector(.0f, Height, -Distance)) + DataVec3;
                    _ufpTransform.UpdateAllData();
                    return;
                }
            }
            else if (!bBarrierInfoFlag || !_motionObj.JudgetGroundSlope(BarrieHitrInfo.normal))  //有地面接触但前方没有障碍物
            {
                if (!_motionObj.JudgetGroundSlope(GroundHitInfo.normal))
                {
                    _rockerControl = ComMoveController.RockerControl.OtherControl;
                    _climbOtherWall = false;
                }
                else
                {
                    if (_climbOtherWall && _beforeWallNormalVec3 != fpGroundInfoNormal)  //表示从一面墙跨到另外一面墙
                    {
                        _beforeGroundNormalVec3 = _beforeWallNormalVec3;
                        _beforeWallNormalVec3   = fpGroundInfoNormal;
                    }
                    _rockerControl = ComMoveController.RockerControl.ClimbControl;
                }

            }
            else //有地面接触且前方有障碍物
            {
                _rockerControl = ComMoveController.RockerControl.None;
                _beforeControl = ComMoveController.RockerControl.OtherControl;
                if (!_motionObj.JudgetGroundSlope(GroundHitInfo.normal)) //从地面跨到墙上的情况
                {
                    _beforeGroundNormalVec3 = fpGroundInfoNormal;
                }
                else                                                   //从一面墙跨到另外一面墙的情况
                {
                    _climbOtherWall = true;
                    _beforeWallNormalVec3 = fpGroundInfoNormal;   //设置这个变量的原因是：有可能检测到障碍物，但是玩家并没有跨越过去
                }
            }
        }

        if (_rockerControl == RockerControl.AirControl)
        {
            Distance = FPMath.Clamp(Distance - (Input.GetAxis("Mouse ScrollWheel") * MouseScrollWheelSensitivity), DistanceMin, DistanceMax);
            _ufpTransform.position = (_ufpTransform.rotation * new FPVector(.0f, Height, -Distance)) + _playerTransformObj.position;
            _ufpTransform.UpdateAllData();
        }
        else if (_rockerControl == RockerControl.OtherControl)
        {
            var quaternion = FPQuaternion.AngleAxis((_currentAngle) + _childTransformObj.transform.eulerAngles.y, FPVector.up);
            Distance = FPMath.Clamp(Distance - (Input.GetAxis("Mouse ScrollWheel") * MouseScrollWheelSensitivity), DistanceMin, DistanceMax);
            _ufpTransform.SetRotation(quaternion);
            // transform.rotation = quaternion;
            FPVector data = (_ufpTransform.rotation * new FPVector(.0f, Height, -Distance));
           
            _ufpTransform.position = (_ufpTransform.rotation * new FPVector(.0f, Height, -Distance)) + _playerTransformObj.position;
            _ufpTransform.UpdatePosition();
            //Debug.Log("data::"+ data.ToVector()+ ", _ufpTransform.position::::" + _ufpTransform.position.ToVector()
            //    + ",transform::" + transform.position + "::_playerTransformObj.position:" + _playerTransformObj.position.ToVector() 
            //    + ",,name::" + _playerTransformObj.gameObject.name);
            _rotation = _ufpTransform.rotation;
            _ufpTransform.UpdateForward();

        }
        else if (_rockerControl == RockerControl.ClimbControl)
        {
            Distance = FPMath.Clamp(Distance - (Input.GetAxis("Mouse ScrollWheel") * MouseScrollWheelSensitivity), DistanceMin, DistanceMax);
            var quaternion = FPQuaternion.AngleAxis((0) + transform.eulerAngles.y, FPVector.up);
            _ufpTransform.position = (_ufpTransform.rotation * new FPVector(.0f, Height, -Distance)) + _playerTransformObj.position;
            _ufpTransform.UpdateAllData();

            FPVector climbForward = _ufpTransform.forward;

            if (_beforeControl == ComMoveController.RockerControl.OtherControl && _beforeGroundNormalVec3 != FPVector.zero)
            {
                FP tempAngle   = FPVector.Angle(_beforeGroundNormalVec3, fpGroundInfoNormal);
                FPVector normal = FPVector.Cross(_beforeGroundNormalVec3, fpGroundInfoNormal);//叉乘求出法线向量
                                                                                              //  num *= Mathf.Sign(Vector3.Dot(normal, info.transform.up));  //求法线向量与物体上方向向量点乘，结果为1或-1，修正旋转方向
                climbForward = FPQuaternion.AngleAxis((90 - tempAngle), normal) * fpGroundInfoNormal;
                climbForward = -1 * climbForward;
                _finishWallNormalVec3 = climbForward;
                _beforeControl = ComMoveController.RockerControl.ClimbControl;
            }

            FP forwardAngle = FPVector.Angle(_finishWallNormalVec3, _ufpTransform.forward);
            if (forwardAngle != 0 && false)  //处理摄像机角度偏转
            {
                //1)调整摄像机的旋转角度
                float direcFlag = -1;
                FPVector normalVec3 = FPVector.Cross(_finishWallNormalVec3, _ufpTransform.forward);//叉乘求出法线向量
                direcFlag *= FPMath.Sign(Vector3.Dot(normalVec3.ToVector(), _ufpTransform.up.ToVector()));  //求法线向量与物体上方向向量点乘，结果为1或-1，修正旋转方向
                forwardAngle *= direcFlag;

                FPVector beforeForward = _ufpTransform.forward;
                FPVector forward = FPQuaternion.AngleAxis(forwardAngle, _ufpTransform.up) * _ufpTransform.forward;
             //   Debug.Log("_ufpTransform.forward::" + _ufpTransform.forward.ToVector() + ",forward::" + forward.ToVector() 
             //        + "forwardAngle:::" + forwardAngle.AsFloat() + ",forward1111::" + forward);
                float quaternionSpeed = 0.003f;
                if (!_bTouchMouse) quaternionSpeed = 0.03f;
                if (beforeForward != forward)
                {
                    Debug.Log("LookRotation(forward):::" + FPQuaternion.LookRotation(forward) + ",_rotation::" + _rotation + ",unity::" + Quaternion.LookRotation(forward.ToVector()));
                   
                    _rotation = FPQuaternion.Slerp(_rotation,FPQuaternion.LookRotation(forward), quaternionSpeed);
                    _ufpTransform.SetRotation(_rotation);
                }
             //   Debug.Log(",forward::"+ forward.ToVector() + ",_ufpTransform.forward::" + _ufpTransform.forward.ToVector());
                //2)调整人物的旋转角度
                if (!_climbOtherWall)  // 这是从地面爬到墙得处理，如果是从一面墙爬到另外一面墙，镜头不做转换
                {
                    Debug.Log("beforeForward:::" + beforeForward.ToVector() + ",_ufpTransform.forward::" + _ufpTransform.forward.ToVector());
                    _offsetAngle = FPVector.Angle(beforeForward, _ufpTransform.forward) * direcFlag;
                    _avatarObj.ChangeAvaterForward(_offsetAngle);
                }
            }

        }

        Debug.DrawLine(_ufpTransform.position.ToVector(), _playerTransformObj.transform.position, Color.red);
     
        if (_rockerControl == RockerControl.OtherControl ||
         _rockerControl == RockerControl.ClimbControl)
        {
            //看是否有障碍物
            FPVector directionTarget = (_ufpTransform.position - _ufpTransform.ChangeVec3ToTSVec(_rayPointObj.position)).normalized;
            FP distance = FPVector.Distance(_ufpTransform.position, _ufpTransform.ChangeVec3ToTSVec(_rayPointObj.position));
            if (distance > Distance)
            {
                _ufpTransform.Translate(directionTarget * (distance - Distance));
                _ufpTransform.UpdateRotationAndPosition();
            }

            //  Debug.DrawRay(_rayPointObj.position, directionTarget * Distance, Color.black);
            int layerMask = LayerMask.GetMask(Layers.Render);
            RaycastHit info;
            if (Physics.Raycast(_rayPointObj.position, directionTarget.ToVector(), out info, Distance.AsFloat(),layerMask))  //如果
            {
             //   Debug.Log("info.name::" + info.transform.name);
                if (info.transform.name != transform.name /*&& info.transform.tag != Tags.Ground*/)
                {
                    _ufpTransform.SetPosition(_ufpTransform.ChangeVec3ToTSVec(info.point));
                    //transform.position = info.point;
                }
                if (_rockerControl == RockerControl.OtherControl)
                {
                    _beforeControl = RockerControl.OtherControl;
                }
            }
        }
    }


    #region  1)============= 摇杆控制 ============= 

    /// <summary>
    /// 设置摄像头操作选项
    /// </summary>
    /// <param name="rockerControl"></param>
    public void SetRockerControl(RockerControl rockerControl)
    {
        _rockerControl = rockerControl;
    }

    /// <summary>
    /// 设置是否操作摇杆标志
    /// </summary>
    /// <param name="moveFlag"></param>
    public void SetJoyStickMoveFlag(bool moveFlag)
    {
        _bTouchMouse = moveFlag;
    }

    /// <summary>
    /// 设置鼠标启动拖拽操作标志
    /// </summary>
    /// <param name="mouseDragFlag"></param>
    public void SetMouseDragFlag(bool mouseDragFlag)
    {
        _dragging = mouseDragFlag;
        if (_dragging)
        {
            _bBeforeDragging = _dragging;
            ChangeDirectionByDragging();
        }
    }

    /// <summary>
    /// 设置鼠标拖拽方向
    /// </summary>
    /// <param name="leftDirection"></param>
    /// <param name="rightDirection"></param>
    public void SetMouseDragDirection(bool leftDirection, bool rightDirection)
    {
        _directorToLeft = leftDirection;
        _directorToRight = rightDirection;
    }

    /// <summary>
    /// 通过拖拽改变摄像头方向：向左旋转和向右旋转
    /// </summary>
    public void ChangeDirectionByDragging()
    {
        if (_dragging && _directorToLeft && !_bTouchMouse)  //左遥感
        {
            _rockerControl = ComMoveController.RockerControl.LeftControl;
            _ufpTransform.RotateAround(_playerTransformObj.position, FPVector.up, FrameSyncManager.DeltaTime * RockerSpeed);
            _ufpTransform.UpdateRotationAndPosition();
            //Debug.Log("_ufpTransform:::" + _ufpTransform.forward.ToVector());
            //transform.RotateAround(_childObj.position, Vector3.up, Time.deltaTime * RockerSpeed);
            return;
        }
        else if (_dragging && _directorToRight && !_bTouchMouse) //右遥感
        {
            _rockerControl = ComMoveController.RockerControl.RightControl;
            _ufpTransform.RotateAround(_playerTransformObj.position, FPVector.up, FrameSyncManager.DeltaTime * -RockerSpeed);
            _ufpTransform.UpdateRotationAndPosition();

            // transform.RotateAround(_childObj.position, Vector3.up, Time.deltaTime * -RockerSpeed);
            return;
        }

    }

    #endregion

    #region 2)============= GUI控制 ============= 
    //private void OnGUI()
    //{
    //    if (Target == null || !_hasCamera) return;
    //    if (_bTouchMouse) return;

    //    if (TouchRect.width == 0)
    //    {
    //        TouchRect = EasyJoystick.GetTouchRect();
    //    }

    //    if (UnityEngine.Event.current.type == EventType.MouseDown)
    //    {
    //        if (EventSystem.current.IsPointerOverGameObject())
    //        {
    //            _bVaildDrag = false;
    //           // _dragging = false;
    //            return;
    //        }
    //        //记录鼠标按下的位置    
    //        _firstPoint = UnityEngine.Event.current.mousePosition;

    //        if ((TouchRect.width != 0) && !TouchRect.Contains(_firstPoint))
    //        {
    //            _bVaildDrag = true;
    //            //  Debug.Log("_bVaildDrag__bVaildDrag__bVaildDrag!!::"+ TouchRect + ",,_firstPoint::"+ _firstPoint);
    //        }
    //        else
    //        {
    //            _bVaildDrag = false;
    //        }
    //        //  Debug.Log("_firstPoint::" + _firstPoint + ",_bVaildDrag::" + _bVaildDrag);
    //     //   _dragging = false;
    //    }
    //    else if (UnityEngine.Event.current.type == EventType.MouseDrag)
    //    {
    //        if (!_dragging && EventSystem.current.IsPointerOverGameObject())
    //        {
    //            return;
    //        }

    //        //记录鼠标拖动的位置    
    //        if (!_bVaildDrag)
    //        {
    //            _firstPoint = _secondPoint;
    //            return;
    //        }

    //        //    Debug.Log("_bVaildDrag::" + _bVaildDrag);
    //       // _dragging = _bVaildDrag;

    //        _secondPoint = UnityEngine.Event.current.mousePosition;
    //        Vector2 slideDirection = _secondPoint - _firstPoint;
    //        float x = slideDirection.x, y = slideDirection.y;

    //        if (y < x && y > -x) // right         
    //        {
    //            //_directorToLeft = false;
    //            //_directorToRight = true;
    //        }
    //        else if (y > x && y < -x)// left  
    //        {
    //            //_directorToLeft = true;
    //            //_directorToRight = false;
    //        }
    //        else
    //        {
    //            //_directorToLeft = false;
    //            //_directorToRight = false;
    //        }

    //       // _bBeforeDragging = _dragging;
    //        //if (!TouchRect.Contains(_firstPoint)  && _bVaildDrag)
    //        //{
    //        //    _bBeforeDragging = true;
    //        //}


    //        _firstPoint = _secondPoint;
    //    }
    //    else if (UnityEngine.Event.current.type == EventType.MouseUp)
    //    {
    //        //Debug.Log("Up_up_bVaildDrag::"+ _bVaildDrag);
    //     //   _dragging = false;
    //        _bVaildDrag = false;
    //    }
    //    // Debug.Log("Event.current.type::" + Event.current.type);
    //}

    #endregion

    #region 3)============= 设置对象旋转角度、forward等相关计算操作 ============= 

    /// <summary>
    /// 设置目标对象
    /// </summary>
    /// <param name="targetObj"></param>
    public void SetTarget(Transform targetObj)
    {
        if (Target == null)
        {
            Target = targetObj;
            _motionObj = Target.GetComponent<CharacterMotion>();
            _avatarObj = Target.GetComponent<Avatar>();
            _childTransformObj  = Target.transform.Find(PlayerCommonName.ChildName).GetComponent<U_FPTransform>();
            _playerTransformObj = Target.GetComponent<U_FPTransform>();
            _rayPointObj        = _childTransformObj.transform.Find("RayPoint");
            Debug.Log("SetTarget is success!!");
        }
    }

    /// <summary>
    /// 设置摄像机对象
    /// </summary>
    public void SetCameraObj()
    {
        Camera.main.transform.SetParent(gameObject.transform);
        Camera.main.transform.position = gameObject.transform.position;
        Camera.main.transform.rotation = gameObject.transform.rotation;
    }

    /// <summary>
    /// 恢复对象的朝向，恢复angle数值
    /// </summary>
    public void ResetCameraForward()
    {
        _currentAngle = 0;
    }
    #endregion

    #region 4)============= 帧同步相关函数操作 ============= 
    public void JoyStickAngleChangeEvent(FP angle)
    {
        _bTouchMouse = true;
        _currentAngle = angle - 90;
    }

    #endregion

    #region  5) =========定点数、物理引擎等相关函数=========

    public FPVector GetForword()
    {
        return _ufpTransform.forward;
    }

    public FPQuaternion GetRotation()
    {
        return _ufpTransform.rotation;
    }

    public U_FPTransform GetFPTransform()
    {
        return _ufpTransform;
    }

   
    #endregion

   

}
