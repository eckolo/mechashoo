using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using UnityEngine.Events;
using System.Linq;

public class Menu : Stage
{
    /// <summary>
    /// 演習ステージ
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

    void JudgeMainMenuChoiceable()
    {
        foreach(var menu in mainMenus)
        {
            if(menu.action == GoNextQuest) menu.ableChoice = !sysPlayer.isInitialState;
            if(menu.action == GoExerciseStage) menu.ableChoice = !sysPlayer.isInitialState;
        }
    }

    protected override IEnumerator StageAction()
    {
        menuPosition = screenSize.Scaling(new Vector2(-1, 1)) / 2;
        TransparentPlayer();

        yield return Fadein();
        yield return MainMenuAction();
        yield return Fadeout();

        isContinue = false;
        yield break;
    }
    protected override bool isComplete { get { return false; } }

    IEnumerator MainMenuAction()
    {
        mainMenus.Add(new MenuState(GoNextQuest, "依頼選択"));
        mainMenus.Add(new MenuState(ManageShip, "機体整備"));
        mainMenus.Add(new MenuState(GoExerciseStage, "演習施設"));
        mainMenus.Add(new MenuState(Config, "設定変更"));

        int oldSelected = 0;
        bool animation = true;
        bool endLoop = false;
        do
        {
            VisualizePlayer();
            JudgeMainMenuChoiceable();

            int selected = 0;
            yield return ChoiceAction(GetChoicesList(mainMenus,
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
            DeleteChoices(endLoop);
        } while(!endLoop);

        yield break;
    }

    IEnumerator GoNextQuest(UnityAction<bool> endMenu)
    {
        TransparentPlayer();

        bool animation = true;
        var endLoop = false;
        do
        {
            int selected = 0;
            var stageList = GetChoicesList(sys.stages, stage => stage.challengeable ? stage.displayName : "");
            yield return ChoiceAction(stageList,
                endProcess: result => selected = result,
                selectedProcess: (index, choice) => DisplayExplanation(sys.stages[index], choice),
                setPosition: menuPosition,
                pivot: TextAnchor.UpperLeft,
                ableCancel: true,
                maxChoices: Configs.Choice.MAX_MENU_CHOICE,
                setMotion: animation);
            DeleteExplanation();
            animation = false;

            if(selected >= 0)
            {
                yield return GetYesOrNo("こちらの依頼を受託しますか", yes => {
                    if(yes)
                    {
                        sys.nextStage = sys.stages[selected];
                        endLoop = true;
                        endMenu(true);
                    }
                });
            }
            else endLoop = true;

            DeleteChoices(endLoop);
        } while(!endLoop);

        yield break;
    }
    /// <summary>
    /// ステージの説明文言表示
    /// </summary>
    /// <param name="stage">説明表示対象のオブジェクト</param>
    void DisplayExplanation(Stage stage, TextsWithWindow choice)
    {
        DeleteExplanation();
        if(stage == null) return;

        var titleCharSize = Configs.Texts.CHAR_SIZE + 1;
        var bodyCharSize = Configs.Texts.CHAR_SIZE;
        var setPosition = choice.upperRight + Vector2.down * titleCharSize * 3;

        DisplayTextSet(stage.requester, stage.explanation, setPosition, titleCharSize, bodyCharSize);
    }

    IEnumerator GoExerciseStage(UnityAction<bool> endMenu)
    {
        TransparentPlayer();
        yield return GetYesOrNo("演習設備を利用しますか？", selectYes => {
            if(selectYes)
            {
                sys.nextStage = exerciseStage;
                endMenu(true);
            }
        });
        yield break;
    }

    IEnumerator ManageShip(UnityAction<bool> endMenu)
    {
        int oldSelected = 0;
        bool animation = true;
        bool endLoop = false;
        do
        {
            var shipMenus = new List<string> { "機体設計", "設計書管理" };
            int selected = 0;
            yield return ChoiceAction(shipMenus,
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
                    yield return ManageShipDirect();
                    break;
                case 1:
                    yield return ManageShipBlueprint();
                    break;
                default:
                    endLoop = true;
                    break;
            }
            DeleteChoices(endLoop);
        } while(!endLoop);

        yield break;
    }
    IEnumerator ManageShipDirect()
    {
        int oldSelected = 0;
        bool animation = true;
        bool endLoop = false;
        do
        {
            VisualizePlayer();
            var shipMenus = new List<string> {
                "組立",
                sys.adoptedShipData != null ? "設計図へ記録" : "",
                sys.shipDataMylist.Count > 0 ? "設計図を反映" : ""
            };
            int selected = 0;
            yield return ChoiceAction(shipMenus,
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
                    yield return ConstructionShip(
                        sys.adoptedShipData,
                        coreData => sys.adoptedShipData = coreData
                        );
                    break;
                case 1:
                    yield return ManageShipBlueprint(sys.adoptedShipData);
                    break;
                case 2:
                    int resultIndex = -1;
                    yield return SelectBlueprint(result => resultIndex = result, oldSelected, createNew: false);
                    if(resultIndex >= 0) sys.adoptedShipData = sys.shipDataMylist[resultIndex];
                    DeleteChoices();
                    break;
                default:
                    endLoop = true;
                    break;
            }
            DeleteChoices(endLoop);
        } while(!endLoop);

        yield break;
    }
    IEnumerator ManageShipBlueprint(Ship.CoreData originCoreData = null)
    {
        int oldSelected = 0;
        var setData = originCoreData?.myself;
        bool animation = true;
        bool endLoop = false;
        do
        {
            int setNum = 0;
            VisualizePlayer();
            yield return SelectBlueprint(result => setNum = result, oldSelected, animation);

            animation = false;
            oldSelected = setNum;
            if(setNum < 0) endLoop = true;
            else
            {
                var originData = setNum < sys.shipDataMylist.Count
                    ? sys.shipDataMylist[setNum]
                    : null;

                if(originCoreData == null) yield return ConstructionShip(originData, coreData => setData = coreData);

                if(setData != null && setData.isCorrect)
                {
                    if(setNum >= sys.shipDataMylist.Count) sys.shipDataMylist.Add(null);
                    int listNum = Mathf.Min(setNum, Mathf.Max(sys.shipDataMylist.Count - 1, 0));
                    sys.shipDataMylist[listNum] = setData;
                }
            }
            DeleteChoices(endLoop);
        } while(!endLoop);

        yield break;
    }
    IEnumerator SelectBlueprint(UnityAction<int> endProcess, int oldSelected = 0, bool animation = true, bool createNew = true)
    {
        var originData = sysPlayer.coreData;
        var dataList = sys.shipDataMylist;
        var choices = GetChoicesList(dataList, "設計図", "番");
        if(createNew) choices.Add("新規設計図作成");

        int selected = 0;
        yield return ChoiceAction(choices,
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
    IEnumerator ConstructionShip(Ship.CoreData originData, UnityAction<Ship.CoreData> endProcess)
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
            yield return ChoiceAction(choices,
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
                    yield return ConstructionShipBody(resultData, data => resultData = data);
                    break;
                case 1:
                    yield return ConstructionShipWeapon(resultData.allWeaponSlots, (index, weapon) => resultData.SetWeapon(index, weapon));
                    break;
                case 2:
                    endLoop = true;
                    break;
                default:
                    var reset = true;
                    if(!resultData.EqualsValue(originData))
                    {
                        TransparentPlayer();
                        yield return ChoiceAction(new List<string> { "取り消し", "取り消さない" },
                            endProcess: result => reset = result == 0,
                            ableCancel: true,
                            maxChoices: Configs.Choice.MAX_MENU_CHOICE,
                            pivot: TextAnchor.MiddleCenter);
                        IndicatePlayer();
                        DeleteChoices();
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
            DeleteChoices(endLoop);
        } while(!endLoop);

        sysPlayer.coreData = sys.adoptedShipData;
        endProcess(resultData);
        yield break;
    }
    /// <summary>
    /// 機体選択
    /// </summary>
    /// <param name="originData">現在の機体</param>
    /// <param name="endProcess">選択動作終了後の処理</param>
    /// <returns>コルーチン</returns>
    IEnumerator ConstructionShipBody(Ship.CoreData originData, UnityAction<Ship.CoreData> endProcess)
    {
        var choices = GetChoicesList(sys.possessionShips, ship => ship.displayName);
        choices.Insert(0, originData != null ? originData.displayName : "");

        int selected = 0;
        yield return ChoiceAction(choices,
            endProcess: result => selected = result,
            selectedProcess: (num, _) => DisplayShipExplanation(num, originData),
            setPosition: menuPosition,
            pivot: TextAnchor.UpperLeft,
            ableCancel: true,
            maxChoices: Configs.Choice.MAX_MENU_CHOICE);

        if(selected == 0) endProcess(originData);
        else if(selected >= 0) endProcess(sys.possessionShips[selected - 1].coreData.SetWeapon());
        DeleteExplanation();
        DeleteChoices();
        yield break;
    }
    /// <summary>
    /// 機体選択中の説明文言表示
    /// </summary>
    /// <param name="selected">今現在選ばれてる選択肢番号</param>
    /// <param name="origin">現在の機体</param>
    void DisplayShipExplanation(int selected, Ship.CoreData origin)
    {
        sysPlayer.coreData = selected == 0 ?
            origin :
            sys.possessionShips[selected - 1].coreData.SetWeapon();
        DisplayExplanation(sysPlayer);
    }
    /// <summary>
    /// 武装選択
    /// </summary>
    /// <param name="slots">対象武装スロットリスト</param>
    /// <param name="_selectProcess">武装選択時の処理</param>
    /// <returns>コルーチン</returns>
    IEnumerator ConstructionShipWeapon(List<Ship.WeaponSlot> slots, UnityAction<int, Weapon> _selectProcess)
    {
        int oldSelected = 0;
        bool animation = true;
        bool endLoop = false;
        do
        {
            var originWeapons = slots.Select(slot => slot.entity).ToList();
            sysPlayer.SetWeapon(originWeapons);

            int slotNum = 0;
            var choiceList = GetChoicesList(slots, "接続孔", "番", slot => !slot.unique);
            yield return ChoiceAction(choiceList,
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
                var choices = GetChoicesList(sys.possessionWeapons, weapon => weapon.abbreviation);
                choices.Insert(0, originWeapon != null ? $"{originWeapon.abbreviation} 〇" : "");
                choices.Add("武装解除");

                yield return ChoiceAction(choices,
                    endProcess: result => selected = result,
                    selectedProcess: (num, _) => DisplayWeaponExplanation(num, slotNum, originWeapon),
                    setPosition: menuPosition,
                    pivot: TextAnchor.UpperLeft,
                    ableCancel: true,
                    maxChoices: Configs.Choice.MAX_MENU_CHOICE,
                    initialSelected: originWeapon != null ? 0 : choices.Count - 1);

                UnityAction<int, Weapon> selectProcess = (index, weapon) => {
                    _selectProcess(index, weapon);
                    slots[index].entity = weapon;
                };
                if(selected > sys.possessionWeapons.Count) selectProcess(slotNum, null);
                else if(selected > 0) selectProcess(slotNum, sys.possessionWeapons[selected - 1]);
                else selectProcess(slotNum, originWeapon);
                DeleteExplanation();
                DeleteChoices();
            }
            else endLoop = true;

            DeleteChoices(endLoop);
        } while(!endLoop);
        yield break;
    }
    /// <summary>
    /// 武装選択中の説明文言表示
    /// </summary>
    /// <param name="selected">今現在選ばれてる選択肢番号</param>
    /// <param name="slotNum">対象武装スロット番号</param>
    /// <param name="origin">現在装備している武装</param>
    void DisplayWeaponExplanation(int selected, int slotNum, Weapon origin)
    {
        var setWeapon = selected == 0
        ? origin
        : selected - 1 < sys.possessionWeapons.Count
        ? sys.possessionWeapons[selected - 1]
        : null;
        sysPlayer.SetWeapon(slotNum, setWeapon);
        DisplayExplanation(setWeapon);
    }
    /// <summary>
    /// 何かしらのオブジェクトの説明文言表示
    /// </summary>
    /// <param name="explanationed">説明表示対象のオブジェクト</param>
    void DisplayExplanation(Materials explanationed)
    {
        DeleteExplanation();
        if(explanationed == null) return;

        var titleCharSize = Configs.Texts.CHAR_SIZE + 2;
        var bodyCharSize = Configs.Texts.CHAR_SIZE + 1;
        var setPosition = -viewSize.Scaling(baseMas).Rescaling(3, 9) + new Vector2(-2, 1) * bodyCharSize;

        DisplayTextSet(explanationed.displayName, explanationed.explanation, setPosition, titleCharSize, bodyCharSize);
    }
    /// <summary>
    /// タイトルと本文のセットの設置
    /// </summary>
    /// <param name="title">タイトル文</param>
    /// <param name="body">本文</param>
    /// <param name="setPosition"></param>
    /// <param name="titleCharSize"></param>
    /// <param name="bodyCharSize"></param>
    void DisplayTextSet(string title, string body, Vector2 setPosition, int titleCharSize, int bodyCharSize)
    {
        var nameText = SetSysText(title, setPosition, pivot: TextAnchor.LowerLeft, charSize: titleCharSize, bold: true);
        var explanationText = SetSysText(body, setPosition, pivot: TextAnchor.UpperLeft, lineSpace: 0.5f, charSize: bodyCharSize);

        objectNameWindow = SetWindowWithText(nameText, 0);
        objectExplanationWindow = SetWindowWithText(explanationText);
    }
    /// <summary>
    /// オブジェクト説明文の消去
    /// </summary>
    void DeleteExplanation()
    {
        objectNameWindow?.DestroyMyself(false);
        objectExplanationWindow?.DestroyMyself();
    }
    /// <summary>
    /// オブジェクト名称ウィンドウ
    /// </summary>
    TextsWithWindow objectNameWindow = null;
    /// <summary>
    /// オブジェクト説明文言ウィンドウ
    /// </summary>
    TextsWithWindow objectExplanationWindow = null;

    IEnumerator Config(UnityAction<bool> endMenu)
    {
        TransparentPlayer();

        var keepVolumeBGM = Configs.Volume.bgm;
        var keepVolumeSE = Configs.Volume.se;
        var keepAimingMethod = Configs.AimingMethod;

        var counfigMenus = new List<string> { "背景音 音量", "効果音 音量", "照準操作" };
        int selected = 0;
        yield return ChoiceAction(counfigMenus,
            endProcess: result => selected = result,
            selectedProcess: (i, c) => ConfigChoiceProcess(i, GetConfigPosition(c)),
            horizontalProcess: (i, h, f, c) => ConfigHorizontalProcess(i, h, f, GetConfigPosition(c)),
            horizontalBarrage: true,
            horizontalInterval: 12,
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

        ConfigChoiceProcess(-1, Vector2.zero);
        DeleteChoices();
        yield break;
    }
    Vector2 GetConfigPosition(TextsWithWindow data)
    {
        var diff = (data.upperRight - data.underLeft).Correct(data.textArea);
        return data.upperRight + diff.Scaling(new Vector2(1, -1));
    }
    void ConfigChoiceProcess(int selected, Vector2 setPosition)
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
                DeleteSysText(configTextName);
                break;
        }
        if(selected >= 0) SetSysText(configText, setPosition, pivot: TextAnchor.UpperLeft, textName: configTextName);
    }
    bool ConfigHorizontalProcess(int selected, bool horizontal, bool first, Vector2 setVector)
    {
        var result = false;
        var diff = (horizontal ? 1 : -1);
        switch(selected)
        {
            case 0:
                Configs.Volume.bgm = Mathf.Clamp(Configs.Volume.bgm + diff, Configs.Volume.MIN, Configs.Volume.MAX);
                result = true;
                break;
            case 1:
                Configs.Volume.se = Mathf.Clamp(Configs.Volume.se + diff, Configs.Volume.MIN, Configs.Volume.MAX);
                result = true;
                break;
            case 2:
                if(!first) break;
                var length = EnumFunctions.GetLength<Configs.AimingOperationOption>();
                var added = (int)Configs.AimingMethod + length + diff;
                Configs.AimingMethod = (Configs.AimingOperationOption)(added % length);
                result = true;
                break;
            default:
                break;
        }
        ConfigChoiceProcess(selected, setVector);
        return result;
    }
}
