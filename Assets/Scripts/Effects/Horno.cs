using UnityEngine;
using System.Collections;

public class Horno : Effect
{
    /// <summary>
    /// 消滅までのフレーム数
    /// </summary>
    [SerializeField]
    protected int destroyLimit = 120;
    /// <summary>
    /// 最大サイズ
    /// </summary>
    [SerializeField]
    protected float maxSize = 2;
    /// <summary>
    /// 合計回転角度
    /// </summary>
    [SerializeField]
    protected float maxAngle = 600;
    /// <summary>
    /// 付属エフェクト画像
    /// </summary>
    [SerializeField]
    protected Sprite lightBeam = null;

    protected override IEnumerator Motion(int actionNum)
    {
        var halfDestroyLimit = destroyLimit / 2;
        var lightBeam1 = InjectObject<Effect>($"{displayName}:lightBeam1");
        var lightBeam2 = InjectObject<Effect>($"{displayName}:lightBeam2");
        lightBeam1.nowSprite = lightBeam;
        lightBeam2.nowSprite = lightBeam;
        lightBeam1.nowParent = transform;
        lightBeam2.nowParent = transform;
        lightBeam1.position = Vector2.zero;
        lightBeam2.position = Vector2.zero;
        lightBeam1.nowMaterial = nowMaterial;
        lightBeam2.nowMaterial = nowMaterial;

        for(int time = 0; time < destroyLimit; time++)
        {
            var behind = time < halfDestroyLimit;
            var halfTime = behind ? time : time - halfDestroyLimit;
            var halfLimit = behind ? halfDestroyLimit - 1 : destroyLimit - halfDestroyLimit - 1;

            nowScale = behind ?
                Vector2.one * maxSize * Easing.quadratic.Out(halfTime, halfLimit) :
                Vector2.one * maxSize * Easing.quadratic.SubOut(halfTime, halfLimit);

            var lightBeamScaleX = behind ?
                Easing.quadratic.In(halfTime, halfLimit) :
                Easing.quintic.SubIn(halfTime, halfLimit);
            var lightBeamScaleY = Easing.quadratic.SubOut(time, destroyLimit);
            var lightBeamScale = new Vector2(lightBeamScaleX * 3, lightBeamScaleY / 2) * maxSize;
            lightBeam1.nowScale = lightBeamScale;
            lightBeam2.nowScale = lightBeamScale;

            var lightBeamAngle = Easing.quadratic.InOut(maxAngle, time, destroyLimit - 1);
            lightBeam1.SetAngle(lightBeamAngle - 45);
            lightBeam2.SetAngle(lightBeamAngle + 45);

            nowAlpha = behind ?
                Easing.quadratic.Out(halfTime, halfLimit) :
                Easing.quadratic.SubOut(halfTime, halfLimit);
            var lightBeamAlpha = behind ?
                Easing.quintic.Out(halfTime, halfLimit) :
                Easing.quintic.SubIn(halfTime, halfLimit);
            lightBeam1.nowAlpha = lightBeamAlpha;
            lightBeam2.nowAlpha = lightBeamAlpha;

            yield return Wait(1);
        }

        lightBeam1.DestroyMyself();
        lightBeam2.DestroyMyself();
        DestroyMyself();
        yield break;
    }
}