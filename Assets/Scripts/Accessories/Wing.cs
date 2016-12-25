using UnityEngine;
using System.Collections;

public class Wing : Reactor
{
    /// <summary>
    /// 本体の回転可否フラグ
    /// </summary>
    [SerializeField]
    public bool rollable = false;
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

    protected override IEnumerator motion(int actionNum)
    {
        for(int time = 0; true; time++)
        {
            var speed = parentMaterial.nowSpeed.magnitude;
            if(speed != 0 && time % (int)(effectInterval / speed) == 0)
            {
                Parts effectRoot = childParts != null
                    ? grandsonParts != null
                    ? grandsonParts
                    : childParts
                    : this;
                effectRoot.outbreakEffect(effect, baseEffectScale);
            }
            yield return wait(1);
        }
    }

    /// <summary>
    ///付属パーツ系の基本動作
    /// </summary>
    public override void accessoryMotion(Vector2 setVector, float correctionAngle = 0)
    {
        if(childParts != null)
        {
            Vector2 addVector = Vector2.right * setVector.y * -1 / 120
                + Vector2.up * setVector.x * parentMaterial.nWidthPositive / 120;

            nowPosition = Vector2.right
               * ((addVector.x != 0) ? nowPosition.x + addVector.x : nowPosition.x * 9 / 10)
               + Vector2.up
               * ((addVector.y != 0) ? nowPosition.y + addVector.y : nowPosition.y * 9 / 10);
            var correctionRotation = Quaternion.Euler(0, 0, correctionAngle);

            nowPosition = MathV.min(nowPosition, limitRange);
            setManipulator(correctionRotation * (baseVector + nowPosition), false);

            if(grandsonParts != null) grandsonParts.setAngle(-childParts.nowLocalAngle);
        }
    }
}
