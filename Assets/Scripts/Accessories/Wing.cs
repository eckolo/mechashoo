using UnityEngine;
using System.Collections;

public class Wing : Accessory
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
    public override void accessoryMotion(float correctionAngle = 0)
    {
        nowPosition = Vector2.right
           * ((parentMaterial.nowSpeed.y != 0)
           ? nowPosition.x - parentMaterial.nowSpeed.y / 100
           : nowPosition.x * 9 / 10)
           + Vector2.up
           * ((parentMaterial.nowSpeed.x != 0)
           ? nowPosition.y + parentMaterial.nowSpeed.x * (parentMaterial.widthPositive ? 1 : -1) / 100
           : nowPosition.y * 9 / 10);
        Quaternion correctionRotation = Quaternion.Euler(0, 0, correctionAngle);

        nowPosition = MathV.Min(nowPosition, limitRange);
        setManipulatePosition(correctionRotation * (baseVector + nowPosition), false);
        Debug.Log(nowPosition);
    }
}
