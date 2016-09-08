using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Menu : Stage
{
    public override void startStageAction()
    {
        StartCoroutine(nowStageAction = stageAction());
    }

    protected override IEnumerator stageAction()
    {
        List<string> stageNames = new List<string>();
        var stages = mainSystem.stages;
        for (int i = 0; i < stages.Count; i++)
            if (stages[i].ableChoice)
                stageNames.Add(!stages[i].isSystem ? stages[i].stageName : "");
        yield return getChoices(stageNames, setPosition: new Vector2(-240, 180));
        mainSystem.nextStageNum = lastSelected ?? 0;

        stopStageAction();

        yield break;
    }
}
