using UnityEngine;
using System.Collections;
using System.Linq;

public class TestStage : Stage
{
    protected override IEnumerator StageAction()
    {
        var enemyCount = enemyList.Count - 1;
        var interval = 2400;

        SetEnemy(0, 1, 0, activityLimit: interval);

        yield return Wait(interval, () => !allEnemies.Any());

        SetEnemy(1, 1.2f, 0, activityLimit: interval);
        SetEnemy(0, 1, 0.3f, activityLimit: interval);
        SetEnemy(0, 1, -0.3f, activityLimit: interval);

        yield return Wait(interval, () => !allEnemies.Any());

        SetEnemy(0, 1, 0);
        SetEnemy(1, -1.2f, 0.8f, -10);
        SetEnemy(1, -1.2f, -0.8f, 10);

        yield return Wait(() => !allEnemies.Any());

        SetEnemy(enemyCount, 1, 0, levelTweak: 12);
        yield break;
    }
}
