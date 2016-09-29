using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Events;

public class Menu : Stage
{
    static Vector2 menuPosition = Vector2.zero;
    delegate IEnumerator Action(UnityAction<bool> endMenu);

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

        yield return mainMenuAction();

        stopStageAction();
        yield break;
    }

    static IEnumerator mainMenuAction()
    {
        bool endRoop = false;
        do
        {
            visualizePlayer();
            judgeMainMenuChoiceable();

            int selected = 0;
            yield return getChoices(getChoicesList(mainMenus,
                menu => menu.ableChoice ? menu.text : ""),
                endProcess: result => selected = result,
                setPosition: menuPosition,
                pibot: TextAnchor.UpperLeft);

            yield return mainMenus[selected % mainMenus.Count].action(result => endRoop = result);
        } while (!endRoop);

        yield break;
    }

    static IEnumerator goNextStage(UnityAction<bool> endMenu)
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
            endMenu(true);
        }

        yield break;
    }

    static IEnumerator manageShip(UnityAction<bool> endMenu)
    {
        bool endRoop = false;
        do
        {
            visualizePlayer();
            var shipMenus = new List<string> { "機体設計", "設計書管理" };
            int selected = 0;
            yield return getChoices(shipMenus,
                endProcess: result => selected = result,
                setPosition: menuPosition,
                pibot: TextAnchor.UpperLeft,
                ableCancel: true);

            switch (selected)
            {
                case 0:
                    yield return manageShipDirect();
                    break;
                case 1:
                    yield return manageShipBlueprint();
                    break;
                default:
                    endRoop = true;
                    break;
            }
        } while (!endRoop);

        yield break;
    }
    static IEnumerator manageShipDirect()
    {
        bool endRoop = false;
        do
        {
            visualizePlayer();
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
                        Sys.adoptedShipData,
                        coreData => Sys.adoptedShipData = coreData
                        );
                    break;
                case 1:
                    yield return manageShipBlueprint();
                    break;
                default:
                    endRoop = true;
                    break;
            }
        } while (!endRoop);

        yield break;
    }
    static IEnumerator manageShipBlueprint(Ship.CoreData setData = null)
    {
        bool endRoop = false;
        do
        {
            visualizePlayer();
            var choices = getChoicesList(Sys.shipDataMylist, shipData => shipData.name);
            choices.Add("新規設計図作成");

            int selected = 0;
            yield return getChoices(choices,
                endProcess: result => selected = result,
                setPosition: menuPosition,
                pibot: TextAnchor.UpperLeft,
                ableCancel: true);

            if (selected < 0) endRoop = true;
            else
            {
                if (selected >= choices.Count) Sys.shipDataMylist.Add(null);
                int listNum = Mathf.Min(selected, Sys.shipDataMylist.Count - 1);

                if (setData == null) yield return constructionShip(setData, coreData => setData = coreData);
                Sys.shipDataMylist[listNum] = setData;
            }
        } while (!endRoop);

        yield break;
    }
    static IEnumerator constructionShip(Ship.CoreData originData, UnityAction<Ship.CoreData> endProcess)
    {
        var resultData = originData;
        bool endRoop = false;

        do
        {
            sysPlayer.setCoreStatus(resultData);
            visualizePlayer();
            var choices = new List<string> {
                "機体選択",
                resultData != null ? "武装選択" : "",
                "確定"
            };
            int selected = 0;
            yield return getChoices(choices,
                endProcess: result => selected = result,
                setPosition: menuPosition,
                pibot: TextAnchor.UpperLeft,
                ableCancel: true);

            switch (selected)
            {
                case 0:
                    yield return constructionShipBody(ship => resultData = ship.coreData.setWeaponData());
                    break;
                case 1:
                    yield return constructionShipWeapon(resultData.weaponSlots, weapon => resultData.setWeaponData(selected, weapon));
                    break;
                case 2:
                    endRoop = true;
                    break;
                default:
                    resultData = originData;
                    endRoop = true;
                    break;
            }

        } while (!endRoop);

        sysPlayer.setCoreStatus(Sys.adoptedShipData);
        endProcess(resultData);
        yield break;
    }
    static IEnumerator constructionShipBody(UnityAction<Ship> endProcess)
    {
        int selected = 0;
        yield return getChoices(getChoicesList(Sys.possessionShips,
            ship => ship.name),
            endProcess: result => selected = result,
            selectedAction: i => sysPlayer.setCoreStatus(Sys.possessionShips[i]),
            setPosition: menuPosition,
            pibot: TextAnchor.UpperLeft,
            ableCancel: true);

        if (selected >= 0) endProcess(Sys.possessionShips[selected]);
        yield break;
    }
    static IEnumerator constructionShipWeapon(List<Ship.WeaponSlot> slots, UnityAction<Weapon> endProcess)
    {
        int slotNum = 0;
        yield return getChoices(getChoicesList(slots, "接続孔", "番"),
            endProcess: result => slotNum = result,
            setPosition: menuPosition,
            pibot: TextAnchor.UpperLeft,
            ableCancel: true);

        if (slotNum >= 0)
        {
            int selected = 0;
            var choices = getChoicesList(Sys.possessionWeapons, weapon => weapon.name);
            choices.Add("武装解除");

            yield return getChoices(choices,
                endProcess: result => selected = result,
                selectedAction: i => sysPlayer.setWeaponData(slotNum, i < Sys.possessionWeapons.Count ? Sys.possessionWeapons[i] : null),
                setPosition: menuPosition,
                pibot: TextAnchor.UpperLeft,
                ableCancel: true);
            if (selected == choices.Count)
            {
                endProcess(null);
            }
            else if (selected >= 0) endProcess(Sys.possessionWeapons[selected]);
        }

        yield break;
    }

    static IEnumerator config(UnityAction<bool> endMenu)
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
