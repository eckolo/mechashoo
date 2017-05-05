using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Events;
using System.Linq;

public class Menu : Stage
{
    /// <summary>
    ///演習ステージ
    /// </summary>
    [SerializeField]
    private Stage exerciseStage = null;

    static Vector2 _menuPosition = Vector2.zero;
    static Vector2 menuPosition
    {
        get {
            if(nowChoicesData != null) return new Vector2(nowChoicesData.upperRight.x, _menuPosition.y);
            return _menuPosition;
        }
        set {
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
        foreach(var menu in mainMenus)
        {
            if(menu.action == goNextQuest) menu.ableChoice = !sysPlayer.isInitialState;
            if(menu.action == goExerciseStage) menu.ableChoice = !sysPlayer.isInitialState;
        }
    }

    protected override IEnumerator stageAction()
    {
        menuPosition = screenSize.scaling(new Vector2(-1, 1)) / 2;

        yield return fadein();
        yield return mainMenuAction();
        yield return fadeout();

        isContinue = false;
        yield break;
    }
    protected override bool isComplete { get { return false; } }

    IEnumerator mainMenuAction()
    {
        mainMenus.Add(new MenuState(goNextQuest, "依頼選択"));
        mainMenus.Add(new MenuState(manageShip, "機体整備"));
        mainMenus.Add(new MenuState(goExerciseStage, "演習施設"));
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
                pivot: TextAnchor.UpperLeft,
                maxChoices: Configs.Choice.MAX_MENU_CHOICE,
                setMotion: animation,
                initialSelected: oldSelected);

            animation = false;
            oldSelected = selected;
            yield return mainMenus[selected % mainMenus.Count].action(result => endLoop = result);
            deleteChoices(endLoop);
        } while(!endLoop);

        yield break;
    }

    IEnumerator goNextQuest(UnityAction<bool> endMenu)
    {
        transparentPlayer();

        bool animation = true;
        var endLoop = false;
        do
        {
            var questExplanation = new TextsWithWindow();
            int selected = 0;
            var stageList = getChoicesList(sys.stages, stage => stage.challengeable ? stage.displayName : "");
            yield return getChoices(stageList,
                endProcess: result => selected = result,
                selectedProcess: (index, choices) => {
                    questExplanation.selfDestroy();
                    questExplanation = setWindowWithText(setSysText(
                        sys.stages[index].explanation,
                        (choices.upperRight + screenSize / 2) / 2,
                        TextAnchor.UpperCenter
                        ));
                },
                setPosition: menuPosition,
                pivot: TextAnchor.UpperLeft,
                ableCancel: true,
                maxChoices: Configs.Choice.MAX_MENU_CHOICE,
                setMotion: animation);
            animation = false;
            questExplanation.selfDestroy();

            if(selected >= 0)
            {
                yield return getYesOrNo("こちらの依頼を受託しますか", yes => {
                    if(yes)
                    {
                        sys.nextStage = sys.stages[selected];
                        endLoop = true;
                        endMenu(true);
                    }
                });
            }
            else endLoop = true;

            deleteChoices(endLoop);
        } while(!endLoop);

        yield break;
    }

    IEnumerator goExerciseStage(UnityAction<bool> endMenu)
    {
        transparentPlayer();
        yield return getYesOrNo("演習設備を利用しますか？", selectYes => {
            if(selectYes)
            {
                sys.nextStage = exerciseStage;
                endMenu(true);
            }
        });
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
                pivot: TextAnchor.UpperLeft,
                ableCancel: true,
                maxChoices: Configs.Choice.MAX_MENU_CHOICE,
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
                pivot: TextAnchor.UpperLeft,
                ableCancel: true,
                maxChoices: Configs.Choice.MAX_MENU_CHOICE,
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
        var setData = originCoreData?.myself;
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
            pivot: TextAnchor.UpperLeft,
            ableCancel: true,
            maxChoices: Configs.Choice.MAX_MENU_CHOICE,
            setMotion: animation,
            initialSelected: oldSelected);

        endProcess(selected);
        sysPlayer.coreData = originData;
        yield break;
    }
    IEnumerator constructionShip(Ship.CoreData originData, UnityAction<Ship.CoreData> endProcess)
    {
        var resultData = originData?.myself;
        int oldSelected = 0;
        bool animation = true;
        bool endLoop = false;
        do
        {
            sysPlayer.coreData = resultData;
            var choices = new List<string> { "本体選択" };
            choices.Add(resultData != null ? "武装選択" : "");
            choices.Add(resultData?.isCorrect ?? false ? "確定" : "");

            int selected = 0;
            yield return getChoices(choices,
                endProcess: result => selected = result,
                setPosition: menuPosition,
                pivot: TextAnchor.UpperLeft,
                ableCancel: true,
                maxChoices: Configs.Choice.MAX_MENU_CHOICE,
                setMotion: animation,
                initialSelected: oldSelected);

            animation = false;
            switch(selected)
            {
                case 0:
                    yield return constructionShipBody(resultData, data => resultData = data);
                    break;
                case 1:
                    yield return constructionShipWeapon(resultData.weaponSlots, (index, weapon) => resultData.setWeapon(index, weapon));
                    break;
                case 2:
                    endLoop = true;
                    break;
                default:
                    var reset = true;
                    if(!resultData.EqualsValue(originData))
                    {
                        transparentPlayer();
                        yield return getChoices(new List<string> { "取り消し", "取り消さない" },
                            endProcess: result => reset = result == 0,
                            ableCancel: true,
                            maxChoices: Configs.Choice.MAX_MENU_CHOICE,
                            pivot: TextAnchor.MiddleCenter);
                        indicatePlayer();
                        deleteChoices();
                    }
                    Debug.Log($"reset {reset}");
                    if(reset)
                    {
                        resultData = originData;
                        endLoop = true;
                    }
                    else selected = oldSelected;
                    break;
            }

            oldSelected = selected;
            deleteChoices(endLoop);
        } while(!endLoop);

        sysPlayer.coreData = sys.adoptedShipData;
        endProcess(resultData);
        yield break;
    }
    IEnumerator constructionShipBody(Ship.CoreData originData, UnityAction<Ship.CoreData> endProcess)
    {
        var choices = getChoicesList(sys.possessionShips, ship => ship.displayName);
        choices.Insert(0, originData != null ? originData.displayName : "");

        int selected = 0;
        yield return getChoices(choices,
            endProcess: result => selected = result,
            selectedProcess: (num, c) => sysPlayer.coreData = num == 0 ? originData : sys.possessionShips[num - 1].coreData.setWeapon(),
            setPosition: menuPosition,
            pivot: TextAnchor.UpperLeft,
            ableCancel: true,
            maxChoices: Configs.Choice.MAX_MENU_CHOICE);

        if(selected == 0) endProcess(originData);
        else if(selected >= 0) endProcess(sys.possessionShips[selected - 1].coreData.setWeapon());
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
                pivot: TextAnchor.UpperLeft,
                ableCancel: true,
                maxChoices: Configs.Choice.MAX_MENU_CHOICE,
                setMotion: animation,
                initialSelected: oldSelected);

            animation = false;
            oldSelected = slotNum;
            if(slotNum >= 0)
            {
                int selected = 0;
                var originWeapon = slots[slotNum].entity;
                var choices = getChoicesList(sys.possessionWeapons, weapon => weapon.abbreviation);
                choices.Insert(0, originWeapon != null ? $"{originWeapon.abbreviation} 〇" : "");
                choices.Add("武装解除");

                yield return getChoices(choices,
                    endProcess: result => selected = result,
                    selectedProcess: (num, _) => displayWeaponExplanation(num, slotNum, originWeapon),
                    setPosition: menuPosition,
                    pivot: TextAnchor.UpperLeft,
                    ableCancel: true,
                    maxChoices: Configs.Choice.MAX_MENU_CHOICE,
                    initialSelected: originWeapon != null ? 0 : choices.Count - 1);

                if(selected > sys.possessionWeapons.Count) endProcess(slotNum, null);
                else if(selected > 0) endProcess(slotNum, sys.possessionWeapons[selected - 1]);
                deleteWeaponExplanation();
                deleteChoices();
            }
            else endLoop = true;

            deleteChoices(endLoop);
        } while(!endLoop);
        yield break;
    }
    /// <summary>
    /// 武装選択中の説明文言表示
    /// </summary>
    /// <param name="selected">今現在選ばれてる番号</param>
    void displayWeaponExplanation(int selected, int slotNum, Weapon origin)
    {
        var setWeapon = selected == 0
        ? origin
        : selected - 1 < sys.possessionWeapons.Count
        ? sys.possessionWeapons[selected - 1]
        : null;
        sysPlayer.setWeapon(slotNum, setWeapon);
        deleteWeaponExplanation();
        if(setWeapon != null)
        {
            var setPosition = -viewSize
                .scaling(baseMas)
                .rescaling(new Vector2(3, 6));
            var nameText = setSysText(setWeapon.displayName, setPosition, pivot: TextAnchor.LowerLeft, charSize: Configs.DEFAULT_TEXT_SIZE + 1);
            var explanationText = setSysText(setWeapon.explanation, setPosition, pivot: TextAnchor.UpperLeft);
            weaponNameWindow = setWindowWithText(nameText, 0);
            weaponExplanationWindow = setWindowWithText(explanationText);
        }
    }
    /// <summary>
    /// 武装説明文の消去
    /// </summary>
    void deleteWeaponExplanation()
    {
        weaponNameWindow?.selfDestroy(false);
        weaponExplanationWindow?.selfDestroy();
    }
    TextsWithWindow weaponNameWindow = null;
    TextsWithWindow weaponExplanationWindow = null;

    IEnumerator config(UnityAction<bool> endMenu)
    {
        transparentPlayer();

        var keepVolumeBGM = Configs.Volume.bgm;
        var keepVolumeSE = Configs.Volume.se;
        var keepAimingMethod = Configs.AimingMethod;

        var counfigMenus = new List<string> { "背景音 音量", "効果音 音量", "照準操作" };
        int selected = 0;
        yield return getChoices(counfigMenus,
            endProcess: result => selected = result,
            selectedProcess: (i, c) => configChoiceAction(i, configPosition(c)),
            horizontalProcess: (i, h, f, c) => configHorizontalAction(i, h, f, configPosition(c)),
            horizontalBarrage: true,
            horizontalInterval: 1,
            setPosition: menuPosition,
            pivot: TextAnchor.UpperLeft,
            ableCancel: true,
            maxChoices: Configs.Choice.MAX_MENU_CHOICE);

        if(selected < 0)
        {
            Configs.Volume.bgm = keepVolumeBGM;
            Configs.Volume.se = keepVolumeSE;
            Configs.AimingMethod = keepAimingMethod;
        }

        configChoiceAction(-1, Vector2.zero);
        deleteChoices();
        yield break;
    }
    Vector2 configPosition(TextsWithWindow data)
    {
        var diff = (data.upperRight - data.underLeft).correct(data.textArea);
        return data.upperRight + diff.scaling(new Vector2(1, -1));
    }
    void configChoiceAction(int selected, Vector2 setPosition)
    {
        const string configTextName = "configs";
        var configText = "";
        switch(selected)
        {
            case 0:
                configText = $"音量\r\n{Configs.Volume.bgm}";
                break;
            case 1:
                configText = $"音量\r\n{Configs.Volume.se}";
                break;
            case 2:
                switch(Configs.AimingMethod)
                {
                    case Configs.AimingOperationOption.WSAD:
                        configText = @"WSAD
サブ十字キー（初期設定WSAD）により照準を操作します。
自在な操作が可能ですが操作難度は高めとなります。";
                        break;
                    case Configs.AimingOperationOption.SHIFT:
                        configText = @"十字キー
低速時（初期設定Shift押下時）、十字キーにより照準操作が可能となります。
操作難度は比較的低くなりますが、移動しながらの照準操作が困難となります。";
                        break;
                    case Configs.AimingOperationOption.COMBINED:
                        configText = @"併用
WSADと十字キーによる照準操作の併用です。
通常時はサブ十字キー、低速時は十字キーとサブ十字キー両方が照準操作に対応します。";
                        break;
                    default:
                        break;
                }
                break;
            default:
                deleteSysText(configTextName);
                break;
        }
        if(selected >= 0) setSysText(configText, setPosition, pivot: TextAnchor.UpperLeft, textName: configTextName);
    }
    void configHorizontalAction(int selected, bool horizontal, bool first, Vector2 setVector)
    {
        var diff = (horizontal ? 1 : -1);
        switch(selected)
        {
            case 0:
                Configs.Volume.bgm = Mathf.Clamp(Configs.Volume.bgm + diff, Configs.Volume.MIN, Configs.Volume.MAX);
                break;
            case 1:
                Configs.Volume.se = Mathf.Clamp(Configs.Volume.se + diff, Configs.Volume.MIN, Configs.Volume.MAX);
                break;
            case 2:
                if(!first) break;
                var length = Enums<Configs.AimingOperationOption>.length;
                var added = (int)Configs.AimingMethod + length + diff;
                Configs.AimingMethod = (Configs.AimingOperationOption)(added % length);
                break;
            default:
                break;
        }
        configChoiceAction(selected, setVector);
    }
}
