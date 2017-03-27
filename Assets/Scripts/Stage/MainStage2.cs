using UnityEngine;
using System.Collections;
using System.Linq;

public class MainStage2 : Stage
{
    const int INTERVAL = 2400;

    protected override IEnumerator openingAction()
    {
        yield return sysPlayer.headingDestination(new Vector2(-3.6f, 0), sysPlayer.maximumSpeed);
        yield return sysPlayer.stoppingAction();

        yield return waitMessage(@"さて、そろそろ周辺警戒部隊の索敵範囲に入った頃だな。");
        yield return waitMessage(@"まず外周部の警戒部隊を蹴散らしてくれ。");
        yield return waitMessage(@"さくっと頼むぜ。");

        yield return base.openingAction();
    }

    protected override IEnumerator stageAction()
    {
        var enemyCount = enemyList.Count - 1;

        setEnemy(0, new Vector2(1.2f, 0.7f), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, 0.5f), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.2f, 0.3f), activityLimit: INTERVAL);

        yield return waitWave(INTERVAL);

        setEnemy(0, new Vector2(1.2f, -0.3f), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, -0.5f), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.2f, -0.7f), activityLimit: INTERVAL);

        yield return waitWave(INTERVAL);

        setEnemy(0, new Vector2(1.1f, 0.75f), 190, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, 0.5f), 190, onTheWay: false, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.25f, 0.4f), 190, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.25f, -0.4f), 170, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, -0.5f), 170, onTheWay: false, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, -0.75f), 170, activityLimit: INTERVAL);

        yield return waitWave(INTERVAL);
        yield return waitMessage(@"梅雨払いありがとよ、お疲れさん。");
        yield return waitMessage(@"…と、言いたとこだがそうも言えなくなっちまってな。");
        yield return waitMessage(@"強奪班連中がドジふんだんで、護衛部隊の本隊が追跡中だ。");
        yield return waitMessage(@"逃げてるやつらにゃこの宙域かすめるよう言ってある。");
        yield return waitMessage(@"ってなわけでそっちの掃除を頼みたいんだがな。
安心しな、報酬は割り増ししてやる。");
        yield return waitMessage(@"ほれ、お出ましだ。
気ぃ張って掛かれよ！");

        setEnemy(1, new Vector2(1.3f, 0), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, 0.3f), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, -0.3f), activityLimit: INTERVAL);

        yield return waitWave(INTERVAL);

        setEnemy(2, new Vector2(1.8f, 0), activityLimit: INTERVAL);
        setEnemy(1, new Vector2(1.3f, 0.3f), 190, activityLimit: INTERVAL);
        setEnemy(1, new Vector2(1.3f, -0.3f), 170, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, 0.6f), 200, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, -0.6f), 160, activityLimit: INTERVAL);

        yield return waitWave(INTERVAL);

        setEnemy(0, new Vector2(1.1f, 0));
        setEnemy(1, new Vector2(-1.3f, 0.8f), -10);
        setEnemy(1, new Vector2(-1.3f, -0.8f), 10);

        yield return wait(() => !allEnemies.Any());

        setEnemy(enemyCount, 1, 0, levelCorrection: 12);
        yield break;
    }
}
