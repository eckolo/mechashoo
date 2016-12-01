using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Events;
using System.Linq;

public class Menu : Stage
{
    static Vector2 _menuPosition = Vector2.zero;
    static Vector2 menuPosition
    {
        get
        {
            if(nowChoicesData != null) return new Vector2(nowChoicesData.upperRight.x, _menuPosition.y);
            return _menuPosition;
        }
        set
        {
            _menuPosition = value;
        }
    }

    class MenuState
    {
        public MenuState(PublicAction<UnityAction<bool>> _action, string _text, bool _ableChoice = true)
        {
            action = _action;
            text = _text;
            ableChoice = _ableChoice;
        }
        public string text { get; set; }
        public bool ableChoice { get; set; }
        public PublicAction<UnityAction<bool>> action { get; private set; }
    }
    List<MenuState> mainMenus = new List<MenuState>();

    void judgeMainMenuChoiceable()
    {
        foreach(var mainMenu in mainMenus)
        {
            if(mainMenu.action == goNextStage) mainMenu.ableChoice = !sysPlayer.isInitialState;
        }
    }

    protected override IEnumerator stageAction()
    {
        menuPosition = MathV.scaling(screenSize / 2, new Vector2(-1, 1));

        yield return mainMenuAction();

        stopStageAction();
        yield break;
    }

    IEnumerator mainMenuAction()
    {
        mainMenus.Add(new MenuState(goNextStage, "戦場選択"));
        mainMenus.Add(new MenuState(manageShip, "機体整備"));
        mainMenus.Add(new MenuState(config, "設定変更"));

        int oldSelected = 0;
        bool animation = true;
        bool endLoop = false;
        do
        {
            visualizePlayer();
            judgeMainMenuChoiceable();

            int selected = 0;
            yield return getChoices(getChoicesList(mainMenus,
                menu => menu.ableChoice ? menu.text : ""),
                endProcess: result => selected = result,
                setPosition: menuPosition,
                pibot: TextAnchor.UpperLeft,
                setMotion: animation,
                initialSelected: oldSelected);

            animation = false;
            oldSelected = selected;
            yield return mainMenus[selected % mainMenus.Count].action(result => endLoop = result);
            deleteChoices(endLoop);
        } while(!endLoop);

        yield break;
    }

    IEnumerator goNextStage(UnityAction<bool> endMenu)
    {
        transparentPlayer();

        int selected = 0;
        yield return getChoices(getChoicesList(sys.stages,
            stage => stage.ableChoice && !stage.isSystem ? stage.displayName : ""),
            endProcess: result => selected = result,
            setPosition: menuPosition,
            pibot: TextAnchor.UpperLeft,
            ableCancel: true);

        if(selected >= 0)
        {
            sys.nextStageNum = selected;
            endMenu(true);
        }

        deleteChoices();
        yield break;
    }

    IEnumerator manageShip(UnityAction<bool> endMenu)
    {
        int oldSelected = 0;
        bool animation = true;
        bool endLoop = false;
        do
        {
            var shipMenus = new List<string> { "機体設計", "設計書管理" };
            int selected = 0;
            yield return getChoices(shipMenus,
                endProcess: result => selected = result,
                setPosition: menuPosition,
                pibot: TextAnchor.UpperLeft,
                ableCancel: true,
                setMotion: animation,
                initialSelected: oldSelected);

            animation = false;
            oldSelected = selected;
            switch(selected)
            {
                case 0:
                    yield return manageShipDirect();
                    break;
                case 1:
                    yield return manageShipBlueprint();
                    break;
                default:
                    endLoop = true;
                    break;
            }
            deleteChoices(endLoop);
        } while(!endLoop);

        yield break;
    }
    IEnumerator manageShipDirect()
    {
        int oldSelected = 0;
        bool animation = true;
        bool endLoop = false;
        do
        {
            visualizePlayer();
            var shipMenus = new List<string> {
                "組立",
                sys.adoptedShipData != null ? "設計図へ記録" : "",
                sys.shipDataMylist.Count > 0 ? "設計図を反映" : ""
            };
            int selected = 0;
            yield return getChoices(shipMenus,
                endProcess: result => selected = result,
                setPosition: menuPosition,
                pibot: TextAnchor.UpperLeft,
                ableCancel: true,
                setMotion: animation,
                initialSelected: oldSelected);

            animation = false;
            oldSelected = selected;
            switch(selected)
            {
                case 0:
                    yield return constructionShip(
                        sys.adoptedShipData,
                        coreData => sys.adoptedShipData = coreData
                        );
                    break;
                case 1:
                    yield return manageShipBlueprint(sys.adoptedShipData);
                    break;
                case 2:
                    int resultIndex = -1;
                    yield return selectBlueprint(result => resultIndex = result, oldSelected, createNew: false);
                    if(resultIndex >= 0) sys.adoptedShipData = sys.shipDataMylist[resultIndex];
                    deleteChoices();
                    break;
                default:
                    endLoop = true;
                    break;
            }
            deleteChoices(endLoop);
        } while(!endLoop);

        yield break;
    }
    IEnumerator manageShipBlueprint(Ship.CoreData originCoreData = null)
    {
        int oldSelected = 0;
        var setData = originCoreData != null ? originCoreData.myself : null;
        bool animation = true;
        bool endLoop = false;
        do
        {
            int setNum = 0;
            visualizePlayer();
            yield return selectBlueprint(result => setNum = result, oldSelected, animation);

            animation = false;
            oldSelected = setNum;
            if(setNum < 0) endLoop = true;
            else
            {
                var originData = setNum < sys.shipDataMylist.Count
                    ? sys.shipDataMylist[setNum]
                    : null;

                if(originCoreData == null) yield return constructionShip(originData, coreData => setData = coreData);

                if(setData != null && setData.isCorrect)
                {
                    if(setNum >= sys.shipDataMylist.Count) sys.shipDataMylist.Add(null);
                    int listNum = Mathf.Min(setNum, Mathf.Max(sys.shipDataMylist.Count - 1, 0));
                    sys.shipDataMylist[listNum] = setData;
                }
            }
            deleteChoices(endLoop);
        } while(!endLoop);

        yield break;
    }
    IEnumerator selectBlueprint(UnityAction<int> endProcess, int oldSelected = 0, bool animation = true, bool createNew = true)
    {
        var originData = sysPlayer.coreData;
        var dataList = sys.shipDataMylist;
        var choices = getChoicesList(dataList, "設計図", "番");
        if(createNew) choices.Add("新規設計図作成");

        int selected = 0;
        yield return getChoices(choices,
            endProcess: result => selected = result,
            selectedProcess: (num, c) => sysPlayer.coreData = num < dataList.Count ? dataList[num] : null,
            setPosition: menuPosition,
            pibot: TextAnchor.UpperLeft,
            ableCancel: true,
            setMotion: animation,
            initialSelected: oldSelected);

        endProcess(selected);
        sysPlayer.coreData = originData;
        yield break;
    }
    IEnumerator constructionShip(Ship.CoreData originData, UnityAction<Ship.CoreData> endProcess)
    {
        var resultData = originData;
        int oldSelected = 0;
        bool animation = true;
        bool endLoop = false;
        do
        {
            sysPlayer.coreData = resultData;
            var choices = new List<string> { "本体選択" };
            choices.Add(resultData != null ? "武装選択" : "");
            choices.Add(resultData != null && resultData.isCorrect ? "確定" : "");

            int selected = 0;
            yield return getChoices(choices,
                endProcess: result => selected = result,
                setPosition: menuPosition,
                pibot: TextAnchor.UpperLeft,
                ableCancel: true,
                setMotion: animation,
                initialSelected: oldSelected);

            animation = false;
            oldSelected = selected;
            switch(selected)
            {
                case 0:
                    yield return constructionShipBody(resultData, ship => resultData = ship.setWeapon());
                    break;
                case 1:
                    yield return constructionShipWeapon(resultData.weaponSlots, (index, weapon) => resultData.setWeapon(index, weapon));
                    break;
                case 2:
                    endLoop = true;
                    break;
                default:
                    resultData = originData;
                    endLoop = true;
                    break;
            }

            deleteChoices(endLoop);
        } while(!endLoop);

        sysPlayer.coreData = sys.adoptedShipData;
        endProcess(resultData);
        yield break;
    }
    IEnumerator constructionShipBody(Ship.CoreData originData, UnityAction<Ship.CoreData> endProcess)
    {
        var choices = getChoicesList(sys.possessionShips, ship => ship.name);
        choices.Insert(0, originData != null ? originData.name : "");

        int selected = 0;
        yield return getChoices(choices,
            endProcess: result => selected = result,
            selectedProcess: (num, c) => sysPlayer.coreData = num == 0 ? originData : sys.possessionShips[num - 1].coreData.setWeapon(),
            setPosition: menuPosition,
            pibot: TextAnchor.UpperLeft,
            ableCancel: true);

        if(selected == 0) endProcess(originData);
        else if(selected >= 0) endProcess(sys.possessionShips[selected - 1].coreData);
        deleteChoices();
        yield break;
    }
    IEnumerator constructionShipWeapon(List<Ship.WeaponSlot> slots, UnityAction<int, Weapon> endProcess)
    {
        int oldSelected = 0;
        bool animation = true;
        bool endLoop = false;
        do
        {
            int slotNum = 0;
            yield return getChoices(getChoicesList(slots, "接続孔", "番"),
                endProcess: result => slotNum = result,
                setPosition: menuPosition,
                pibot: TextAnchor.UpperLeft,
                ableCancel: true,
                setMotion: animation,
                initialSelected: oldSelected);

            animation = false;
            oldSelected = slotNum;
            if(slotNum >= 0)
            {
                int selected = 0;
                var originWeapon = slots[slotNum].entity;
                var choices = getChoicesList(sys.possessionWeapons, weapon => weapon.name);
                choices.Insert(0, originWeapon != null ? originWeapon.name : "");
                choices.Add("武装解除");

                yield return getChoices(choices,
                    endProcess: result => selected = result,
                    selectedProcess: (num, c) => sysPlayer.setWeapon(slotNum, num == 0
                    ? originWeapon
                    : num - 1 < sys.possessionWeapons.Count
                    ? sys.possessionWeapons[num - 1]
                    : null),
                    setPosition: menuPosition,
                    pibot: TextAnchor.UpperLeft,
                    ableCancel: true,
                    initialSelected: originWeapon != null ? 0 : choices.Count - 1);

                if(selected > sys.possessionWeapons.Count) endProcess(slotNum, null);
                else if(selected > 0) endProcess(slotNum, sys.possessionWeapons[selected - 1]);
                deleteChoices();
            }
            else
                endLoop = true;

            deleteChoices(endLoop);
        } while(!endLoop);
        yield break;
    }

    IEnumerator config(UnityAction<bool> endMenu)
    {
        transparentPlayer();

        var keepVolumeBGM = Volume.bgm;
        var keepVolumeSE = Volume.se;

        var counfigMenus = new List<string> { "背景音 音量", "効果音 音量" };
        int selected = 0;
        yield return getChoices(counfigMenus,
            endProcess: result => selected = result,
            selectedProcess: (i, c) => configChoiceAction(i, c.upperRight + MathV.scaling(viewSize, baseMas.x / 2, baseMas.y / -2)),
            horizontalProcess: (i, h, f, c) => configHorizontalAction(i, h, c.upperRight + MathV.scaling(viewSize, baseMas.x / 2, baseMas.y / -2)),
            horizontalBarrage: true,
            horizontalInterval: 1,
            setPosition: menuPosition,
            pibot: TextAnchor.UpperLeft,
            ableCancel: true);

        if(selected < 0)
        {
            Volume.bgm = keepVolumeBGM;
            Volume.se = keepVolumeSE;
        }

        deleteSysText("volume");
        deleteChoices();
        yield break;
    }
    void configChoiceAction(int selected, Vector2 setVector)
    {
        switch(selected)
        {
            case 0:
                setSysText("音量\r\n" + Volume.bgm, "volume", setVector);
                break;
            case 1:
                setSysText("音量\r\n" + Volume.se, "volume", setVector);
                break;
            default:
                deleteSysText("volume");
                break;
        }
    }
    void configHorizontalAction(int selected, bool horizontal, Vector2 setVector)
    {
        switch(selected)
        {
            case 0:
                Volume.bgm = Mathf.Clamp(Volume.bgm + (horizontal ? 1 : -1), Volume.MIN, Volume.MAX);
                break;
            case 1:
                Volume.se = Mathf.Clamp(Volume.se + (horizontal ? 1 : -1), Volume.MIN, Volume.MAX);
                break;
            default:
                break;
        }
        configChoiceAction(selected, setVector);
    }
}
