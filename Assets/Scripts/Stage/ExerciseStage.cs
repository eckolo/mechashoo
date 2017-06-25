using UnityEngine;
using System.Collections;
using System.Linq;

public class ExerciseStage : Stage
{
    protected override IEnumerator StageAction()
    {
        while(enemyList.Any())
        {
            SetEnemy(0, 1.2f, 0);

            yield return Wait(() => !allEnemies.Any());
        }
    }

    protected override bool isComplete
    {
        get {
            return false;
        }
    }
}
