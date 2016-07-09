using UnityEngine;
using System.Collections;

public class testStage : Stage
{
    protected override IEnumerator stageAction()
    {
        setEnemy(enemyList[0].GetComponent<Npc>(), new Vector2(0, Random.value));
        yield return wait(720 - (int)stageLevel++);

        yield return stageAction();
    }
}
