using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アームの途中接続部
/// </summary>
public class Arm : Parts
{
    /// <summary>
    /// 制御力
    /// </summary>
    [SerializeField]
    float _power = 0.01f;
    /// <summary>
    /// 制御力
    /// </summary>
    public float power
    {
        get {
            var parentArm = nowParent?.GetComponent<Arm>();
            if(parentArm != null) return parentArm.power;
            return _power;
        }
    }

    /// <summary>
    /// 先端のHandオブジェクト
    /// </summary>
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
    /// <summary>
    /// 先端部までの長さ
    /// </summary>
    public float tipLength
    {
        get {
            var length = nowLengthVector.magnitude;
            if(GetComponent<Hand>() != null) return length;
            if(childParts == null) return length;

            var childArm = childParts.GetComponent<Arm>();
            if(childArm == null) return length;
            return length + childArm.tipLength;
        }
    }
}
