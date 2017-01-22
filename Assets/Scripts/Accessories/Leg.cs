using UnityEngine;
using System.Collections;

public class Leg : Reactor
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
        for(int time = 0; true; time++)
        {
            var speed = nowRoot.nowSpeed.magnitude;
            if(time % (int)(effectInterval / (speed + 1)) == 0 && childParts != null)
            {
                Vector2 setPosition = childParts.transform.rotation * -MathV.scaling(childParts.selfConnection, childParts.lossyScale);
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
        var maxSpeed = nowRoot.GetComponent<Ship>().maximumSpeed;

        setAngle(baseAngle + horizontalVariation * setVector.x * (nowRoot.widthPositive ? 1 : -1) / maxSpeed + verticalVariation * setVector.y / maxSpeed);
        if(childParts != null) childParts.setAngle(childVariation * (1 - setVector.magnitude / maxSpeed));
    }
}
