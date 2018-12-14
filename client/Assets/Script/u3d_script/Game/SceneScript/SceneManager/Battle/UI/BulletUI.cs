using GameLogic.Configs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletUI : MonoBehaviour
{
    public Transform AvatarPlayerObj;
    private Avatar _avatarObj;
    private RectTransform _bulletsUIObj;
    private Canvas _canvas;
    private Transform _player;
    private Transform _componentPoint;
    // Use this for initialization
    void Start ()
    {
        _player    = AvatarPlayerObj.Find(PlayerCommonName.PlayerName).transform;
        _avatarObj = AvatarPlayerObj.Find(PlayerCommonName.PlayerName).GetComponent<Avatar>();
        _bulletsUIObj = gameObject.transform.Find(PlayerCommonName.BulletsUIName).GetComponent<RectTransform>();
        _canvas = _player.Find("Canvas").GetComponent<Canvas>();
        _componentPoint = AvatarPlayerObj.Find(PlayerCommonName.ComponentPoint).transform;
    }
	
	// Update is called once per frame
	void Update ()
    {
        int hp = _avatarObj.AcquireCurrentEnerget();
        //if (_avatarObj.AvatarForm == Avatar.E_AvatarForm.INKFISHDIVE_STATE
        //    || hp < 10)
        //{
        //    gameObject.SetActive(true);
        //}
        //else
        //{
        //    gameObject.SetActive(false);
        //}
    
        ChangeBulletPosition();
        ChangeBulletCount(hp);
    }

    private void ChangeBulletCount(int Hp)
    {
        float Height = _bulletsUIObj.rect.height;
        float realHeight = (float)Hp * (float)(1 / 100f);
        if (realHeight == 1 && Height == 1) return;

        _bulletsUIObj.sizeDelta = new Vector2(_bulletsUIObj.rect.width, realHeight);
        _bulletsUIObj.localPosition = new Vector3(_bulletsUIObj.localPosition.x, _bulletsUIObj.localPosition.y - (Height - realHeight) / 2f, 0);
      //  Debug.Log("cccccccccccccccccccccccc:::"+ Hp);
    }

    private void ChangeBulletPosition()
    {
        _canvas.transform.rotation = _componentPoint.rotation;
    }

    private void OnDestroy()
    {
        Destroy(_avatarObj);
        Destroy(_bulletsUIObj);
        Destroy(_canvas);
        Destroy(_player);
        Destroy(_componentPoint);
        Resources.UnloadUnusedAssets();
    }
}
