using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerEvents : MonoBehaviour
{
    public PlayerAttack playerAttack;
    public Sword sword;

    void TurnOnSwordHitbox()
    {
        sword.swordHitbox = true;
    }

    void TurnOffSwordHitbox()
    {
        sword.swordHitbox = false;
    }
}
