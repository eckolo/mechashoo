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

        for(int index = 0; index < interval && allEnemies.Any(); index++) yield return wait(1);

        setEnemy(1, 1.2f, 0);
        setEnemy(0, 1, 0.3f);
        setEnemy(0, 1, -0.3f);

        for(int index = 0; index < interval && allEnemies.Any(); index++) yield return wait(1);

        setEnemy(0, 1, 0);
        setEnemy(1, 1.5f, 0.8f);
        setEnemy(1, 1.5f, -0.8f);

        while(allEnemies.Any()) yield return wait(1);

        setEnemy(enemyCount, new Vector2(1, 0), 12);
        yield break;
    }
}
