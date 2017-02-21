using UnityEngine;
using System.Collections;
using System.Linq;

public class ExerciseStage : Stage
{
    protected override IEnumerator stageAction()
    {
        while(enemyList.Any())
        {
            setEnemy(0, 1.2f, 0);

            yield return wait(() => !allEnemies.Any());
        }
    }

    protected override bool isComplete
    {
        get {
            return false;
        }
    }
}
