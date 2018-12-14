using UnityEngine;
using GameLogic.Configs;

public class ArtAnimatorManager : MonoBehaviour {

    private Animator _animator;
    // Use this for initialization

    private GameObject _diveLoopEffectPrefab = null;
    private GameObject _diveNoLoopEffectPrefab = null;
    private GameObject _weaponEffectOPrefab = null;
    private GameObject _diveEffObject;   //下潜特效
    private GameObject _weaponEffObject; //武器特效

    public Transform  WeaponPointObj; //武器开枪口对象
    void Start () {
        _animator = gameObject.transform.GetComponent<Animator>();
    }

	// Update is called once per frame
	void Update ()
    {
		
	}

    #region  1)===================关于动画=====================
    public void Move(bool flag)
    {
        if (!_animator) return;

        _animator.SetBool(AnimatorConstName.Move,flag);

        if (_animator.GetBool(AnimatorConstName.Dive))
            ModelMoveToEffect(flag);
        else 
        {
            ModelMoveToEffect(false);
        }
    }

    public void Jump()
    {
        if (_animator)
        {
            _animator.SetTrigger(AnimatorConstName.Jump);
        }
    }
    public void Shoot(bool flag, int AttackDirection = 0)
    {
        if (_animator)
        {
            _animator.SetInteger(AnimatorConstName.AttackDirection, AttackDirection);
            _animator.SetBool(AnimatorConstName.Shoot, flag);
            _animator.SetBool(AnimatorConstName.Grenade, flag);
            ModelAttackToEffect(flag);
        }
    }
    public void Grenade(bool flag)
    {
        if (_animator)
        {
           _animator.SetBool(AnimatorConstName.Grenade, flag);
        }
    }

    public void Dive(bool flag)
    {
        _animator.SetBool(AnimatorConstName.Dive, flag);
    }

    public void Death()
    {
        _animator.SetTrigger(AnimatorConstName.Death);
    }

    public void ChangeModelState(Avatar.E_AvatarForm ModelState)
    {
     
        int state = _animator.GetInteger(AnimatorConstName.ModelState);
      
        if (state == (int)ModelState) return;
        _animator.SetInteger(AnimatorConstName.ModelState, (int)ModelState);
        if (ModelState == (Avatar.E_AvatarForm.INKFISHDIVE_STATE))
        { 
            //表示为进入下潜状态
            _animator.SetBool(AnimatorConstName.Dive, true);
            if (state == (int)(Avatar.E_AvatarForm.PERSON_STATE))
            {
                ChangeAnimtorController(false);
            }
            ModelMoveToEffect(true, false);
        }
        else
        {
            _animator.SetBool(AnimatorConstName.Dive, false);
            if (ModelState == (Avatar.E_AvatarForm.PERSON_STATE) && state > (int)(Avatar.E_AvatarForm.PERSON_STATE))
            {
                ChangeAnimtorController(true);
            }
            else if(ModelState == (Avatar.E_AvatarForm.INKFISH_STATE) && state == (int)(Avatar.E_AvatarForm.PERSON_STATE))
            {
                ChangeAnimtorController(false);
            }
        }
    }

    public void ChangeAnimtorController(bool AvatarController)
    {
        bool bDive = _animator.GetBool(AnimatorConstName.Dive);
        bool bShoot = _animator.GetBool(AnimatorConstName.Shoot);
        bool bMove  = _animator.GetBool(AnimatorConstName.Move);
        int  ModelState = _animator.GetInteger(AnimatorConstName.ModelState);

        if (_animator && AvatarController)
        {
            _animator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load(AnimatorConstName.PEASON_CONTROLLER_PATH);
        }
        else
        {
            _animator.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load(AnimatorConstName.FISH_CONTROLLER_PATH);
        }
      //  Debug.Log("bDive::"+ bDive + ",shoot::"+ bShoot + ",move::"+ bMove + ",ModelState:::" + ModelState);
        _animator.SetBool(AnimatorConstName.Dive,  bDive);
        _animator.SetBool(AnimatorConstName.Shoot, bShoot);
        _animator.SetBool(AnimatorConstName.Move,  bMove);
        _animator.SetInteger(AnimatorConstName.ModelState, ModelState);
    }

    #endregion

    #region  2)===================关于特效=====================
    public void ModelMoveToEffect(bool flag, bool Loop = true)
    {
      //  Debug.Log("ModelMoveToEffect::" + flag + ",,::Loop:"+ Loop);
        if (flag)
        {
            if (Loop)
                LoadEffectToDive(EffectConstName.FISH_DIVE_EFFECT_PATH);
            else
                LoadEffectToDive(EffectConstName.FISH_DIVE_EFFECT_NOLOOP_PATH);
            _diveEffObject.transform.position = transform.position;
            _diveEffObject.transform.rotation = transform.rotation;
        }
        else
        {
            if (_diveEffObject)
            {
                Destroy(_diveEffObject);
                _diveEffObject = null;
            }
           
        }
    }

    /// <summary>
    /// 移动特效
    /// </summary>
    public void ModelAttackToEffect(bool flag)
    {
        if (flag)
        {
            LoaddEffectToWeapon(EffectConstName.WEAPON_EFFECT_PATH);
            _weaponEffObject.transform.position = WeaponPointObj.position;
            _weaponEffObject.transform.rotation = WeaponPointObj.rotation;
        }
        else
        {
            if (_weaponEffObject)
            {
                Destroy(_weaponEffObject);
                _weaponEffObject = null;
            }
        }
    }
   
    /// <summary>
    /// 加载下潜的对象特效
    /// </summary>
    /// <param name="name"></param>
    public void LoadEffectToDive(string name)
    {
        if (_diveEffObject) Destroy(_diveEffObject);
        _diveEffObject = null;
        if (name == "") return;
        if (EffectConstName.FISH_DIVE_EFFECT_PATH == name)
        {
            if(!_diveLoopEffectPrefab)
                _diveLoopEffectPrefab = (GameObject)Resources.Load(name);
            _diveEffObject = _diveLoopEffectPrefab;

        }
        else if (EffectConstName.FISH_DIVE_EFFECT_NOLOOP_PATH == name)
        {
            if (!_diveNoLoopEffectPrefab)
                _diveNoLoopEffectPrefab = (GameObject)Resources.Load(name);
            _diveEffObject = _diveNoLoopEffectPrefab;
        }
      //  _diveEffObject = (GameObject)Resources.Load(name);
        if (_diveEffObject == null) return;
        _diveEffObject = MonoBehaviour.Instantiate(_diveEffObject, transform.position, transform.rotation);
    }

    public void LoaddEffectToWeapon(string name)
    {
        if (_weaponEffObject) Destroy(_weaponEffObject);
        _weaponEffObject = null;
        if (name == "") return;
        if (EffectConstName.WEAPON_EFFECT_PATH == name)
        {
            if (!_weaponEffectOPrefab)
                _weaponEffectOPrefab = (GameObject)Resources.Load(name);
            _weaponEffObject = _weaponEffectOPrefab;
        }
     
        if (_weaponEffObject == null) return;
        _weaponEffObject = MonoBehaviour.Instantiate(_weaponEffObject, WeaponPointObj.position, WeaponPointObj.rotation);
    }

    #endregion

}
