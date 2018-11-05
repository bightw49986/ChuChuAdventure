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
        battleData.DisableAttackBox(0);
    }
    void StartRecord()
    {
        playerFSMGenerater.BAllowTransit = false;
        player.bPreEnter = true;
        battleData.EnableAttackBox(0);
    }
}
