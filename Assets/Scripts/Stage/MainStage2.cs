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

        yield return waitMessage(@"さて、そろそろ周辺警戒部隊の索敵範囲に入った頃だな。
まず外周部の警戒部隊を蹴散らしてくれ。
さくっと頼むぜ。");

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
        setEnemy(0, new Vector2(1.1f, 0.5f), 190, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.25f, 0.4f), 190, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.25f, -0.4f), 170, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, -0.5f), 170, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, -0.75f), 170, activityLimit: INTERVAL);

        yield return waitWave(INTERVAL);

        setEnemy(1, new Vector2(1.3f, 0), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, 0.3f), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, -0.3f), activityLimit: INTERVAL);

        yield return waitWave(INTERVAL);

        setEnemy(0, new Vector2(1.1f, 0));
        setEnemy(1, new Vector2(-1.3f, 0.8f), -10);
        setEnemy(1, new Vector2(-1.3f, -0.8f), 10);

        yield return wait(() => !allEnemies.Any());

        setEnemy(enemyCount, 1, 0, levelCorrection: 12);
        yield break;
    }
}
