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
            for(int _index = 0; _index < 720 && allEnemies.Any(); _index++) yield return wait(1);
            setEnemy(Random.Range(0, enemyCount), new Vector2(1, ((float)index / limit) * 2 + (1f / (limit * 2)) * 2 - 1));
        }
        while(allEnemies.Any()) yield return wait(1);
        var boss = setEnemy(enemyCount, new Vector2(1, 0), 72);
    }
}
