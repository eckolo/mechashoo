using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アームの途中接続部
/// </summary>
public class Arm : Parts
{
    public Hand tipHand
    {
        get {
            var hand = GetComponent<Hand>();
            if(childParts == null) return hand;
            if(hand != null) return hand;

            var childArm = childParts.GetComponent<Arm>();
            if(childArm == null) return hand;
            return childArm.tipHand;
        }
    }
}
