using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Menu : Stage
{
    static Vector2 menuPosition = new Vector2(-240, 180);

    class MenuState
    {
        public string text;
        public bool ableChoice;
    }
    static MenuState getMenu(string text, bool ableChoice)
    {
        return new MenuState
        {
            text = text,
            ableChoice = ableChoice
        };
    }
    List<MenuState> mainMenus = new List<MenuState>
    {
        getMenu("戦場選択",true),
        getMenu("機体選択",true)
    };

    public override void startStageAction()
    {
        StartCoroutine(nowStageAction = stageAction());
    }

    protected override IEnumerator stageAction()
    {
        yield return mainMuneAction();

        stopStageAction();
        yield break;
    }

    protected IEnumerator mainMuneAction()
    {
        List<string> menus = new List<string>();
        for (int i = 0; i < mainMenus.Count; i++)
            menus.Add(mainMenus[i].ableChoice ? mainMenus[i].text : "");
        yield return getChoices(menus, setPosition: menuPosition);

        var selectedAction = lastSelected % mainMenus.Count;
        IEnumerator nextAction = stageAction();
        switch (selectedAction)
        {
            case 0:
                nextAction = goNextStage();
                break;
            default:
                break;
        }

        yield return nextAction;
        yield break;
    }

    protected IEnumerator goNextStage()
    {
        List<string> stageNames = new List<string>();
        var stages = mainSystem.stages;
        for (int i = 0; i < stages.Count; i++)
            stageNames.Add(stages[i].ableChoice && !stages[i].isSystem ? stages[i].stageName : "");
        yield return getChoices(stageNames, setPosition: menuPosition);

        if (lastSelected >= 0)
        {
            mainSystem.nextStageNum = lastSelected ?? 0;
            stopStageAction();
        }
        else
        {
            yield return mainMuneAction();
        }

        yield break;
    }
}
