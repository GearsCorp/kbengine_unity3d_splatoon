using CBFrame.Core;
using CBFrame.Sys;
using GameLogic.Configs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtManager : CBComponent
{
    
    // Use this for initialization
    private CBArtPlayer _st;

    private void Start()
    {
        _st = gameObject.GetComponent<CBArtPlayer>();
        _st.Init("data/avatar_art");
       AddEventListener<int>(DefineEventId.PlayerStateChangedEvent, OnAvatarStateChanged);
    }

    // Update is called once per frame
    private void Update ()
    {
		
	}

    void OnAvatarStateChanged(int state)
    {
        if (!ArtEventIds.GetArtNameDict().ContainsKey(state)) return;

        string name = ArtEventIds.GetArtNameDict()[state];

     //   Debug.Log("name::"+ name + ",state::" + state);
        if (_st && name != null)
        {
            _st.stateChanged(name);
        }
    }

}
