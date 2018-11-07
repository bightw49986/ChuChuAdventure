using UnityEngine;
using BattleSystem;
public class PlayerAnimEvents : MonoBehaviour 
{
    Player player;
    PlayerFSMGenerater playerFSMGenerater;
    BattleData battleData;
    void Awake()
    {
        player = GetComponent<Player>();
        playerFSMGenerater = GetComponent<PlayerFSMGenerater>();
        battleData = GetComponent<BattleData>();
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
    void OAB()
    {
        battleData.EnableAttackBox(0);
    }
    void CAB()
    {
        battleData.DisableAttackBox(0);
    }
}
