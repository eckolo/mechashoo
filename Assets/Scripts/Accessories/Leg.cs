using UnityEngine;
using System.Collections;

public class Leg : Accessory
{
    /// <summary>
    /// パーツモーションの基準位置
    /// </summary>
    [SerializeField]
    protected Vector2 baseVector;
    /// <summary>
    /// パーツモーションの現在位置
    /// </summary>
    protected Vector2 nowPosition;
    /// <summary>
    /// パーツモーションの現在位置
    /// </summary>
    [SerializeField]
    protected float limitRange;

    /// <summary>
    ///付属パーツ系の基本動作
    /// </summary>
    public override void accessoryMotion(Vector2 setVector, float correctionAngle = 0)
    {
        Ship.ShipData parentData = parentMaterial.GetComponent<Ship>().outputShiData();

        setAngle(-90 * (1 + setVector.x / parentData.maxSpeed));
        childParts.setAngle(-45 * (1 - setVector.magnitude / parentData.maxSpeed));
    }
}
