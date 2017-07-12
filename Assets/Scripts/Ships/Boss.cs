using UnityEngine;
using System.Collections;

/// <summary>
/// ボスクラス敵機クラス
/// </summary>
public abstract class Boss : Npc
{
    protected override IEnumerator SinkingMotion()
    {
        var phaselimit = 24;
        var baseInterval = 36;
        for(var phase = 0; phase < phaselimit; phase++)
        {
            var setPosition = new Vector2(Random.Range(-spriteSize.x / 2, spriteSize.x / 2), Random.Range(-spriteSize.y / 2, spriteSize.y / 2));
            var sizeTweak = Easing.quadratic.Out(phase, phaselimit);
            OutbreakExplosion(sizeTweak, setPosition, 1);
            var explodInterval = Mathf.FloorToInt(Easing.quadratic.SubIn(baseInterval, phase, phaselimit));
            for(int time = 0; time < explodInterval; time++)
            {
                ExertPower(nowSpeed, reactPower + nowSpeed.magnitude, 0);
                yield return Wait(1);
            }
        }
        OutbreakExplosion(2, index: 2);
        OutbreakExplosion(2, index: 2);
        yield return Wait(180);
        yield return base.SinkingMotion();
        yield break;
    }
}
