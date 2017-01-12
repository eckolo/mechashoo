using UnityEngine;
using System.Collections;

public class TestStage : Stage
{
    protected override IEnumerator stageAction()
    {
        setEnemy(1, new Vector2(0, Random.value));
        yield return wait(1000 - (int)stageLevel++);

        yield return stageAction();
    }
}
