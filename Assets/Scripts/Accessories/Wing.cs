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
    /// 発生エフェクト
    /// </summary>
    [SerializeField]
    protected Effect effect;
    /// <summary>
    /// エフェクト発生間隔
    /// </summary>
    [SerializeField]
    protected int effectInterval;

    public override void Update()
    {
        base.Update();

        string effectCountName = "wingEffect";
        var speed = parentMaterial.nowSpeed.magnitude;
        if (speed != 0 && timer.get(effectCountName) % (int)(effectInterval / speed) == 0)
        {
            if (childParts != null)
            {
                if (childParts.childParts != null)
                {
                    childParts.childParts.outbreakEffect(effect);
                }
                else
                {
                    childParts.outbreakEffect(effect);
                }
            }
            else
            {
                outbreakEffect(effect);
            }
            timer.reset(effectCountName);
        }
    }

    /// <summary>
    ///付属パーツ系の基本動作
    /// </summary>
    public override void accessoryMotion(Vector2 setVector, float correctionAngle = 0)
    {
        if (childParts != null)
        {
            Vector2 addVector = Vector2.right * setVector.y * -1 / 120
                + Vector2.up * setVector.x * (parentMaterial.widthPositive ? 1 : -1) / 120;

            nowPosition = Vector2.right
               * ((addVector.x != 0) ? nowPosition.x + addVector.x : nowPosition.x * 9 / 10)
               + Vector2.up
               * ((addVector.y != 0) ? nowPosition.y + addVector.y : nowPosition.y * 9 / 10);
            Quaternion correctionRotation = Quaternion.Euler(0, 0, correctionAngle);

            nowPosition = MathV.Min(nowPosition, limitRange);
            setManipulatePosition(correctionRotation * (baseVector + nowPosition), false);
        }
    }
}
