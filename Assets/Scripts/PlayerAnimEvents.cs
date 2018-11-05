using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimEvents : MonoBehaviour {
    Player player;
    PlayerFSMGenerater playerFSMGenerater;
    private void Awake()
    {
        player = GetComponent<Player>();
        playerFSMGenerater = GetComponent<PlayerFSMGenerater>();
    }
    void CloseRecord()
    {
        player.bPreEnter = false;
        playerFSMGenerater.BAllowTransit = true;
    }
    void StartRecord()
    {
        playerFSMGenerater.BAllowTransit = false;
        player.bPreEnter = true;
    }
}
