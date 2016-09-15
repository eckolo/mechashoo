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
        getMenu("機体選択",selectShip,true),
        getMenu("設定変更",config,true)
    };

    public override void startStageAction()
    {
        StartCoroutine(nowStageAction = stageAction());
    }

    static void judgeMainMenuChoiceable()
    {
        foreach (var mainMenu in mainMenus)
        {
            if (mainMenu.action == goNextStage) mainMenu.ableChoice = !sysPlayer.isInitialState;
        }
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
        judgeMainMenuChoiceable();

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

        if (Sys.shipDataMylist.Count == 0) foreach (var ship in Sys.possessionShips) Sys.shipDataMylist.Add(ship.coreData);

        List<string> ships = new List<string>();
        for (var i = 0; i < Sys.shipDataMylist.Count; i++) ships.Add(Sys.shipDataMylist[i].name);

        yield return getChoices(ships,
            selectedAction: i => sysPlayer.setCoreStatus(Sys.shipDataMylist[i]),
            setPosition: Vector2.down * 90,
            maxChoices: 3,
            ableCancel: true);
        if (lastSelected < 0) sysPlayer.setCoreStatus(keepSipData);

        transparentPlayer();
        yield break;
    }

    static IEnumerator config()
    {
        var keepVolumeBGM = volumeBGM;
        var keepVolumeSE = volumeSE;

        var counfigMenus = new List<string> { "背景音 音量", "効果音 音量" };
        yield return getChoices(counfigMenus,
            selectedAction: i => configChoiceAction(i),
            horizontalAction: (i, h, f) => configHorizontalAction(i, h),
            horizontalBarrage: true,
            horizontalInterval: 1,
            setPosition: Vector2.down * 90,
            ableCancel: true);

        if (lastSelected < 0)
        {
            volumeBGM = keepVolumeBGM;
            volumeSE = keepVolumeSE;
        }

        deleteSysText("volume");
        yield break;
    }

    static void configChoiceAction(int selected)
    {
        switch (selected)
        {
            case 0:
                setSysText("音量\r\n" + volumeBGM, "volume", new Vector2(160, -120));
                break;
            case 1:
                setSysText("音量\r\n" + volumeSE, "volume", new Vector2(160, -120));
                break;
            default:
                break;
        }
    }
    static void configHorizontalAction(int selected, bool horizontal)
    {
        switch (selected)
        {
            case 0:
                volumeBGM = Mathf.Clamp(volumeBGM + (horizontal ? 1 : -1), minVolume, maxVolume);
                break;
            case 1:
                volumeSE = Mathf.Clamp(volumeSE + (horizontal ? 1 : -1), minVolume, maxVolume);
                break;
            default:
                break;
        }
    }
}
