using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Events;

public class Menu : Stage
{
    static Vector2 menuPosition = Vector2.zero;
    static IEnumerator nextAction = null;
    delegate IEnumerator Action();

    class MenuState
    {
        public MenuState(Action _action, string _text, bool _ableChoice = true)
        {
            action = _action;
            text = _text;
            ableChoice = _ableChoice;
        }
        public string text { set; get; }
        public bool ableChoice { set; get; }
        public Action action { private set; get; }
    }
    static List<MenuState> mainMenus = new List<MenuState>
    {
        new MenuState(goNextStage,"戦場選択"),
        new MenuState(manageShip,"機体整備"),
        new MenuState(config,"設定変更")
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
        menuPosition = MathV.scaling(screenSize / 2, new Vector2(-1, 1));

        yield return mainMuneAction();
        while (nextAction != null)
        {
            visualizePlayer();
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

        int selected = 0;
        yield return getChoices(getChoicesList(mainMenus,
            menu => menu.ableChoice ? menu.text : ""),
            endProcess: result => selected = result,
            setPosition: menuPosition,
            pibot: TextAnchor.UpperLeft);

        nextAction = mainMenus[selected % mainMenus.Count].action();

        yield break;
    }

    static IEnumerator goNextStage()
    {
        transparentPlayer();

        int selected = 0;
        yield return getChoices(getChoicesList(Sys.stages,
            stage => stage.ableChoice && !stage.isSystem ? stage.displayName : ""),
            endProcess: result => selected = result,
            setPosition: menuPosition,
            pibot: TextAnchor.UpperLeft,
            ableCancel: true);

        if (selected >= 0)
        {
            Sys.nextStageNum = selected;
            nextAction = null;
        }

        yield break;
    }

    static IEnumerator manageShip()
    {
        var shipMenus = new List<string>();
        shipMenus.Add("機体設計");
        if (Sys.shipDataMylist.Count > 0) shipMenus.Add("設計書管理");
        int selected = 0;
        yield return getChoices(shipMenus,
            endProcess: result => selected = result,
            setPosition: menuPosition,
            pibot: TextAnchor.UpperLeft,
            ableCancel: true);

        switch (selected)
        {
            case 0:
                nextAction = manageShipDirect();
                break;
            case 1:
                nextAction = manageShipBlueprint();
                break;
            default:
                break;
        }

        yield break;
    }
    static IEnumerator manageShipDirect()
    {
        nextAction = manageShip();

        var shipMenus = new List<string> { "組立", "設計図記録" };
        int selected = 0;
        yield return getChoices(shipMenus,
            endProcess: result => selected = result,
            setPosition: menuPosition,
            pibot: TextAnchor.UpperLeft,
            ableCancel: true);

        switch (selected)
        {
            case 0:
                yield return constructionShip(
                    sysPlayer.coreData,
                    coreData => sysPlayer.setCoreStatus(coreData)
                    );
                break;
            case 1:
                nextAction = manageShipBlueprint();
                break;
            default:
                break;
        }

        yield break;
    }
    static IEnumerator manageShipBlueprint()
    {
        nextAction = manageShip();

        var shipMenus = getChoicesList(Sys.shipDataMylist, shipData => shipData.name);
        shipMenus.Add("新規設計図作成");

        int selected = 0;
        yield return getChoices(shipMenus,
            endProcess: result => selected = result,
            setPosition: menuPosition,
            pibot: TextAnchor.UpperLeft,
            ableCancel: true);

        if (selected >= 0) yield return constructionShip(
            Sys.shipDataMylist[selected],
            coreData => Sys.shipDataMylist[selected] = coreData
            );

        yield break;
    }
    static IEnumerator constructionShip(Ship.CoreData originData, UnityAction<Ship.CoreData> endProcess)
    {
        var returnData = originData;

        if (Sys.shipDataMylist.Count == 0) foreach (var ship in Sys.possessionShips) Sys.shipDataMylist.Add(ship.coreData);

        List<string> ships = new List<string>();
        for (var i = 0; i < Sys.shipDataMylist.Count; i++) ships.Add(Sys.shipDataMylist[i].name);

        int selected = 0;
        yield return getChoices(ships,
            endProcess: result => selected = result,
            selectedAction: i => sysPlayer.setCoreStatus(Sys.shipDataMylist[i]),
            setPosition: menuPosition,
            pibot: TextAnchor.UpperLeft,
            maxChoices: 3,
            ableCancel: true);

        returnData = Sys.shipDataMylist[selected];
        endProcess(returnData);
        yield break;
    }

    static IEnumerator config()
    {
        transparentPlayer();

        var keepVolumeBGM = volumeBGM;
        var keepVolumeSE = volumeSE;

        var counfigMenus = new List<string> { "背景音 音量", "効果音 音量" };
        int selected = 0;
        yield return getChoices(counfigMenus,
            endProcess: result => selected = result,
            selectedAction: i => configChoiceAction(i),
            horizontalAction: (i, h, f) => configHorizontalAction(i, h),
            horizontalBarrage: true,
            horizontalInterval: 1,
            setPosition: menuPosition,
            pibot: TextAnchor.UpperLeft,
            ableCancel: true);

        if (selected < 0)
        {
            volumeBGM = keepVolumeBGM;
            volumeSE = keepVolumeSE;
        }

        deleteSysText("volume");
        yield break;
    }
    static void configChoiceAction(int selected)
    {
        Vector2 setVector = MathV.scaling(screenSize / 2, new Vector2(1, -1)) + menuPosition
            + Vector2.right * screenSize.x / 3
            - Vector2.up * defaultTextSize * 1.2f;

        switch (selected)
        {
            case 0:
                setSysText("音量\r\n" + volumeBGM, "volume", setVector);
                break;
            case 1:
                setSysText("音量\r\n" + volumeSE, "volume", setVector);
                break;
            default:
                deleteSysText("volume");
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
