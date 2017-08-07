using UnityEngine;
using System.Collections;
using System.Linq;

public class ChargingWeapon : Gun
{
    /// <summary>
    /// 最大溜め量
    /// </summary>
    [SerializeField]
    float maximumCharge = 100;
    /// <summary>
    /// 溜めエフェクトの最大サイズ
    /// </summary>
    [SerializeField]
    float maxChargeSize = 1;
    /// <summary>
    /// 現在の溜め量
    /// </summary>
    public float nowCharge { get; private set; } = 0;

    protected override IEnumerator BeginMotion(int actionNum)
    {
        if(!onTypeInjections.Any()) yield break;
        if(nowAction != ActionType.NOMAL && nowCharge < maximumCharge) yield break;
        if(nowAction == ActionType.NOMAL && nowCharge >= maximumCharge) yield break;
        var size = Vector2.one * maxChargeSize * Easing.quadratic.Out(nowCharge + 1, maximumCharge);
        yield return Charging(size);
        yield break;
    }

    protected override IEnumerator Motion(int actionNum)
    {
        if(nowAction == ActionType.NOMAL)
        {
            if(nowCharge < maximumCharge)
            {
                nowCharge++;
                yield return base.Motion(actionNum);
            }
        }
        else if(nowCharge >= maximumCharge)
        {
            yield return base.Motion(actionNum);
            nowCharge = 0;
        }
        yield break;
    }
}
