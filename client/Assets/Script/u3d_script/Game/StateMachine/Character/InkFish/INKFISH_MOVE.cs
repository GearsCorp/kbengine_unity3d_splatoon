using CBFrame.Sys;
using KBEngine;
using UnityEngine;


public class INKFISH_MOVE : SMState{

    private Avatar _avatar;
    private CharacterMotion _characterMotionObj;

    public FP _joystickAngle = 90f;

    public INKFISH_MOVE(string name, GameObject go) :
        base(name, go) 
    {
        _avatar = gameObject.GetComponent<Avatar>();
        _characterMotionObj = gameObject.transform.GetComponent<CharacterMotion>();
    }

    public override void OnUpdate()
    {
        _joystickAngle = _avatar.JoystickAngle;

        //获取移动服务组件的数值,改变其属性
        if ((_joystickAngle >= 80 && _joystickAngle <= 100) ||
           (_joystickAngle >= 260 && _joystickAngle <= 280))  //表示直线行走
        {
            _characterMotionObj.MotionMode = CharacterMotion.E_MotionMode.MM_ON_LINE;
        }
        else //表示绕圆行走
        {
            _characterMotionObj.MotionMode = CharacterMotion.E_MotionMode.MM_ON_ROUND;
            _characterMotionObj.ChangeRoundCenter(_joystickAngle);
        }
        _characterMotionObj.SurfaceMode = CharacterMotion.E_SurfaceMode.SM_ON_GROUND /*SM_ON_SURFACE*/;   //非跨面模式
        _characterMotionObj.ForWardMode = CharacterMotion.E_ForWardMode.FM_ON_NORMAL;   //贴面时当前面法线的朝向模式
        _characterMotionObj.Move();

    }

   
}
