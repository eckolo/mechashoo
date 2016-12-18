using UnityEngine;
using System.Collections;

public class ExerciseStage : Stage
{
    protected override IEnumerator stageAction()
    {
        yield return wait(1);

        yield return stageAction();
    }
}
