using UnityEngine;
using System.Collections;
using System.Linq;

public class TestStage : Stage
{
    protected override IEnumerator stageAction()
    {
        var enemyCount = enemyList.Count - 1;
        var interval = 1200;

        setEnemy(0, 1, 0);

        yield return wait(interval, () => !allEnemies.Any());

        setEnemy(1, 1.2f, 0);
        setEnemy(0, 1, 0.3f);
        setEnemy(0, 1, -0.3f);

        yield return wait(interval, () => !allEnemies.Any());

        setEnemy(0, 1, 0);
        setEnemy(1, -1.2f, 0.8f, -10);
        setEnemy(1, -1.2f, -0.8f, 10);

        yield return wait(() => !allEnemies.Any());

        setEnemy(enemyCount, 1, 0, levelCorrection: 12);
        yield break;
    }
}
