using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.UI;

public abstract partial class Methods : MonoBehaviour
{
    /// <summary>
    /// 選択肢表示名生成
    /// </summary>
    protected static string choiceTextName(int index)
    {
        return $"choices{_choicesDataList.ToArray().Length}-{index}";
    }
    private static Stack<TextsWithWindow> _choicesDataList = new Stack<TextsWithWindow>();
    protected static TextsWithWindow nowChoicesData
    {
        get {
            if(_choicesDataList.ToArray().Length <= 0) return null;
            return _choicesDataList.Peek();
        }
    }
    protected void deleteChoices(bool setMotion = true)
    {
        _choicesDataList.Pop().selfDestroy(setMotion, true);
        if(nowChoicesData != null) nowChoicesData.nowAlpha = 1;
        return;
    }
    /// <summary>
    /// 選択肢関数
    /// 結果の値はendProcessで返す
    /// </summary>
    protected IEnumerator getChoices(List<string> choices,
        UnityAction<int> endProcess,
        UnityAction<int, TextsWithWindow> selectedProcess = null,
        UnityAction<int, bool, bool, TextsWithWindow> horizontalProcess = null,
        bool horizontalBarrage = false,
        int horizontalInterval = 0,
        Vector2? setPosition = null,
        TextAnchor pivot = TextAnchor.UpperCenter,
        bool ableKeepVertical = true,
        bool ableCancel = false,
        int? maxChoices = null,
        int? textSize = null,
        bool setMotion = true,
        int initialSelected = 0)
    {
        int lastSelected = -1;
        var choicesData = new TextsWithWindow();

        var choiceNums = choices
            .Select((value, index) => index)
            .Where(num => choices[num].Length > 0).ToList();

        if(choices.Count <= 0 || choiceNums.Count <= 0)
        {
            Debug.Log("zero choices");
            foreach(var choice in choices) Debug.Log(choice);
            _choicesDataList.Push(choicesData);
            endProcess(lastSelected);
            yield break;
        }

        int selectNum = Mathf.Clamp(choiceNums.IndexOf(initialSelected), 0, choiceNums.Count - 1);
        int firstDisplaied = selectNum;
        int choiceableCount = Mathf.Min(maxChoices ?? choiceNums.Count, choiceNums.Count);

        int baseTextSize = textSize ?? Configs.Texts.CHAR_SIZE;
        var monoHeight = baseTextSize * 1.5f;

        var maxWidth = choiceNums
            .Select((value, i) => $"\t{choices[value]}\t")
            .Select(value => getTextWidth(value))
            .Max();
        var windowSize = new Vector2(maxWidth + baseTextSize, monoHeight * (choiceableCount + 1));
        var textHeight = monoHeight * (choiceableCount - 1);

        foreach(var _choicesData in _choicesDataList) _choicesData.nowAlpha = 0.5f;

        Vector2 windowPosition = (setPosition ?? Vector2.zero)
            - pivot.getAxis(TextAnchor.MiddleCenter).scaling(windowSize);
        Vector2 textBasePosition = windowPosition
            - Vector2.right * maxWidth / 2
            + Vector2.up * textHeight / 2;

        Window backWindow = setWindow(windowPosition, setMotion ? Configs.Choice.WINDOW_MOTION_TIME : 0, system: true);
        choicesData.backWindow = backWindow;

        yield return wait(1, isSystem: true);

        bool toDecision = false;
        bool toCancel = false;
        long horizontalCount = 0;
        int keepKeyVertical = 0;
        int oldSelectNum = -1;
        Text upperMargin = setSysText("↑");
        Text lowerMargin = setSysText("↓");
        while(!toDecision && !toCancel)
        {
            selectNum %= choiceNums.Count;

            firstDisplaied = Mathf.Clamp(firstDisplaied,
                Mathf.Max(selectNum + 1 - choiceableCount, 0),
                Mathf.Min(selectNum, choiceNums.Count - choiceableCount));
            var endDisplaied = firstDisplaied + choiceableCount;

            var texts = new List<Text>();
            for(int i = firstDisplaied; i < endDisplaied; i++)
            {
                var index = i - firstDisplaied;
                var choice = (i == selectNum ? ">\t" : "\t") + choices[choiceNums[i]];
                var nowPosition = textBasePosition + Vector2.down * monoHeight * index;
                var text = setSysText(choice, nowPosition, TextAnchor.MiddleLeft, baseTextSize, TextAnchor.MiddleLeft, textName: choiceTextName(index));
                texts.Add(text);
            }
            upperMargin.setAlpha((firstDisplaied != 0).toInt());
            lowerMargin.setAlpha((endDisplaied != choiceNums.Count).toInt());
            var marginPosition = textBasePosition + Vector2.down * monoHeight * -0.5f;
            upperMargin.setPosition(marginPosition);
            lowerMargin.setPosition(marginPosition + Vector2.down * monoHeight * choiceableCount);

            choicesData.texts = texts;
            backWindow.nowSize = windowSize.rescaling(baseMas);

            if(oldSelectNum != selectNum && selectedProcess != null) selectedProcess(choiceNums[selectNum], choicesData);
            oldSelectNum = selectNum;

            bool inputUpKey = false;
            bool inputDownKey = false;
            bool? inputHorizontalKey = null;
            bool inputHorizontalFirst = false;

            KeyCode? inputKey = null;
            bool firstKey = false;
            var ableKeys = Key.Set.decide.Concat(Key.Set.vertical);
            if(ableCancel) ableKeys = ableKeys.Concat(Key.Set.cancel);
            if(horizontalProcess != null) ableKeys = ableKeys.Concat(Key.Set.horizontal);

            yield return wait(ableKeys.ToList(), (key, first) => {
                inputKey = key;
                firstKey = first;
            }, isSystem: true);

            toDecision = inputKey.judge(Key.Set.decide) && firstKey;
            toCancel = inputKey.judge(Key.Set.cancel) && firstKey;

            if(toDecision) soundSE(sys.ses.decisionSE, Configs.Choice.DECISION_SE_VORUME, isSystem: true);
            if(toCancel) soundSE(sys.ses.cancelSE, Configs.Choice.CANCEL_SE_VORUME, isSystem: true);

            if(inputKey.judge(Key.Set.vertical))
            {
                if(firstKey)
                {
                    inputUpKey = inputKey.judge(Key.Set.up);
                    inputDownKey = inputKey.judge(Key.Set.down);
                    keepKeyVertical = 0;
                }
                else
                {
                    if(inputKey.judge(Key.Set.up)) keepKeyVertical++;
                    if(inputKey.judge(Key.Set.down)) keepKeyVertical--;
                    if(Mathf.Abs(keepKeyVertical) > Configs.Choice.KEEP_VERTICAL_LIMIT && keepKeyVertical % Configs.Choice.KEEP_VERTICAL_INTERVAL == 0)
                    {
                        inputUpKey = keepKeyVertical > 0;
                        inputDownKey = keepKeyVertical < 0;
                    }
                }
            }
            if(inputKey.judge(Key.Set.horizontal))
            {
                if(firstKey)
                {
                    horizontalCount = 0;
                    inputHorizontalFirst = true;
                    inputHorizontalKey = inputKey.judge(Key.Set.right);
                }
                else if(horizontalBarrage)
                {
                    inputHorizontalKey = inputKey.judge(Key.Set.right);
                }
            }
            if(inputUpKey || inputDownKey || inputHorizontalKey != null) soundSE(sys.ses.setectingSE, Configs.Choice.SETECTING_SE_VORUME, isSystem: true);

            if(horizontalProcess != null
                && inputHorizontalKey != null
                && horizontalCount++ == 0)
                horizontalProcess(choiceNums[selectNum], (bool)inputHorizontalKey, inputHorizontalFirst, choicesData);
            horizontalCount %= (horizontalInterval + 1);

            if(inputDownKey) selectNum += 1;
            if(inputUpKey) selectNum += choiceNums.Count - 1;
            if(toCancel) selectNum = -1;
        }
        _choicesDataList.Push(choicesData);
        Destroy(upperMargin.gameObject);
        Destroy(lowerMargin.gameObject);

        lastSelected = selectNum >= 0 ? choiceNums[selectNum] : -1;
        endProcess(lastSelected);
        yield break;
    }

    protected static List<string> getChoicesList<Type>(List<Type> things, System.Func<Type, string> nameMethod)
    {
        return things.Select(nameMethod).ToList();
    }
    protected static List<string> getChoicesList<Type>(List<Type> things, string prefix, string suffix = "")
    {
        return getChoicesList(things.Select((value, index) => index).ToList(), i => prefix + (i + 1) + suffix);
    }
}
