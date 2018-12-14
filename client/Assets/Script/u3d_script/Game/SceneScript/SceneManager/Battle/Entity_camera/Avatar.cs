using CBFrame.Core;
using CBFrame.Sys;
using GameLogic.Configs;
using System;
using System.Collections;
using KBEngine;
using UnityEngine;

public class Avatar : CBComponent
{
    public enum E_AvatarState
    {
        AS_ALIVE,
        AS_DEAD
    }
    public enum E_AvatarForm
    {
        PERSON_STATE = 1,
        INKFISH_STATE = 2,
        INKFISHDIVE_STATE = 3,
    }
    public enum E_AvatarMotionState
    {
        AM_IDLE = 1,
        AM_MOVE = 2,
        AM_MOVEJUMP = 3,
        AM_JUMP = 4,
        AM_DIVE_LEAP = 5,
        //  AM_SHOOT = 5,
        // AM_MOVESHOOT = 6,
    }

    public enum E_AvatarBehavior
    {
        B_NONE = 0,
        B_SHOOT = 1,
        B_GRENADE = 2,
    }

    private int _hp;                     //属性
    public int HpMax = 100;


    public int ItemType;                //队伍类别


    public FP MoveSpeed = 4f;           //移动速度
    public FP NorMalMoveSpeed = 4f;
    public FP AddMoveSpeed = 8f;
    public FP SubMoveSpeed = 1f;

    private E_AvatarForm _beforeAvatarForm;  //之前人物的形态
    public E_AvatarForm AvatarForm;         //人物的形态
    public E_AvatarMotionState AvatarMotion;       //人物的移动状态
    public E_AvatarBehavior AvatarBehavior;     //人物的行爲
    public E_AvatarState AvatarState;        //人物的生命状态



    private int _motionStateCount;
    private bool _jumpFlag = false;
    private bool _bInAirtoLeap = false;

    //涉及遥感选项
    public FP JoystickAngle = 90f;
    public FP RotateAngel = 0;

    //gameObject上的对象
    private Transform _childObj;                   //PlayerChild对象
    private U_FPTransform _ufpTransformChildObj;   //PlayerChild对象的U_FPTransform组件
    private U_FPTransform _ufpTransformObj;        //Player对象的U_FPTransform组件

    private CharacterMotion _characterMotionObj;   //移动控制器
    private CBStateMachine _sm;                    //Avatar状态机
    private ArtAnimatorManager _artAnimatorManager;//Art管理
    public ComMoveController ComMoveFollowObj;   //公共的跟随对象

    //关于武器

    private int _energyValue = 100;              //存储的能量值
    public bool bEnergyShortageByShoot = false;  //射击的时能量短缺标志
    public WeaponCaster WeaponCasterObj;         //武器投掷者对象
    private int _restoreEnergySpeed;              //恢复消耗的能量

    public int RestoreEnergySpeed
    {
        get
        {
            return _restoreEnergySpeed;
        }

        set
        {
            _restoreEnergySpeed = value;
        }
    }

    private FP _speedRate = 1; //移动的速率
    public int AvatarEntityId = 0;
    public int Hp
    {
        get
        {
            return _hp;
        }

        set
        {
            _hp = value;
            if (_hp < 0) _hp = 0;
            if (0 == _hp)
                AvatarState = E_AvatarState.AS_DEAD;
        }
    }

    public int EnergyValue
    {
        get
        {
            return _energyValue;
        }

        set
        {
            _energyValue = value;
            if (_energyValue < 0) _energyValue = 0;
            else if (_energyValue > 100) _energyValue = 100;
        }
    }

    // Use this for initialization
    void Start()
    {
        Init();
        _sm = gameObject.GetComponent<CBStateMachine>();
        _artAnimatorManager = gameObject.GetComponent<ArtAnimatorManager>();
        PlayerStateChanged(E_AvatarMotionState.AM_IDLE);

        //补充弹药
        StartCoroutine(RestoreEnergy());
        AddEventListener(DefineEventId.PlayerInAirEvent, PlayerInAirEvent);

        //   testNum();
    }

    // Update is called once per frame
    public void Update()
    {
        if (AvatarState == E_AvatarState.AS_DEAD) return;

        //if (_childObj.eulerAngles.x > 0 || _childObj.eulerAngles.z > 0)
        //{
        //    //补充功能：如果掉落，需要重新拉回
        //    _childObj.eulerAngles = new Vector3(0, _childObj.eulerAngles.y, 0);
        //  //  _ufpTransformChildObj.rotation = FPQuaternion.Eule(0, _ufpTransformChildObj.eulerAngles.y, 0);
        //}
        RaycastHit info;
        if (PixelManager.GetInstance() != null && _characterMotionObj.RayGroundInfo(out info) &&
            (AvatarMotion == E_AvatarMotionState.AM_IDLE))
        {
            PixelManager.E_InColorType type = PixelManager.GetInstance().GetPixelsInfo(AvatarEntityId, info.textureCoord);
            AcquireRestoreEnergySpeed(type);

            //2)处于敌方颜料中血量的减少
            if (type == PixelManager.E_InColorType.IC_OTHER_COLOR)
            {
                Hp--;
            }
        }

        //  Debug.Log("Hp::::" + Hp);
    }

    public void OnDestroy()
    {
        Destroy(_childObj);
        Destroy(ComMoveFollowObj);
        Destroy(_characterMotionObj);
        Destroy(WeaponCasterObj);
        Resources.UnloadUnusedAssets();
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (_bInAirtoLeap)
        {
            RaycastHit hitInfo;
            if (!_characterMotionObj.RayGroundInfo(out hitInfo)) return;

            bool flag = _characterMotionObj.CanMoveByInk(ref hitInfo);
            Debug.Log("sssssssssssssssssss::" + hitInfo.transform.name + ",flag::" + flag);
            if (_sm && flag)
            {
                _sm.SetParameter("AvatarForm", (int)E_AvatarForm.INKFISHDIVE_STATE);
                PlayerStateChanged(E_AvatarForm.INKFISHDIVE_STATE);
                ChangeMeshRenderForm(AvatarForm);
            }

            _bInAirtoLeap = false;
        }
        else
        {

        }
    }


    #region 1) =========初始化函数及系统相关函数=========
    public void Init()
    {
        AvatarState = E_AvatarState.AS_ALIVE;
        AvatarMotion = E_AvatarMotionState.AM_IDLE;
        AvatarForm = E_AvatarForm.PERSON_STATE;
        _beforeAvatarForm = E_AvatarForm.PERSON_STATE;
        AvatarBehavior = E_AvatarBehavior.B_NONE;

        _motionStateCount = Enum.GetValues(typeof(E_AvatarMotionState)).Length;

        _childObj = gameObject.transform.Find(PlayerCommonName.ChildName);
        _ufpTransformObj = transform.GetComponent<U_FPTransform>();
        _ufpTransformChildObj = _childObj.GetComponent<U_FPTransform>();
        _characterMotionObj = transform.GetComponent<CharacterMotion>();

        _hp = HpMax;
        AutoRenderSplatData(AvatarEntityId);

        Debug.Log("AvatarId::" + AvatarEntityId + "  == > InitInitInitInitInitInitInitInitInitInit!!!");
    }


    #endregion

    #region 2) =========玩家行爲及移动状态等操作：如 move\jump、movejump、shoot等=========

    /// <summary>
    /// 
    /// </summary>
    /// <param name="speedRate"> 帧的速率，如快播、平速率播放等</param>
    /// <param name="Angle">移动的角度</param>
    public void Move(/*FP speedRate, */FP Angle)
    {
        //if (Angle > 100 && Angle < 260)
        //{
        //    Angle = 180;
        //}
        //else if (Angle > 280 || Angle < 80)
        //{
        //    Angle = 360;
        //}
        // _speedRate = speedRate;   //增加移动的速率，配合帧的播放速度
        RotateAngel = JoystickAngle - Angle;
        // Debug.Log("Angle::::" + Angle.AsFloat() + ",RotateAngel::"+ RotateAngel.AsFloat());
        JoystickAngle = Angle;
      


        if (RotateAngel != 0)
        {
            //选择物体角度，然后进行移动
            RotateChildByFP(0, RotateAngel, 0);
        }
        //Debug.Log("FP_FP_right:::" + _ufpTransformChildObj.right.ToVector() + ",,,right:::" + _ufpTransformChildObj.transform.right
        // + ",yAngle::" + RotateAngel + ",FP_forward::" + _ufpTransformChildObj.forward.ToVector() + ",_forward::" + _ufpTransformChildObj.transform.forward
        // + ",FP_Rotation" + _ufpTransformChildObj.rotation.ToQuaternion() + ",_Rotation" + _ufpTransformChildObj.transform.rotation);
        //判断移动速度及相关处理
        AvatarJudgetSpeed();

        //根据角度设置移动脚本
        if (_sm)
            _sm.SetParameter("IsMove", true);
        PlayerStateChanged(E_AvatarMotionState.AM_MOVE);

        // Debug.Log("_ufpTransform::" +_ufpTransformObj.position.ToVector() + ",_ufpTransform::" + _ufpTransformObj.gameObject.name);
    }

    public void Jump(bool flag = true)
    {
        // Debug.Log("jump:::" + flag);
        if (flag)
        {
            if (AvatarMotion == E_AvatarMotionState.AM_MOVE
            || AvatarMotion == E_AvatarMotionState.AM_MOVEJUMP)
                PlayerStateChanged(E_AvatarMotionState.AM_MOVEJUMP);
            else
                PlayerStateChanged(E_AvatarMotionState.AM_JUMP);
        }
        else
        {
            if (AvatarMotion == E_AvatarMotionState.AM_MOVEJUMP)
            {
                AvatarMotion = E_AvatarMotionState.AM_MOVE;
            }
            else
            {
                PlayerStateChanged(E_AvatarMotionState.AM_IDLE);
                // AvatarMotion = E_AvatarMotionState.AM_IDLE;
            }
        }

        _jumpFlag = flag;
    }

    public void Idle()
    {
        //根据角度设置移动脚本
        if (_sm)
            _sm.SetParameter("IsMove", false);
        PlayerStateChanged(E_AvatarMotionState.AM_IDLE);
    }

    public void Shoot(FPVector ComMoveFollowData, bool bStart = true)
    {
        if (bStart)
        {
            //如果当下武器需要的能量值存储的能量值，即不能射击
            if (WeaponCasterObj.GetMinEnergofWeapon() > EnergyValue)
            {
                bEnergyShortageByShoot = true;
                if (AvatarBehavior == E_AvatarBehavior.B_SHOOT)
                {
                    PlayerStateChanged(E_AvatarBehavior.B_NONE);  //恢复射击数据值
                }
                return;
            }
            // 开枪需检测是否在地面上，不然直接忽略
            if (_characterMotionObj.bOnGround)
            {
                if (_sm)
                {
                    _sm.SetParameter("AvatarForm", (int)E_AvatarForm.PERSON_STATE);
                    //1)变为人型
                    ChangeMeshRenderForm(E_AvatarForm.PERSON_STATE);
                    //2)播放开枪动画
                    PlayerStateChanged(E_AvatarBehavior.B_SHOOT);
                }

                //3)调整朝向跟摄像头一致
                if (!_characterMotionObj.bMoveFlag && ComMoveFollowData != FPVector.zero)
                {
                    ComMoveFollowObj.GetFPTransform().ChangeForward(ComMoveFollowData);
                    ComMoveFollowObj.ResetCameraForward();  //同步重置Component对象的旋转数值
                    ChangeAvatarForward(ComMoveFollowObj.GetFPTransform().forward);
                }
            }
        }
        else
        {
            PlayerStateChanged(E_AvatarBehavior.B_NONE);
        }

        bEnergyShortageByShoot = false;
        //4)开枪——关枪
        WeaponCasterObj.Shoot(bStart);
    }

    public Vector3 GetComMoveFollow()
    {
        return ComMoveFollowObj.transform.forward;
    }

    public void ThrowGrenade(bool bStart = true)
    {
        if (bStart)
        {
            //如果当下武器需要的能量值存储的能量值，即不能扔手雷
            if (WeaponCasterObj.GetMinEnergofWeapon(GameManager.Instance.GrenadesId) > EnergyValue)
            {
                bEnergyShortageByShoot = true;
                return;
            }
            // 需检测是否在地面上，不然直接忽略
            if (_characterMotionObj.bOnGround)
            {
                if (_sm)
                {
                    _sm.SetParameter("AvatarForm", (int)E_AvatarForm.PERSON_STATE);
                    //1)变为人型
                    ChangeMeshRenderForm(E_AvatarForm.PERSON_STATE);
                    //2)播放扔手雷动画
                    PlayerStateChanged(E_AvatarBehavior.B_GRENADE);
                }

                //3)调整朝向跟摄像头一致
                if (!_characterMotionObj.bMoveFlag)
                {
                    ComMoveFollowObj.ResetCameraForward();  //同步重置Component对象的旋转数值
                    ChangeAvatarForward(ComMoveFollowObj.GetForword());
                }
            }
        }
        else
        {
            PlayerStateChanged(E_AvatarBehavior.B_NONE);
        }

        bEnergyShortageByShoot = false;
        //4)开枪——关枪
        WeaponCasterObj.ThrowGrenade(bStart);
    }

    public void SendEvent()
    {
        int state = (int)(AvatarForm - 1) * _motionStateCount + (int)AvatarMotion + (int)AvatarBehavior * 100;
        TriggerEvent(DefineEventId.PlayerStateChangedEvent, state);
    }

    /// <summary>
    /// 玩家模型改变
    /// </summary>
    /// <param name="form"></param>
    public void ChangeMeshRenderForm(E_AvatarForm form)
    {
        if (form == _beforeAvatarForm) return;
        PlayerStateChanged(form);

        //人型状态
        //string formName = "";
        string curFormName = "";
        string otherFormName = "";
        string other1FormName = "";

        if (AvatarForm == E_AvatarForm.PERSON_STATE)
        {
            curFormName = PlayerCommonName.PersonModelName;
            otherFormName = PlayerCommonName.FishModelName;
            other1FormName = PlayerCommonName.FishDiveModelName;

        }
        else if (AvatarForm == E_AvatarForm.INKFISH_STATE)
        {
            curFormName = PlayerCommonName.FishModelName;
            otherFormName = PlayerCommonName.PersonModelName;
            other1FormName = PlayerCommonName.FishDiveModelName;
        }
        else if (AvatarForm == E_AvatarForm.INKFISHDIVE_STATE)
        {
            curFormName = PlayerCommonName.FishDiveModelName;
            otherFormName = PlayerCommonName.PersonModelName;
            other1FormName = PlayerCommonName.FishModelName;
        }
        _childObj.Find(curFormName).gameObject.SetActive(true);
        _childObj.Find(otherFormName).gameObject.SetActive(false);
        _childObj.Find(other1FormName).gameObject.SetActive(false);


        //if (_beforeAvatarForm == E_AvatarForm.PERSON_STATE)
        //{
        //    formName = PlayerCommonName.PersonModelName;
        //}
        //else if (_beforeAvatarForm == E_AvatarForm.INKFISH_STATE)
        //{
        //    formName = PlayerCommonName.FishModelName;
        //}
        //else if (_beforeAvatarForm == E_AvatarForm.INKFISHDIVE_STATE)
        //{
        //    formName = PlayerCommonName.FishDiveModelName;
        //}

        //_childObj.Find(formName).gameObject.SetActive(false);

        _beforeAvatarForm = AvatarForm;
    }

    /// <summary>
    /// avatar下潜或者跳跃的动作延时
    /// </summary>
    public void AvatarDiveorLeapInvoke()
    {
        PlayerStateChanged(AvatarForm);
        ChangeMeshRenderForm(AvatarForm);
    }

    /// <summary>
    /// 改变移动速度
    /// </summary>
    public void AvatarJudgetSpeed()
    {
        PixelManager.E_InColorType ColorType;
        bool bSlope = false;
        if (AcquireInkColorType(out ColorType, out bSlope))
        {
            //test...test....//
            // ColorType = PixelManager.E_InColorType.IC_SELF_COLOR;
            if (ColorType == PixelManager.E_InColorType.IC_SELF_COLOR)  //自身颜料中
            {

                ////!这是在油漆中自动下潜的代码
                //if (AvatarForm != E_AvatarForm.INKFISHDIVE_STATE)
                //{
                //if (_sm)
                //{
                //    _sm.SetParameter("AvatarForm", (int)E_AvatarForm.INKFISHDIVE_STATE);
                //    PlayerStateChanged(E_AvatarForm.INKFISHDIVE_STATE);
                //    ChangeMeshRenderForm(AvatarForm);
                //}
                //}
                //MoveSpeed = AddMoveSpeed;

                if (AvatarForm != E_AvatarForm.PERSON_STATE)
                {
                    MoveSpeed = AddMoveSpeed;
                }
                else
                {
                    MoveSpeed = NorMalMoveSpeed;
                }
            }
            else if (ColorType == PixelManager.E_InColorType.IC_OTHER_COLOR) //敌方颜料中
            {
                MoveSpeed = SubMoveSpeed;
                //!如果此时为乌贼下潜状态，则需要变化为人型
                if (AvatarForm == E_AvatarForm.INKFISHDIVE_STATE && !bSlope)
                {
                    if (_sm)
                    {
                        _sm.SetParameter("AvatarForm", (int)E_AvatarForm.PERSON_STATE);
                        PlayerStateChanged(E_AvatarForm.PERSON_STATE);
                        ChangeMeshRenderForm(AvatarForm);
                    }
                }

                //2)处于敌方颜料中血量的减少
                Hp--;
            }
            else //空地中
            {
                //!如果此时为乌贼下潜状态，则需要变化为乌贼
                if (AvatarForm == E_AvatarForm.INKFISHDIVE_STATE && !bSlope)
                {
                    if (_sm)
                    {
                        _sm.SetParameter("AvatarForm", (int)E_AvatarForm.INKFISH_STATE);
                        PlayerStateChanged(E_AvatarForm.INKFISH_STATE);
                        ChangeMeshRenderForm(AvatarForm);
                    }
                }
                MoveSpeed = NorMalMoveSpeed;
            }

            //    Debug.Log("MoveSpeed:::"+ MoveSpeed + ",ColorType::"+ ColorType);
            _characterMotionObj.MoveSpeed = (MoveSpeed * _speedRate);
        }

        AcquireRestoreEnergySpeed(ColorType);
    }

    /// <summary>
    /// 玩家跃出时的处于空中的事件
    /// </summary>
    public void PlayerInAirEvent()
    {
        if (AvatarForm == E_AvatarForm.INKFISHDIVE_STATE)
        {
            _bInAirtoLeap = true;
            if (_sm)
            {
                _sm.SetParameter("AvatarForm", (int)E_AvatarForm.INKFISH_STATE);
                PlayerStateChanged(E_AvatarForm.INKFISH_STATE);
                ChangeMeshRenderForm(AvatarForm);
            }
        }
    }

    #endregion

    #region 3) =========玩家受攻击等操作=========

    /// <summary>
    /// 玩家受到攻击
    /// </summary>
    /// <param name="damage"></param>
    public void TargetAttack(int damage)
    {
        Hp -= damage;
    }

    #endregion

    #region 4) =========根据摄像机对人物朝向进行转换=========
    /// <summary>
    /// 改变朝向，改变forward的朝向跟摄像机的朝向一样
    /// </summary>
    /// <param name="forward"></param>
    public void ChangeAvatarForward(FPVector toForward)
    {
        JoystickAngle = 90;
        if (AvatarForm == E_AvatarForm.PERSON_STATE)
        {
            _ufpTransformChildObj.ChangeForward(toForward);
            return;
        }

        RaycastHit _groundHit;
        _characterMotionObj.RayGroundInfo(out _groundHit);
        FPVector _fpNormal = _ufpTransformChildObj.ChangeVec3ToTSVec(_groundHit.normal);
        if (_groundHit.normal == Vector3.up)
        {
            _ufpTransformChildObj.ChangeForward(toForward);

            // _childObj.forward = toForward;
            JoystickAngle = 90;
            return;
        }
        else
        {
            FPVector left = FPVector.Cross(toForward, _fpNormal);   //切线
            FPVector newForward = FPVector.Cross(_fpNormal, left);

            if (_fpNormal == FPVector.zero)
            {
                Debug.Log("forward:::" + toForward);
                return;
            }
            FPQuaternion newRotation = FPQuaternion.LookRotation(newForward, _fpNormal);
            U_FPTransform ComMoveTransform = ComMoveFollowObj.GetFPTransform();
            FPQuaternion comObject = new FPQuaternion(ComMoveTransform.rotation.x, ComMoveTransform.rotation.y,
                                          ComMoveTransform.rotation.z, ComMoveTransform.rotation.w);

            // ComMoveFollowObj.transform.rotation = newRotation;
            ComMoveTransform.SetRotation(newRotation);

            FPVector comForward = ComMoveTransform.forward;
            FPVector childForward = _ufpTransformChildObj.forward;
            FP DragAngle = FPVector.Angle(comForward, childForward);
            DragAngle = FPMath.Sign(FPVector.Cross(childForward, comForward).y) * DragAngle;
            ChangeAvaterForward(DragAngle);   //改变对象的旋转数值
            ComMoveTransform.SetRotation(comObject);   //恢复摄像机原来的rotation数值
        }
    }


    /// <summary>
    /// 改变朝向，通过摄像机改变玩家方向
    /// </summary>
    /// <param name="RotateAngel"></param>
    public void ChangeAvaterForward(FP RotateAngel)
    {
        if (RotateAngel != 0)
        {
            //选择物体角度，然后进行移动
            // _childObj.Rotate(0, RotateAngel, 0);
        //    Debug.Log("RotateAngel:::" + RotateAngel.AsFloat());
            RotateChildByFP(0, RotateAngel, 0);
        }
    }
    #endregion

    #region 5) =========像素颜色获取=========

    public bool AcquireInkColorType(out PixelManager.E_InColorType ColorType)
    {
        RaycastHit info;
        if (!_characterMotionObj.OnGrouldRayInfoAndRender(out info))
        {
            ColorType = PixelManager.E_InColorType.IC_BLANK_COLOR;
            return false;
        }
        ColorType = PixelManager.GetInstance().GetPixelsInfo(AvatarEntityId, info.textureCoord);
        return true;
    }

    public bool AcquireInkColorType(out PixelManager.E_InColorType ColorType, out bool Slope)
    {
        RaycastHit info;
        if (!_characterMotionObj.OnGrouldRayInfoAndRender(out info))
        {
            Slope = false;
            ColorType = PixelManager.E_InColorType.IC_BLANK_COLOR;
            return false;
        }
        ColorType = PixelManager.GetInstance().GetPixelsInfo(AvatarEntityId, info.textureCoord);
        Slope = (Vector3.Angle(Vector3.up, info.normal) > _characterMotionObj.SlopeAngle) ? true : false;
        return true;
    }

    #endregion

    #region 6) =========enterBattle前的相关数值操作=========

    /// <summary>
    /// 进入战斗的相关数据赋值
    /// </summary>
    /// <param name="entityId"></param>
    public void EnterBattleRelateData(int entityId)
    {
        AvatarEntityId = entityId;

        InitWeaponData(entityId);

        AutoRenderSplatData(entityId);
    }

    /// <summary>
    /// 初始化武器数据
    /// </summary>
    public void InitWeaponData(int entityId)
    {
        if (!WeaponCasterObj)
        {
            Debug.Log("SetWeaponRenderColor:::WeaponCasterObj is null!!");
            return;
        }
        int   WeaponId    = GameManager.Instance.GetWeaponIdByEntityId(entityId);
        Color WeaponColor = GameManager.Instance.GetTeamColorByEntityId(entityId);
        PixelManager.GetInstance().SetPlayerIdToTeamColor(entityId, WeaponColor);
        WeaponCasterObj.InitWeaponData(WeaponId, WeaponColor);
    }

    /// <summary>
    /// 主动渲染贴花Render数据
    /// </summary>
    /// <param name="entityId"></param>
    public void AutoRenderSplatData(int entityId)
    {
        if (!_characterMotionObj)
        {
            Debug.Log("entityId::" + entityId + ", Avatar_characterMotionObj is null!==》AutoRenderSplatData！");
            return;
        }
        _characterMotionObj.AutoRenderSplatData(entityId);

    }

    #endregion

    #region 7) =========关于武器子弹的操作=========

    /// <summary>
    /// 更换武器
    /// </summary>
    public void ChangeWeapon(int WeaponId)
    {
        WeaponCasterObj.ChangeWeaponEvent(WeaponId);
    }


    /// <summary>
    /// 能量补充
    /// </summary>
    /// <returns></returns>
    IEnumerator RestoreEnergy()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            RestoreEnergys();
        }
    }

    public void RestoreEnergys()
    {
        if (EnergyValue < WeaponComConfig.UNIFORM_AMMUNITION)
        {
            EnergyValue += this.RestoreEnergySpeed;
            if (EnergyValue > WeaponComConfig.UNIFORM_AMMUNITION)
                EnergyValue = WeaponComConfig.UNIFORM_AMMUNITION;
        }

    }


    /// <summary>
    /// 获取恢复弹药的速度
    /// </summary>
    /// <param name="type"></param>
    void AcquireRestoreEnergySpeed(PixelManager.E_InColorType type)
    {
        RestoreEnergySpeed = JudgetRestoreBulletSpeed((type == PixelManager.E_InColorType.IC_SELF_COLOR &&
                                                                        AvatarForm == E_AvatarForm.INKFISHDIVE_STATE) ? true : false);
    }

    /// <summary>
    /// 获取当前能量储值
    /// </summary>
    /// <returns></returns>
    public int AcquireCurrentEnerget()
    {
        return EnergyValue;
        if (WeaponCasterObj.CurrentWeapon != null)
            return WeaponCasterObj.CurrentWeapon.WeaponProperty.HP;
        return 0;
    }

    /// <summary>
    /// 判断子弹库存恢复速度
    /// </summary>
    /// <param name="bSelfInkFlag"></param>
    /// <returns></returns>
    public int JudgetRestoreBulletSpeed(bool bSelfInkFlag)
    {
        if (bSelfInkFlag)
            return WeaponComConfig.INK_RESTORE_SPEED;
        else
            return WeaponComConfig.NOR_RESTORE_SPEED;
    }

    #endregion

    #region 8) =========ArtAnimatorManager=========


    /// <summary>
    /// 选择avatar的形态
    /// PERSON_STATE = 1,INKFISH_STATE = 2,INKFISHDIVE_STATE = 3
    /// </summary>
    /// <param name="form"></param>
    public void ChangeAvatarForm(E_AvatarForm form)
    {
        //1）如果要变化为乌贼下潜状态的话，需要看是否处于自身油漆中
        PixelManager.E_InColorType colorType;
        bool Slope = false;
        if (!AcquireInkColorType(out colorType, out Slope)) return;
        //test...test ..//
        //colorType = PixelManager.E_InColorType.IC_SELF_COLOR;
        if (colorType != PixelManager.E_InColorType.IC_SELF_COLOR &&
            form == E_AvatarForm.INKFISHDIVE_STATE) return;

        if (_sm)
            _sm.SetParameter("AvatarForm", (int)form);

        if (AvatarForm != form)
        {
            int DiveState = 0;

            E_AvatarMotionState state = AvatarMotion;

            if (form == E_AvatarForm.INKFISHDIVE_STATE &&
                AvatarForm == E_AvatarForm.PERSON_STATE && AvatarMotion == E_AvatarMotionState.AM_IDLE)//变化Avatar角色变为乌贼下潜模式时,调用下潜动画
            {
                DiveState = 1;
            }
            else if (AvatarForm == E_AvatarForm.INKFISHDIVE_STATE && form != E_AvatarForm.INKFISHDIVE_STATE)
            { //变化Avatar角色变为非乌贼下潜模式时,调用跃出动画
                DiveState = 2;
            }

            if (DiveState == 1)
            {
                _artAnimatorManager.Dive(true);
                AvatarForm = form;
                Invoke("AvatarDiveorLeapInvoke", 0.25f);
            }
            else
            {
                PlayerStateChanged(form);
                ChangeMeshRenderForm(form);
            }

            //if (IsInvoking()) DiveState = 0;
            //if (0 == DiveState)
            //{
            //    PlayerStateChanged(form);
            //    ChangeMeshRenderForm(form);
            //}
            //else
            //{
            //    DealAvatarDiveorLeap(form, state);
            // }
        }
    }
    public void PlayerStateChanged(E_AvatarMotionState motionState)
    {
        AvatarMotion = motionState;
        if (!_artAnimatorManager) return;
        if (motionState == E_AvatarMotionState.AM_IDLE)
        {
            _artAnimatorManager.Move(false);
        }
        else if (motionState == E_AvatarMotionState.AM_MOVE)
        {
            _artAnimatorManager.Move(true);
        }
        else if (motionState == E_AvatarMotionState.AM_MOVEJUMP)
        {
            _artAnimatorManager.Move(true);
            _artAnimatorManager.Jump();
            SimulateJump();
        }
        else if (motionState == E_AvatarMotionState.AM_JUMP)
        {
            _artAnimatorManager.Jump();
            SimulateJump();
        }

        //SendEvent();
    }
    public void PlayerStateChanged(E_AvatarForm formState)
    {
        AvatarForm = formState;
        if (_artAnimatorManager)
            _artAnimatorManager.ChangeModelState(formState);
        //  SendEvent();
    }
    public void PlayerStateChanged(E_AvatarBehavior behaviorState)
    {
        AvatarBehavior = behaviorState;

        if (!_artAnimatorManager) return;
        if (behaviorState == E_AvatarBehavior.B_SHOOT)
        {
            if (AvatarMotion == E_AvatarMotionState.AM_MOVE)
            {
                int direction = 1;
                if (JoystickAngle >= 100 && JoystickAngle <= 250)
                {
                    direction = 3;
                }
                else if (JoystickAngle > 280 || JoystickAngle < 80)
                {
                    direction = 4;
                }
                else if (JoystickAngle >= 260 && JoystickAngle <= 280)
                {
                    direction = 2;
                }
                _artAnimatorManager.Shoot(true, direction);
            }
            else
            {
                _artAnimatorManager.Shoot(true);
            }

        }
        else if (behaviorState == E_AvatarBehavior.B_GRENADE)
        {
            _artAnimatorManager.Grenade(true);
        }
        else
        {
            _artAnimatorManager.Shoot(false);
        }
        // SendEvent();
    }

    /// <summary>
    /// 模拟跳跃,不使用动画带高度跳跃
    /// </summary>
    public void SimulateJump()
    {
        _characterMotionObj.Jump();
    }

    #endregion

    #region 10) =========test测试操作=========

    public void testNum()
    {
        int entityId = 1000;
        AvatarEntityId = entityId;
        PixelManager.GetInstance().SetPlayerIdToTeamColor(entityId, Color.blue);
        WeaponCasterObj.SetWeaponInkColor(Color.blue);
        AutoRenderSplatData(entityId);
    }

    #endregion

    #region  11) =========定点数、物理引擎等相关函数=========

    //public override void OnTestFrame(uint frameId)
    //{
    // //   Debug.LogError("frameId::::" + frameId + "_ufpTransformObj:::" + _ufpTransformObj.position.ToVector() + ",,transform:::" + transform.position);
    //}

    private void RotateChildByFP(FP xAngle, FP yAngle, FP zAngle)
    {
        _ufpTransformChildObj.Rotate(xAngle, yAngle, zAngle);
        _ufpTransformChildObj.UpdateRotation();
       // _ufpTransformChildObj.SetPosition(_ufpTransformChildObj.position);

        //Debug.Log("FP_FP_right:::" + _ufpTransformChildObj.right.ToVector() + ",,,right:::" + _ufpTransformChildObj.transform.right 
        //    + ",yAngle::"+ yAngle + ",FP_forward::"+ _ufpTransformChildObj.forward.ToVector() + ",_forward::" + _ufpTransformChildObj.transform.forward
        //    + ",FP_Rotation"+_ufpTransformChildObj.rotation.ToQuaternion() + ",_Rotation" + _ufpTransformChildObj.transform.rotation);
        // Debug.Log("FP_FP_right:::"+ _ufpTransformChildObj.right.ToVector() + ",,,right:::" + _ufpTransformChildObj.transform.right);
        //    Debug.Log("yAngle:::"+ yAngle.AsFloat() + ",_ufpTransformChildObj::"+ _ufpTransformChildObj.rotation.ToQuaternion()  + ",_childObj::" + _childObj.rotation);
        // Debug.Log("yAngle:::" + yAngle.AsFloat() + ",_ufpTransformChildObj_eulerAngles::" + _ufpTransformChildObj.rotation.eulerAngles.ToVector() + ",_childObj_eulerAngles::" + _childObj.rotation.eulerAngles);
        //_childObj.rotation = _ufpTransformChildObj.rotation.ToQuaternion();
        //_childObj.position = _ufpTransformChildObj.position.ToVector();
    }


    public override void OnSyncedUpdate()
    {
        DealSycncedEndInfo();

        FPVector changeForwardVec3 = FrameSyncInput.GetFPVector((byte)E_InputId.E_DRAGGING_CAMERA);
        bool IdelFlag = FrameSyncInput.GetBool((byte)E_InputId.E_IDEL);
        FP move_x = FrameSyncInput.GetFP((byte)E_InputId.E_MOVE_X);
        FP move_y = FrameSyncInput.GetFP((byte)E_InputId.E_MOVE_Y);


        bool mouseDraggingFlag = FrameSyncInput.GetBool((byte)E_InputId.E_MOUSE_DRAGGING);
        bool mouseDragEndFlag  = FrameSyncInput.GetBool((byte)E_InputId.E_MOUSE_DRAGEND);
        if (mouseDraggingFlag || mouseDragEndFlag)
        {
            bool leftDirection  = FrameSyncInput.GetBool((byte)E_InputId.E_MOUSE_DRAG_LEFT);
            bool rightDirection = FrameSyncInput.GetBool((byte)E_InputId.E_MOUSE_DRAG_RIGHT);
            ComMoveFollowObj.SetMouseDragDirection(leftDirection, rightDirection);
            if(mouseDraggingFlag) ComMoveFollowObj.SetMouseDragFlag(true);
            else ComMoveFollowObj.SetMouseDragFlag(false);
        }

        if (changeForwardVec3 != FPVector.zero)
        {
             Debug.Log("changeForwardVec3:::::::::::" + changeForwardVec3.ToVector() + ",,,E_InputId.E_IDEL::" + IdelFlag);
             Debug.Log("FP_FP_FP_::move_x_xxxxxxxx::" + move_x.AsFloat() + ",move_y::" + move_y.AsFloat());
        }

        //移动处理
        if (IdelFlag)
        {
            Idle();
            ComMoveFollowObj.SetJoyStickMoveFlag(false);
            //Debug.Log("idelidelidelidelidelidelidelidelidelidelidelidelidelidelidel");
        }
        else
        {

            if (move_x != 0 || move_y != 0)
            {
                if (changeForwardVec3 != FPVector.zero)
                {
                    Debug.Log("changeForwardVec3::" + changeForwardVec3.ToVector() + "camera::" + ComMoveFollowObj.transform.forward + ",touch:::" + ComMoveFollowObj._bTouchMouse);
                    ChangeAvatarForward(changeForwardVec3);
                   // Debug.Log("changeForwardVec3:::::::::::" + changeForwardVec3.ToVector()  +"camera::"+ ComMoveFollowObj.transform.forward);
                } 
                SycnMove(move_x, move_y);
            }
        }

      
        //射击处理
        bool shootDownFlag = FrameSyncInput.GetBool((byte)E_InputId.E_SHOOT_DOWN);
        if (shootDownFlag)
        {
            Shoot(changeForwardVec3);
        }
        bool shootUpFlag = FrameSyncInput.GetBool((byte)E_InputId.E_SHOOT_UP);
        if (shootUpFlag) Shoot(FPVector.zero, false);

        //跳跃处理
        bool jumpFlag = FrameSyncInput.GetBool((byte)E_InputId.E_JUMP);
        if (jumpFlag) {
            Jump();
        } 

        //模型改变处理
        int modelRender = FrameSyncInput.GetInt((byte)E_InputId.E_CHANGE_MODEL);
        if (modelRender > 0)
        {
            if (modelRender == (int)KeyCode.P)      //人形
                ChangeAvatarForm(Avatar.E_AvatarForm.PERSON_STATE);
            else if (modelRender == (int)KeyCode.F)  //乌贼
                ChangeAvatarForm(Avatar.E_AvatarForm.INKFISH_STATE);
            else if (modelRender == (int)KeyCode.D)  //下潜模式
                ChangeAvatarForm(Avatar.E_AvatarForm.INKFISHDIVE_STATE);
        }

        //更换装备
        int replaceWeaponFlag = FrameSyncInput.GetInt((byte)E_InputId.E_REPLACE_WEAPON);
        if (replaceWeaponFlag > 0)
        {
            ChangeWeapon(replaceWeaponFlag);
        }

        //扔手雷
        bool throwGrenadeFlag = FrameSyncInput.GetBool((byte)E_InputId.E_THROW_GRENADE);
        if (throwGrenadeFlag) ThrowGrenade();
    }

    public void DealSycncedEndInfo()
    {
        _characterMotionObj.RenderEndUpdatePosition();
    }


    public void SycnMove(FP move_x, FP move_y)
    {
        FP _triggerAngle = CalculaAngle(move_x, move_y);
        //_triggerAngle = 90;
      //  Debug.Log("----------------->_triggerAngle ------->before:::" + _triggerAngle.AsFloat() +" ,_fp::" + _ufpTransformObj.transform.position +",_unity::" +_ufpTransformObj.transform.position.ToFPVector()  +" ,_fp_child::" + _ufpTransformChildObj.forward + ",_unity_child::" + _ufpTransformChildObj.forward.ToVector());
        Move(_triggerAngle);

      //  Debug.Log("----------------->_triggerAngle ------->late:::"   + _triggerAngle.AsFloat() + " ,_fp::" + _ufpTransformObj.transform.position + ",_unity::" + _ufpTransformObj.transform.position.ToFPVector() + " ,_fp_child::" + _ufpTransformChildObj.forward + ",_unity_child::" + _ufpTransformChildObj.forward.ToVector());
        ComMoveFollowObj.JoyStickAngleChangeEvent(_triggerAngle);
        ComMoveFollowObj.SetJoyStickMoveFlag(true);
    //    Debug.Log("angleangleangleangle::" + _triggerAngle.AsFloat());
    }

    /// <summary>
    /// 角度处理，处理为 0 - 360
    /// </summary>
    /// <param name="_joyPositionX"></param>
    /// <param name="_joyPositionY"></param>
    /// <returns></returns>
    private FP CalculaAngle(FP _joyPositionX, FP _joyPositionY)
    {
        FP currentAngleX = _joyPositionX * 90f + 90f; //X轴 当前角度
        FP currentAngleY = _joyPositionY * 90f + 90f; //Y轴 当前角度
                                                      //    Debug.Log(currentAngleX+" , "+ currentAngleY);
                                                      //下半圆
        if (currentAngleY < 90f)
        {
            if (currentAngleX < 90f)
            {
                return 180f + (90f - currentAngleY);
            }
            else if (currentAngleX > 90f)
            {
                return 270f + currentAngleY;
            }
        }
        return 180f - currentAngleX;
    }

   

    #endregion
}
