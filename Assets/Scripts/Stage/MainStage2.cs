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

        yield return wait(INTERVAL / 10);

        yield return base.openingAction();
    }

    protected override IEnumerator stageAction()
    {
        var enemyCount = enemyList.Count - 1;

        setEnemy(0, new Vector2(1.1f, 0.7f), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1, 0.5f), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, 0.3f), activityLimit: INTERVAL);

        yield return wait(INTERVAL, () => !allEnemies.Any());

        setEnemy(0, new Vector2(1.1f, -0.3f), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1, -0.5f), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.1f, -0.7f), activityLimit: INTERVAL);

        yield return wait(INTERVAL, () => !allEnemies.Any());

        setEnemy(0, new Vector2(1f, 0.75f), 10, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1, 0.5f), 10, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.15f, 0.4f), 10, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1.15f, -0.4f), -10, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1, -0.5f), -10, activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1f, -0.75f), -10, activityLimit: INTERVAL);

        yield return wait(INTERVAL, () => !allEnemies.Any());

        setEnemy(1, new Vector2(1.2f, 0), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1, 0.3f), activityLimit: INTERVAL);
        setEnemy(0, new Vector2(1, -0.3f), activityLimit: INTERVAL);

        yield return wait(INTERVAL, () => !allEnemies.Any());

        setEnemy(0, new Vector2(1, 0));
        setEnemy(1, new Vector2(-1.2f, 0.8f), -10);
        setEnemy(1, new Vector2(-1.2f, -0.8f), 10);

        yield return wait(() => !allEnemies.Any());

        setEnemy(enemyCount, 1, 0, levelCorrection: 12);
        yield break;
    }
}
