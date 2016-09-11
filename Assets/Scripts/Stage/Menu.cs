using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;

public class Menu : Stage
{
    static Vector2 menuPosition = new Vector2(-240, 180);
    static IEnumerator nextAction = null;
    delegate IEnumerator Action();

    class MenuState
    {
        public string text;
        public bool ableChoice;
        public Action action;
    }
    static MenuState getMenu(string text, Action action, bool ableChoice)
    {
        return new MenuState
        {
            text = text,
            action = action,
            ableChoice = ableChoice
        };
    }
    static List<MenuState> mainMenus = new List<MenuState>
    {
        getMenu("戦場選択",goNextStage,true),
        getMenu("機体選択",selectShip,true)
    };

    public override void startStageAction()
    {
        StartCoroutine(nowStageAction = stageAction());
    }

    protected override IEnumerator stageAction()
    {
        yield return mainMuneAction();
        while (nextAction != null)
        {
            var runAction = nextAction;
            nextAction = mainMuneAction();
            yield return runAction;
        }

        stopStageAction();
        yield break;
    }

    static IEnumerator mainMuneAction()
    {
        List<string> menus = new List<string>();
        for (int i = 0; i < mainMenus.Count; i++)
            menus.Add(mainMenus[i].ableChoice ? mainMenus[i].text : "");
        yield return getChoices(menus, setPosition: menuPosition);

        nextAction = mainMenus[lastSelected ?? 0 % mainMenus.Count].action();

        yield break;
    }

    static IEnumerator goNextStage()
    {
        List<string> stageNames = new List<string>();
        var stages = Sys.stages;
        for (int i = 0; i < stages.Count; i++)
            stageNames.Add(stages[i].ableChoice && !stages[i].isSystem ? stages[i].stageName : "");

        yield return getChoices(stageNames,
            setPosition: menuPosition,
            ableCancel: true);

        if (lastSelected >= 0)
        {
            Sys.nextStageNum = lastSelected ?? 0;
            nextAction = null;
        }

        yield break;
    }

    static IEnumerator selectShip()
    {
        setPlayer();

        var keepSipData = sysPlayer.coreData;

        List<string> ships = new List<string>();
        for (var i = 0; i < Sys.selectShip.Count; i++) ships.Add(Sys.selectShip[i].gameObject.name);

        yield return getChoices(ships,
            action: i => sysPlayer.setCoreStatus(Sys.selectShip[i]),
            setPosition: Vector2.down * 90,
            ableCancel: true);
        if (lastSelected < 0) sysPlayer.setCoreStatus(keepSipData);

        //transparentPlayer();
        yield break;
    }
}
