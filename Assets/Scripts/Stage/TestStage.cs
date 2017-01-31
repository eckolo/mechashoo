using UnityEngine;
using System.Collections;

public class TestStage : Stage
{
    protected override IEnumerator stageAction()
    {
        int enemyCount = enemyList.Count;
        setEnemy(Random.Range(0, enemyCount), new Vector2(0, Random.value));
        for(int index = 0; index < 2; index++)
        {
            yield return wait(1000 - (int)stageLevel++);
            setEnemy(Random.Range(0, enemyCount), new Vector2(0, Random.value));
        }
    }
}
