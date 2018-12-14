using GameLogic.Configs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletUIShow : MonoBehaviour
{
    public  Transform AvatarPlayerObj;
    private Avatar _avatarObj;
    private GameObject _bulletUIGameObj;


    // Use this for initialization
    void Start()
    {
        _avatarObj = AvatarPlayerObj.Find(PlayerCommonName.PlayerName).GetComponent<Avatar>();
        _bulletUIGameObj = transform.Find("BulletCount").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

        int EnergyValue = _avatarObj.AcquireCurrentEnerget();
        if (_avatarObj.AvatarForm == Avatar.E_AvatarForm.INKFISHDIVE_STATE
            || EnergyValue < 10 || _avatarObj.bEnergyShortageByShoot)
        {
            _bulletUIGameObj.SetActive(true);
        }
        else
        {
            _bulletUIGameObj.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        Destroy(_bulletUIGameObj);
    }
}
