using CBFrame.Core;
using CBFrame.Sys;
using GameLogic.Configs;
using GameLogic.Events;
using KBEngine;
using UnityEngine;
using UnityEngine.EventSystems;

public class LocalController : FrameSyncBehaviour
{
    //左遥感参数
    private FP _positionX = 0;
    private FP _positionY = 0;
    // private float _curAngle = 90;                 //当前角度
    private FP _triggerAngle = 90;              //遥感触发的角度
    private bool _bTouchMouse     = false;           //是否触碰摇杆
    private bool _bBeforeDragging = false;           //标志之前是否有划过屏幕，调节镜头
    private bool _dragging        = false;           //标记是否鼠标在滑动 
    private Vector2 _firstPoint   = Vector2.zero;    //记录鼠标点击的初始位置  
    private Vector2 _secondPoint  = Vector2.zero;    //记录鼠标移动时的位置  
    private bool _bVaildDrag      = false;           //有效的拖拽/排除标题栏拖动事件信息触发的消息
    private bool _directorToLeft  = false;
    private bool _directorToRight = false;
    private Avatar _avatarObj = null;
    public ComMoveController ComMoveControllerObj;          //主追随目标
    public Rect TouchRect;



    //对应帧同步
    private bool _fpIdleFlag           = false;        //设置标志玩家静止的标志
    private int  _fpModelChangeState   = 0;            //玩家当前改变模型的Id
    private bool _fpShootDownFlag      = false;        //射击按下标志
    private bool _fpShootUpFlag        = false;        //射击抬起标志
    private bool _fpJumpFlag           = false;        //跳跃标志
    private bool _fpThrowGrenadeFlag   = false;        //扔手雷标志
    private bool _fpReplaceWeapon      = false;        //替换武器
    private bool _fpDragFlag           = false;        //替换武器
    private FPVector _fpChangeAvatarForward = FPVector.zero;    //改变avatar的移动方向

    private bool _fpDragEndFlag         = false;
    private void Awake()
    {
        ////test....test//
        //_avatarObj = gameObject.transform.Find(PlayerCommonName.PlayerName).GetComponent<Avatar>();
        //ComMoveControllerObj = gameObject.transform.Find(PlayerCommonName.ComponentPoint).GetComponent<ComMoveController>();

        CBGlobalEventDispatcher.Instance.AddEventListener<int>(ButtonEventId.ButtonClickEvent, ButtonClickEvent);
        CBGlobalEventDispatcher.Instance.AddEventListener<int>(ButtonEventId.ButtonDownEvent, ButtonDownEvent);
        CBGlobalEventDispatcher.Instance.AddEventListener<int>(ButtonEventId.ButtonPressEvent, ButtonPressEvent);
        CBGlobalEventDispatcher.Instance.AddEventListener<int>(ButtonEventId.ButtonUpEvent, ButtonUpEvent);
    }


    void installEvents()
    {
       

    }

    public void SetTarget(Transform targetObj)
    {
        if (_avatarObj == null)
        {
            _avatarObj = gameObject.transform.Find(PlayerCommonName.PlayerName).GetComponent<Avatar>();
            ComMoveControllerObj = gameObject.transform.Find(PlayerCommonName.ComponentPoint).GetComponent<ComMoveController>();

            CBGlobalEventDispatcher.Instance.AddEventListener<int>(ButtonEventId.ButtonClickEvent, ButtonClickEvent);
            CBGlobalEventDispatcher.Instance.AddEventListener<int>(ButtonEventId.ButtonDownEvent, ButtonDownEvent);
            CBGlobalEventDispatcher.Instance.AddEventListener<int>(ButtonEventId.ButtonPressEvent, ButtonPressEvent);
            CBGlobalEventDispatcher.Instance.AddEventListener<int>(ButtonEventId.ButtonUpEvent, ButtonUpEvent);
        }
    }

    void OnEnable()
    {
        EasyJoystick.On_JoystickMove += OnJoystickMove;
        EasyJoystick.On_JoystickTouchStart += OnJoyStickBegin;
        EasyJoystick.On_JoystickTouchUp += OnJoyStickUp;
    }
    void OnDisable()
    {
        //撤销事件  
        EasyJoystick.On_JoystickMove -= OnJoystickMove;
        EasyJoystick.On_JoystickTouchStart -= OnJoyStickBegin;
        EasyJoystick.On_JoystickTouchUp -= OnJoyStickUp;
    }

    void OnJoyStickBegin(MovingJoystick move)
    {
        if (_avatarObj == null) return;

        TouchRect = EasyJoystick.GetTouchRect();
        _bTouchMouse = true;
      
    }

    void OnJoyStickUp(MovingJoystick move)
    {
        if (_avatarObj == null) return;

        _positionX = 0;
        _positionY = 0;
        _secondPoint = move.joystick.JoystickTouch;
        _bTouchMouse = false;
        _fpIdleFlag  = true;
       
    }


    /// <summary>
    /// 摇杆操作
    /// </summary>
    /// <param name="move"></param>
    void OnJoystickMove(MovingJoystick move)
    {
        if (_avatarObj == null) return;
        if (move.joystickName != "New joystick") return;

        _positionX = move.joystickAxis.x;       //   获取摇杆偏移摇杆中心的x坐标
        _positionY = move.joystickAxis.y;      //    获取摇杆偏移摇杆中心的y坐标
        if (_bBeforeDragging)
        {
            _bBeforeDragging = false;
            //   _avatarObj.ChangeAvatarForward(ComMoveControllerObj.GetFPTransform().forward);
            _fpChangeAvatarForward = ComMoveControllerObj.GetFPTransform().forward;
            _fpDragFlag = true;
            Debug.Log("_bBeforeDragging_bBeforeDragging_bBeforeDragging::" + _fpChangeAvatarForward.ToVector());
        }

        FrameSyncInput.SetFP((byte)E_InputId.E_MOVE_X, _positionX);
        FrameSyncInput.SetFP((byte)E_InputId.E_MOVE_Y, _positionY);
    }

    void  Update()
    {
        if (_avatarObj == null) return;
        //  if (_avatarObj.AvatarState == Avatar.E_AvatarState.AS_DEAD) return;
        if (Input.GetKeyDown(KeyCode.P))      //人形
        {
            _fpModelChangeState = (int)KeyCode.P;
        }
        else if (Input.GetKeyDown(KeyCode.F))  //乌贼
        {
            _fpModelChangeState = (int)KeyCode.F;
        }
        else if (Input.GetKeyDown(KeyCode.D))  //下潜模式
        {
            _fpModelChangeState = (int)KeyCode.D;
        }
        else if (Input.GetKeyDown(KeyCode.H))   //换枪
        {
            GameManager.Instance.ChangeCurrentWeapon();
            _fpReplaceWeapon = true;
        }

    }


    private void OnGUI()
    {
        if (_avatarObj == null) return;
        if (_bTouchMouse) return;

        if (UnityEngine.Event.current.type == EventType.MouseDown)
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                _bVaildDrag = false;
                _dragging = false;
                _fpDragEndFlag = true;
                return;
            }
            _firstPoint = UnityEngine.Event.current.mousePosition;
            //记录鼠标按下的位置 
            if (!TouchRect.Contains(_firstPoint))
            {
                _bVaildDrag = true;
            }
            else
            {
                _bVaildDrag = false;
            }
            _dragging = false;
            _fpDragEndFlag = true;
        }

        if (UnityEngine.Event.current.type == EventType.MouseDrag)
        {
            if (!_dragging && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            //记录鼠标拖动的位置    
            if (!_bVaildDrag)
            {
                _firstPoint = _secondPoint;
                return;
            }

            _dragging = _bVaildDrag;

            _secondPoint = UnityEngine.Event.current.mousePosition;
            Vector2 slideDirection = _secondPoint - _firstPoint;
            float x = slideDirection.x, y = slideDirection.y;

            if (y < x && y > -x) // right         
            {
                _directorToLeft  = false;
                _directorToRight = true;
            }
            else if (y > x && y < -x)// left  
            {
                _directorToLeft = true;
                _directorToRight = false;
            }
            else
            {
                _directorToLeft  = false;
                _directorToRight = false;
            }

            _bBeforeDragging = _dragging;

            _firstPoint = _secondPoint;
        }
        else if (UnityEngine.Event.current.type == EventType.MouseUp)
        {
            _dragging      = false;
            _bVaildDrag    = false;
            _fpDragEndFlag = true;
        }
    }

    private void ButtonClickEvent(int id)
    {
        //if (id == ButtonEventId.ShootButtonEventId)
        //{
        //    _avatarObj.Shoot();
        //}
    }
    private void ButtonDownEvent(int id)
    {
        if (_avatarObj == null) return;
        if (id == ButtonEventId.ShootButtonEventId)
        {
            _fpShootDownFlag = true;

            _fpChangeAvatarForward = ComMoveControllerObj.GetFPTransform().forward;
            _fpDragFlag = true;

             //   FrameSyncInput.SetFPVector((byte)E_InputId.E_DRAGGING_CAMERA, ComMoveControllerObj.GetFPTransform().forward);
        }
        else if (id == ButtonEventId.JumpButtonEventId)
        {
            _fpJumpFlag = true;
        }
        else if (id == ButtonEventId.GrenadeButtonEventId)
        {
            _fpThrowGrenadeFlag = true;
        }
    }
    private void ButtonPressEvent(int id)
    {
        if (_avatarObj == null) return;
        if (id == ButtonEventId.ShootButtonEventId)
        {
            _fpShootDownFlag = true;
            //   FrameSyncInput.SetFPVector((byte)E_InputId.E_DRAGGING_CAMERA, ComMoveControllerObj.GetFPTransform().forward);
        }
    }

    private void ButtonUpEvent(int id)
    {
        if (_avatarObj == null) return;
        if (id == ButtonEventId.ShootButtonEventId)
        {
            _fpShootUpFlag   = true;
            _fpShootDownFlag = false;
        }
    }


    public override void OnSyncedInput()
    {
        if (_dragging)//拖拽中
        {  
            FrameSyncInput.SetBool((byte)E_InputId.E_MOUSE_DRAGGING, _dragging);
            FrameSyncInput.SetBool((byte)E_InputId.E_MOUSE_DRAG_LEFT,  _directorToLeft);
            FrameSyncInput.SetBool((byte)E_InputId.E_MOUSE_DRAG_RIGHT, _directorToRight);
        }
        if (_fpDragEndFlag) //拖拽结束
        {
            FrameSyncInput.SetBool((byte)E_InputId.E_MOUSE_DRAGEND, _fpDragEndFlag);
            FrameSyncInput.SetBool((byte)E_InputId.E_MOUSE_DRAG_LEFT,  false);
            FrameSyncInput.SetBool((byte)E_InputId.E_MOUSE_DRAG_RIGHT, false);
        }  
            
        //如果之前有摄像头拖拽,传递数值
        if (_fpDragFlag)
        {
            FrameSyncInput.SetFPVector((byte)E_InputId.E_DRAGGING_CAMERA, _fpChangeAvatarForward);
         //   Debug.Log("Input_Input_Input:::DraggingDraggingDraggingDragging + " + _fpChangeAvatarForward.ToVector() + ", datax::" + _positionX + " , _datay::" + _positionY);

        }

        //移动
        if (_positionX != 0 || _positionY != 0){
            FrameSyncInput.SetFP((byte)E_InputId.E_MOVE_X, _positionX);
            FrameSyncInput.SetFP((byte)E_InputId.E_MOVE_Y, _positionY);
           // Debug.Log("FP_FP_FP_::move_yyyyyyyyyy::" + _positionX.AsFloat() + ",move_y::" + _positionY.AsFloat());
        }
        //暂停
        if (_fpIdleFlag)
        {
            FrameSyncInput.SetBool((byte)E_InputId.E_IDEL, _fpIdleFlag);
           // Debug.Log("Input_Input_Input:::IdelIdelIdelIdelIdelIdelIdelIdel");
        }
            
        //射击
        if(_fpShootDownFlag)
            FrameSyncInput.SetBool((byte)E_InputId.E_SHOOT_DOWN, _fpShootDownFlag);
        if(_fpShootUpFlag)
            FrameSyncInput.SetBool((byte)E_InputId.E_SHOOT_UP, _fpShootUpFlag);

        //跳跃
        if (_fpJumpFlag)
        {
            FrameSyncInput.SetBool((byte)E_InputId.E_JUMP, _fpJumpFlag);
          //  Debug.Log("Input_Input_Input:::JUMPJUMPJUMPJUMPJUMPJUMPJUMP");
        }
           
        
        //改变模型格局
        if (_fpModelChangeState > 0)
            FrameSyncInput.SetInt((byte)E_InputId.E_CHANGE_MODEL,  _fpModelChangeState);
        
        //更换装备
        if(_fpReplaceWeapon)
            FrameSyncInput.SetInt((byte)E_InputId.E_REPLACE_WEAPON, GameManager.Instance.CurWeapon);
        
        //扔手雷
        if(_fpThrowGrenadeFlag)
            FrameSyncInput.SetBool((byte)E_InputId.E_THROW_GRENADE,  _fpThrowGrenadeFlag);

       
           

        //FrameSyncInput.SetFPVector((byte)E_InputId.E_SHOOT_FORWARD, _fpBeforeShootForward);
        //FrameSyncInput.SetFPVector((byte)E_InputId.E_CHANGE_AVATAR_FORWARD, _fpChangeAvatarForward);


        //重置数值
        _fpIdleFlag = false;
        _fpModelChangeState = 0;
        _fpJumpFlag         = false;
        _fpReplaceWeapon    = false;
        _fpThrowGrenadeFlag = false;
        _fpShootUpFlag      = false;
        _fpDragFlag         = false;
        _fpDragEndFlag      = false;

    }

    //public override void OnSyncedUpdate()
    //{
    //    //移动处理
    //    FP move_x = FrameSyncInput.GetFP((byte)E_InputId.E_MOVE_X);
    //    FP move_y = FrameSyncInput.GetFP((byte)E_InputId.E_MOVE_Y);
    //    Debug.Log("FP_FP_FP_::move_x_xxxxxxxx::" + move_x + ",move_y::" + move_y);
    //    if (move_x != 0 || move_y != 0)
    //    {
    //        SycnMove(move_x, move_y);
    //    }
    //    else if (FrameSyncInput.GetBool((byte)E_InputId.E_IDEL))
    //    {
    //        _avatarObj.Idle();
    //    }

    //    //射击处理
    //    bool shootDownFlag = FrameSyncInput.GetBool((byte)E_InputId.E_SHOOT_DOWN);
    //    if (shootDownFlag)
    //    {
    //        FPVector shootVec = FrameSyncInput.GetFPVector((byte)E_InputId.E_CHANGE_AVATAR_FORWARD);
    //        _avatarObj.Shoot(shootVec);
    //    }
    //    bool shootUpFlag = FrameSyncInput.GetBool((byte)E_InputId.E_SHOOT_UP);
    //    if (shootUpFlag) _avatarObj.Shoot(FPVector.zero, false);

    //    //跳跃处理
    //    bool jumpFlag = FrameSyncInput.GetBool((byte)E_InputId.E_JUMP);
    //    if (jumpFlag) _avatarObj.Jump();

    //    //模型改变处理
    //    int modelRender = FrameSyncInput.GetInt((byte)E_InputId.E_CHANGE_MODEL);
    //    if (modelRender > 0)
    //    {
    //        if (modelRender == (int)KeyCode.P)      //人形
    //            _avatarObj.ChangeAvatarForm(Avatar.E_AvatarForm.PERSON_STATE);
    //        else if (modelRender == (int)KeyCode.F)  //乌贼
    //            _avatarObj.ChangeAvatarForm(Avatar.E_AvatarForm.INKFISH_STATE);
    //        else if (modelRender == (int)KeyCode.D)  //下潜模式
    //            _avatarObj.ChangeAvatarForm(Avatar.E_AvatarForm.INKFISHDIVE_STATE);
    //    }

    //    //更换装备
    //    bool replaceWeaponFlag = FrameSyncInput.GetBool((byte)E_InputId.E_REPLACE_WEAPON);
    //    if (replaceWeaponFlag) _avatarObj.ChangeWeapon();

    //    //扔手雷
    //    bool throwGrenadeFlag = FrameSyncInput.GetBool((byte)E_InputId.E_THROW_GRENADE);
    //    if(throwGrenadeFlag) _avatarObj.ThrowGrenade();
    //}

    //public void SycnMove(FP move_x, FP move_y)
    //{
    //    _triggerAngle = CalculaAngle(move_x, move_y);
    //    _avatarObj.Move(_triggerAngle);
    //    ComMoveControllerObj.JoyStickAngleChangeEvent(_triggerAngle);
    // //   Debug.Log("angleangleangleangle::" + _triggerAngle.AsFloat());
    //}

}
