using UnityEngine;
using System.Collections;

public class Leg : Accessory
{
    /// <summary>
    /// パーツモーションの基準角度
    /// </summary>
    [SerializeField]
    protected int baseAngle;
    /// <summary>
    /// 横移動に対する角度変動幅
    /// </summary>
    [SerializeField]
    protected int horizontalVariation;
    /// <summary>
    /// 縦移動に対する角度変動幅
    /// </summary>
    [SerializeField]
    protected int verticalVariation;
    /// <summary>
    /// 子パーツの角度変動幅
    /// </summary>
    [SerializeField]
    protected int childVariation;

    /// <summary>
    ///付属パーツ系の基本動作
    /// </summary>
    public override void accessoryMotion(Vector2 setVector, float correctionAngle = 0)
    {
        Ship.ShipData parentData = parentMaterial.GetComponent<Ship>().outputShiData();

        setAngle(baseAngle + horizontalVariation * setVector.x * (parentMaterial.widthPositive ? 1 : -1) / parentData.maxSpeed + verticalVariation * setVector.y / parentData.maxSpeed);
        childParts.setAngle(childVariation * (1 - setVector.magnitude / parentData.maxSpeed));
    }
}
