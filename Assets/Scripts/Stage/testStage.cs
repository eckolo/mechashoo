using UnityEngine;
using System.Collections;
using System.Linq;

public class TestStage : Stage
{
    protected override IEnumerator stageAction()
    {
        var limit = 6;
        var enemyCount = enemyList.Count - 1;
        for(int index = 0; index < limit; index++)
        {
            yield return wait(720);
            setEnemy(Random.Range(0, enemyCount), new Vector2(0, (float)index / limit));
        }
        while(allEnemies.Any()) yield return wait(1);
        var boss = setEnemy(enemyCount, new Vector2(0, 0.5f));
    }
}
