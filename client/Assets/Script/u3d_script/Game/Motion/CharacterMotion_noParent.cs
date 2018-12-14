using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CharacterMotion_noParent : MonoBehaviour
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

    public float MoveSpeed = 2f;                   //移动速度
    public float RotationSpeed = 15f;              //旋转速度
    public float Radius = 5;                       //设置走圆方式下，圆的半径
    public float SlopeAngle = 45f;                 //设置最高坡度值

    public float GroundRayLength = 0.9f;           //射线长度
    public float BarrierLimit = 0.75f;             //射线向前检测长度
    public Transform RayPointObj;                  //设置一个射线点
    public Transform Pos;                          //为了后期方便查看圆心的点，扔个物体点进来作为圆心点

    public E_MotionMode  MotionMode;                //设置行走方式
    public E_SurfaceMode SurfaceMode;               //设置跨面模式
    public E_ForWardMode ForWardMode;                //设置Forward贴面模式

    private Vector3 _center;                       //设置走圆方式下，圆的真正中心点
    public bool bMoveFlag = false;                 //是否处于移动状态 
               
    public bool bOnGround                           //检测是否在地面
    {
        get
        {
            return Physics.Raycast(RayPointObj.position, -transform.up, GroundRayLength);
        }
    }

    private float g = 9.81f;                        //重力加速度
    private float _verCurSpeed;                     //增加的惯力速度
    private void Awake()
    {
      //  _center = Pos.position;   //设置物体的center数值
        _verCurSpeed = MoveSpeed; //设置为移动速度一致
    }

    public void Update()
    {
      
        RaycastHit _groundHit;
        RaycastHit _hitInfo;
        //bool data = RayGroundInfo(out _groundHit);
        //bool ss = RayBarrierInfo(transform.forward, out _hitInfo);
        //if (!data && !ss)//空中调整角度
        if (!RayGroundInfo(out _groundHit) && !RayBarrierInfo(transform.forward, out _hitInfo))//空中调整角度
        {

            //if (Physics.Raycast(RayPointObj.transform.position, -Vector3.up, out _groundHit, GroundRayLength)) //悬空
            //{
            //    Debug.Log("11111111111111111111111111111111111111111");
            //    Vector3 forward = transform.forward;
            //    float angle = Vector3.Angle(transform.forward, _groundHit.normal);

            //    Vector3 normal11 = Vector3.Cross(transform.forward, _groundHit.normal);
            //    angle *= Mathf.Sign(Vector3.Dot(normal11, transform.up));
            //    forward = Quaternion.AngleAxis(angle, -_groundHit.normal) * -transform.up;

            //    //  Debug.Log("angel:::" + angle + "forward::" + forward + ":normal::"+ normal+ ":fffQuaternion.AngleAxis(angle, normal)::"+ Quaternion.AngleAxis(angle, normal)) ;
            //    FitSurface(forward, _groundHit.point, _groundHit.normal);

            //    return;

            //}

         //   Debug.Log("transform.up::" + transform.up +",ground::"+ data +",barrier::"+ss);
            _verCurSpeed = _verCurSpeed - g * Time.fixedDeltaTime; //垂直上的初速度随时间的改变
            transform.Translate(transform.forward * MoveSpeed * Time.fixedDeltaTime, Space.World);//水平上的运动
            transform.Translate(Vector3.up * _verCurSpeed * Time.fixedDeltaTime, Space.World);//垂直上的运动

            float angleForward = Vector3.Angle(transform.up, Vector3.up);
            if (angleForward == 0) return;

            Vector3 normal = Vector3.Cross(transform.up, Vector3.up);
            angleForward *= Mathf.Sign(Vector3.Dot(normal, transform.up));
            Vector3 forwardVec3 = Quaternion.AngleAxis(angleForward, normal) * transform.up;
            Vector3 forwardForward = Quaternion.AngleAxis(angleForward, normal) * transform.forward;

            Quaternion qur = Quaternion.LookRotation(forwardForward, forwardVec3);
            transform.rotation = Quaternion.Slerp(transform.rotation, qur, 0.1f);
           // Debug.Log("transform.rotation::" + transform.rotation + ",,,transform.location:::" + transform.position + ",,,forwardForward::" + forwardForward + ",,,forwardVec3::" + forwardVec3);
        }
        else
        {
            float angle = Vector3.Angle(Vector3.up, _groundHit.normal);
            if (angle > SlopeAngle)
            {
                if (!bMoveFlag)
                {
                    Vector3 V = Vector3.zero;
                    float offsetAngle = Vector3.Angle(Vector3.up, _groundHit.normal);
                    float tempData = offsetAngle;
                    V.y = V.y - (g * Mathf.Sin(offsetAngle * Mathf.Deg2Rad)) * Time.deltaTime;
                    V.z = V.z - (g * Mathf.Cos(offsetAngle * Mathf.Deg2Rad)) * Time.deltaTime;
                    transform.position += V;
                }
            }
            else
            {
                transform.position = new Vector3(transform.position.x, _groundHit.point.y, transform.position.z);
            }

        }

    }

    public void Move()
    {
        if (!bOnGround)
        {
           bMoveFlag = false;
           return;  //如果离地,直接返回
        }
        if(!bMoveFlag) bMoveFlag = true;
        Vector3 forward = transform.forward;

        RaycastHit hitInfo;
        bool bExitBarrier = (RayBarrierInfo(transform.forward, out hitInfo)); //表示前方检测到物体,暂时把检测到的物体都称之为障碍物
        bool bSlope = false; //默认不是为斜坡标志 
        if (bExitBarrier)
        {
            bSlope = (Vector3.Angle(Vector3.up, hitInfo.normal) > SlopeAngle) ? true : false;
            
            if (ForWardMode == E_ForWardMode.FM_ON_FORWARD)
            {
                forward = transform.forward;
            }
            else
            {
                // //Vector3 dd = forward;
                // // float angle = 90 - Vector3.Angle(transform.up, hitInfo.normal);
                // // angle *= Vector3.Cross(transform.forward, hitInfo.normal).y < 0 ? -1 : 1;
                // //forward = Quaternion.AngleAxis(angle, transform.right) * transform.up;
                // //Debug.Log("transform.forward::"+transform.forward);
                // Vector3 beforeForward = forward;
                // float angle = 90 - Vector3.Angle(Vector3.up, hitInfo.normal);
                // Vector3 CrossData = Vector3.Cross(Vector3.up, hitInfo.normal);
                // Vector3 WallNormal = Quaternion.AngleAxis(Mathf.Abs(angle), CrossData) * hitInfo.normal;
                // //     Debug.Log("Angle:::" + angle+ ",WallNornal::"+ WallNormal+ ",transform.forward::" + transform.forward);
                // // forward = Quaternion.AngleAxis(angle, normal) * transform.up;
                //// Debug.Log("WallNormal:::"+ WallNormal);
                ///* float*/ angle = 180 - Vector3.Angle(transform.forward, WallNormal);
                // Vector3 normal = Vector3.Cross(transform.forward, WallNormal).y < 0 ? -WallNormal : WallNormal;
                // forward = Quaternion.AngleAxis(angle, normal) * transform.up;


                //  angle = 90 - Vector3.Angle(transform.up, hitInfo.normal);
                //  angle *= Vector3.Cross(transform.forward, hitInfo.normal).y < 0 ? -1 : 1;
                //  forward = Quaternion.AngleAxis(angle, transform.right) * forward;
                // Debug.DrawRay(transform.position, forward * 3f, Color.yellow);
                // Debug.Log("forward::"+ forward + ",before::"+ beforeForward);
                // //  angle *= Vector3.Cross(transform.forward, hitInfo.normal).y < 0 ? -1 : 1;
                // //  forward = Quaternion.AngleAxis(angle, transform.right) * forward;

                // //  Debug.Log("forward:::"+ forward +":angle::"+ angle + "Vector3.Cross(transform.forward, WallNormal)::"+ Vector3.Cross(transform.forward, WallNormal));

                float angle = 90 - Vector3.Angle(Vector3.up, hitInfo.normal);
                Vector3 CrossData = Vector3.Cross(Vector3.up, hitInfo.normal);
                forward = Quaternion.AngleAxis(angle, transform.right) * transform.up;
                Vector3 WallNormal = Quaternion.AngleAxis(Mathf.Abs(angle), CrossData) * hitInfo.normal;
                //     Debug.Log("Angle:::" + angle+ ",WallNornal::"+ WallNormal+ ",transform.forward::" + transform.forward);
                // forward = Quaternion.AngleAxis(angle, normal) * transform.up;
                // Debug.Log("WallNormal:::"+ WallNormal);
                angle = 180 - Vector3.Angle(transform.forward, WallNormal);
                Vector3 normal = Vector3.Cross(transform.forward, WallNormal).y < 0 ? -WallNormal : WallNormal;
                forward = Quaternion.AngleAxis(angle, normal) * forward;

                Debug.Log("dddddddddddddddddddddddddddddddd::"+ angle);
                //Vector3 climbForward = Vector3.zero;
                //float tempAngle = Vector3.Angle(Vector3.up, hitInfo.normal);
                //float angle = 180 - Vector3.Angle(transform.forward, hitInfo.normal) - tempAngle;
                //Vector3 normal = Vector3.Cross(transform.forward, hitInfo.normal).y < 0 ? -hitInfo.normal : hitInfo.normal;
                //forward = Quaternion.AngleAxis(angle, normal) * transform.up;
                //Debug.Log("tempAngle::"+ tempAngle + ",angle::"+ angle + ",Vector3.Angle(transform.forward, hitInfo.normal)::"+ Vector3.Angle(transform.forward, hitInfo.normal));
            }
        }
        else
        {
            RayGroundInfo(out hitInfo);
            bSlope = (Vector3.Angle(Vector3.up, hitInfo.normal) > SlopeAngle) ? true : false;
        }

        if (SurfaceMode == E_SurfaceMode.SM_ON_GROUND)   //非跨面模式：供人型和乌贼模式下使用
        {
            if (bExitBarrier && bSlope) return;      
        }
        else if(SurfaceMode == E_SurfaceMode.SM_ON_SURFACE) //跨面模式：供乌贼下潜模式下使用
        {
            if (bSlope) MotionMode = E_MotionMode.MM_ON_LINE;
        }

        if (hitInfo.transform != null && ForWardMode != E_ForWardMode.FM_ON_FORWARD)
        {
            FitSurface(forward, hitInfo.point, hitInfo.normal);  //调整贴面
        }
        else
        {
           // Debug.Log("hitInfo.transform is null");
        }


        if (MotionMode == E_MotionMode.MM_ON_ROUND) //画圆模式：按照圆的弧度来行走
        {
            transform.RotateAround(_center, transform.up, RotationSpeed * MoveSpeed * Time.deltaTime);
        }
        else if (MotionMode == E_MotionMode.MM_ON_LINE)//直走模式：按照其forward的方向行走
        {
            transform.Translate(transform.forward * MoveSpeed * Time.deltaTime, Space.World);
        }

        _verCurSpeed = MoveSpeed;
    }

    public void Idle()
    {
        if(bMoveFlag) bMoveFlag = false;
    }
    public bool RayGroundInfo(out RaycastHit _groundHit)  //检测是否在地面
    {
        if (Physics.Raycast(RayPointObj.position, -transform.up, out _groundHit, GroundRayLength))
        {
            Debug.DrawRay(RayPointObj.position, -transform.up * GroundRayLength, Color.green);
            return true;
        }
        return false;
      //  return Physics.Raycast(RayPointObj.position, -transform.up,out _groundHit, GroundRayLength);
    }

    /// <summary>
    /// 检测前方是否有物体，这里暂时把检测的物体都统一叫为障碍物
    /// </summary>
    /// <param name="forward"></param>
    /// <param name="_barrierHit"></param>
    /// <returns></returns>
    public bool RayBarrierInfo(Vector3 forward ,out RaycastHit _barrierHit)
    {
        var barrierVec3 = RayPointObj.transform.position + (-RayPointObj.transform.up * BarrierLimit);
        // return (Physics.Raycast(barrierVec3, forward, out _barrierHit, 1f));
        if (Physics.Raycast(barrierVec3, forward, out _barrierHit, 1f)) {
            Debug.DrawRay(barrierVec3, forward * 1f, Color.red);
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
    private void FitSurface(Vector3 forward, Vector3 position, Vector3 normal)
    {
        Vector3 left = Vector3.Cross(forward, normal);   //切线
        Vector3 newForward = Vector3.Cross(normal, left);
        if (normal == Vector3.zero) {
          Debug.Log("forward:::"+ forward);
            return;
        }
        Quaternion newRotation = Quaternion.LookRotation(newForward, normal);
       // Debug.Log("newForward::"+ newForward+ ",normal::"+ normal + ",,forward::"+ forward);
        transform.rotation = newRotation;
        transform.position = position;
    }


    private void FitSurface(Vector3 forward, ref RaycastHit hitInfo)
    {  //调整贴面

        Vector3 left = Vector3.Cross(forward, hitInfo.normal);   //切线
        Vector3 newForward = Vector3.Cross(hitInfo.normal, left);
        if (hitInfo.normal == Vector3.zero)
        {
            Debug.Log("ssssssssssssssssssssssssssss::::forward:::" + forward + ",hitInfo.normal::"+ hitInfo.normal+",hit.name::"+hitInfo.transform.name);
            return;
        }
        Quaternion newRotation = Quaternion.LookRotation(newForward, hitInfo.normal);
       // Debug.Log("newForward::" + newForward + ",normal::" + hitInfo.normal);
        transform.rotation = newRotation;
        transform.position = hitInfo.point;
    }
    public Vector3 GetCenterPoint()
    {
        return _center;
    }
    public void SetCenterPoint(Vector3 value)
    {
        _center = value;
    }

    private void OnCollisionEnter(Collision collision)
    {
     //   Debug.Log("sssssssssssssssssssssssssssssssssssssssssssssssssssssssss::" + ",transform.up != Vector3.up::" + (transform.up != Vector3.up));
        if (!bMoveFlag && transform.up != Vector3.up)
        {
       //     Debug.Log("ddddddddddddddddddddddddddddd");
            RaycastHit _groundHit;
            if (Physics.Raycast(RayPointObj.transform.position, -Vector3.up, out _groundHit, GroundRayLength)) //悬空
            {

                if (Vector3.Angle(Vector3.up, _groundHit.normal) > SlopeAngle) return;
             Debug.Log("11111111111111111111111111111111111111111::"+ Vector3.Angle(Vector3.up, _groundHit.normal) + ",data::"+ _groundHit.transform.name);
                Vector3 forward = transform.forward;
                float angle = Vector3.Angle(transform.forward, _groundHit.normal);

                Vector3 normal = Vector3.Cross(transform.forward, _groundHit.normal);
                angle *= Mathf.Sign(Vector3.Dot(normal, transform.up));
                forward = Quaternion.AngleAxis(angle, -_groundHit.normal) * -transform.up;

            //    Debug.Log("transform.forward::" + transform.forward);
                //  Debug.Log("angel:::" + angle + "forward::" + forward + ":normal::"+ normal+ ":fffQuaternion.AngleAxis(angle, normal)::"+ Quaternion.AngleAxis(angle, normal)) ;
                FitSurface(forward, _groundHit.point, _groundHit.normal);
            //    Debug.Log("transform.forward11111::" + transform.forward);
                return;

            }
        //    else if(Physics.Raycast(RayPointObj.transform.position+ new Vector3(0,4,0), -Vector3.up, out _groundHit, GroundRayLength +4f))
        //    {
        //        Debug.Log("22222222222222222222222222222222222222222222222222222::"+_groundHit.transform.name);
        //        Vector3 forward = transform.forward;
        //        float angle = Vector3.Angle(transform.forward, _groundHit.normal);

        //        Vector3 normal = Vector3.Cross(transform.forward, _groundHit.normal);
        //        angle *= Mathf.Sign(Vector3.Dot(normal, transform.up));
        //        forward = Quaternion.AngleAxis(angle, -_groundHit.normal) * -transform.up;

        //        //  Debug.Log("angel:::" + angle + "forward::" + forward + ":normal::"+ normal+ ":fffQuaternion.AngleAxis(angle, normal)::"+ Quaternion.AngleAxis(angle, normal)) ;
        //        FitSurface(forward, _groundHit.point, _groundHit.normal);

        //        return;
        //    }
        //}

    }
    //private void OnCollisionStay(Collision collision)
    //{
    //    if (!bMoveFlag && transform.up != Vector3.up)
    //    {
    //        Debug.Log("ddddddddddddddddddddddddddddd");
    //        RaycastHit _groundHit;
    //        if (Physics.Raycast(RayPointObj.transform.position, -Vector3.up, out _groundHit, GroundRayLength)) //悬空
    //        {
    //            Debug.Log("11111111111111111111111111111111111111111");
    //            Vector3 forward = transform.forward;
    //            float angle = Vector3.Angle(transform.forward, _groundHit.normal);

    //            Vector3 normal = Vector3.Cross(transform.forward, _groundHit.normal);
    //            angle *= Mathf.Sign(Vector3.Dot(normal, transform.up));
    //            forward = Quaternion.AngleAxis(angle, -_groundHit.normal) * -transform.up;

    //            //  Debug.Log("angel:::" + angle + "forward::" + forward + ":normal::"+ normal+ ":fffQuaternion.AngleAxis(angle, normal)::"+ Quaternion.AngleAxis(angle, normal)) ;
    //            FitSurface(forward, _groundHit.point, _groundHit.normal);

    //            return;

    //        }
    //    }
    }

    /// <summary>
    /// true:表示地面坡度超过预设值; false:表示地面坡度在坡度预设值之内
    /// </summary>
    /// <returns></returns>
    public bool JudgetGroundSlope()
    {
        RaycastHit hitInfo;
        if (RayGroundInfo(out hitInfo) && Vector3.Angle(Vector3.up, hitInfo.normal) > SlopeAngle) {
            return true;
        }
        return false;
    }

    public void ChangeRoundCenter(float _joystickAngle)
    {
        Vector3 rv = gameObject.transform.right;
        if (_joystickAngle > 90 && _joystickAngle < 270)
        {
            rv = -gameObject.transform.right;
        }

        float RvRate = 1; //这是为了根据角度值，求圆心
        if (_joystickAngle > 180)
        {
            RvRate += Mathf.Sin((_joystickAngle - 180) * Mathf.Deg2Rad);
        }
        else
        {
            RvRate += Mathf.Sin((_joystickAngle) * Mathf.Deg2Rad);
        }

        _center = rv * Radius * RvRate;
        _center = gameObject.transform.position + _center;

        RotationSpeed = Mathf.Abs(RotationSpeed);
        if (_joystickAngle > 90 && _joystickAngle < 270)
        {
            RotationSpeed = Mathf.Abs(RotationSpeed) * -1;
        }
    }



}



