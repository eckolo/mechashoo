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
    /// 発生エフェクト
    /// </summary>
    [SerializeField]
    protected Effect effect;
    /// <summary>
    /// エフェクト発生間隔
    /// </summary>
    [SerializeField]
    protected int effectInterval;

    protected override IEnumerator motion(int actionNum)
    {
        for (int time = 0; true; time++)
        {
            var speed = parentMaterial.nowSpeed.magnitude;
            if (time % (int)(effectInterval / (speed + 1)) == 0)
            {
                Vector2 setPosition = childParts.transform.rotation * -childParts.selfConnection;
                outbreakEffect(effect, baseEffectScale, setPosition);
            }
            yield return wait(1);
        }
    }

    /// <summary>
    ///付属パーツ系の基本動作
    /// </summary>
    public override void accessoryMotion(Vector2 setVector, float correctionAngle = 0)
    {
        Ship.CoreData parentData = parentMaterial.GetComponent<Ship>().coreData;

        setAngle(baseAngle + horizontalVariation * setVector.x * (parentMaterial.widthPositive ? 1 : -1) / parentData.palamates.maxSpeed + verticalVariation * setVector.y / parentData.palamates.maxSpeed);
        childParts.setAngle(childVariation * (1 - setVector.magnitude / parentData.palamates.maxSpeed));
    }
}
