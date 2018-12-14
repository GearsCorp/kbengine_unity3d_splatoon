using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChange : MonoBehaviour {
    
    public  int Index;
    private int Id;
	// Use this for initialization
	void Start () {

        if (Index < GameManager.Instance.WeaponQueue.Count) {
            Id = GameManager.Instance.WeaponQueue[Index];
        }

        GetComponent<Button>().onClick.AddListener(OnClick);
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
    void OnClick()
    {
        if (Index >= GameManager.Instance.WeaponQueue.Count) return;
        GameManager.Instance.CurWeapon = Id;
        KBEngine.Event.fireIn("weaponChanged", Id);
        Debug.Log("Id:::" + Id); 
    }
}
