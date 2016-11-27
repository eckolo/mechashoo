using UnityEngine;
using System.Collections;

public class TestStage : Stage {
    protected override IEnumerator stageAction() {
        setEnemy(enemyList[0].GetComponent<Npc>(), new Vector2(0, Random.value));
        yield return wait(1000 - (int)stageLevel++);

        yield return stageAction();
    }
}
