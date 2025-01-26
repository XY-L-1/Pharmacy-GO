using UnityEngine;
using System.Collections.Generic;

public interface IPlayerTriggerable
{
    void OnTriggerEnter2D(PlayerControl player);
    void OnPlayerTriggerExit(PlayerControl player);
}
