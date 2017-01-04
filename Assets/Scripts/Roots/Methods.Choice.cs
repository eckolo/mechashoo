using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using System.Linq;
using UnityEngine.UI;

public partial class Methods : MonoBehaviour
{
    /// <summary>
    /// 選択肢表示名生成
    /// </summary>
    protected static string choiceTextName(int index)
    {
        return "choices" + _choicesDataList.ToArray().Length + "-" + index;
    }
    private static Stack<TextsWithWindow> _choicesDataList = new Stack<TextsWithWindow>();
    protected static TextsWithWindow nowChoicesData
    {
        get
        {
            if(_choicesDataList.ToArray().Length <= 0) return null;
            return _choicesDataList.Peek();
        }
    }
    protected void deleteChoices(bool setMotion = true)
    {
        _choicesDataList.Pop().selfDestroy(setMotion, true);
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
        TextAnchor pibot = TextAnchor.UpperCenter,
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
            foreach(var choice in choices)
                Debug.Log(choice);
            _choicesDataList.Push(choicesData);
            endProcess(lastSelected);
            yield break;
        }

        int selectNum = Mathf.Clamp(choiceNums.IndexOf(initialSelected), 0, choiceNums.Count - 1);
        int firstDisplaied = selectNum;
        int choiceableCount = Mathf.Min(maxChoices ?? choiceNums.Count, choiceNums.Count);

        int baseTextSize = textSize ?? DEFAULT_TEXT_SIZE;
        var monoHeight = baseTextSize * 1.5f;

        var maxWidth = choiceNums
            .Select((value, i) => "\t" + choices[value] + "\t")
            .Select(value => getTextWidth(value))
            .Max();
        var windowSize = new Vector2(maxWidth + baseTextSize, monoHeight * (choiceableCount + 1));
        var textHeight = monoHeight * (choiceableCount - 1);

        Vector2 windowPosition = (setPosition ?? Vector2.zero)
            - MathV.scaling(getAxis(pibot, TextAnchor.MiddleCenter), windowSize);
        Vector2 textBasePosition = windowPosition
            - Vector2.right * maxWidth / 2
            + Vector2.up * textHeight / 2;

        Window backWindow = setWindow(windowPosition, setMotion ? Choice.WINDOW_MOTION_TIME : 0, system: true);
        choicesData.backWindow = backWindow;

        yield return wait(1, system: true);

        bool toDecision = false;
        bool toCancel = false;
        long horizontalCount = 0;
        int keepKeyVertical = 0;
        int oldSelectNum = -1;
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
            choicesData.texts = texts;
            backWindow.size = Vector2.right * windowSize.x / baseMas.x
                + Vector2.up * windowSize.y / baseMas.y;

            if(oldSelectNum != selectNum && selectedProcess != null) selectedProcess(choiceNums[selectNum], choicesData);
            oldSelectNum = selectNum;

            bool inputUpKey = false;
            bool inputDownKey = false;
            bool? inputHorizontalKey = null;
            bool inputHorizontalFirst = false;

            KeyCode? inputKey = null;
            bool firstKey = false;
            var ableKeyList = new List<KeyCode> {
                Buttom.Z,
                Buttom.Up,
                Buttom.Down
            };
            if(ableCancel)
            {
                ableKeyList.Add(Buttom.X);
                ableKeyList.Add(Buttom.Esc);
            }
            if(horizontalProcess != null)
            {
                ableKeyList.Add(Buttom.Right);
                ableKeyList.Add(Buttom.Left);
            }

            yield return waitKey(ableKeyList, (key, first) => {
                inputKey = key;
                firstKey = first;
            }, isSystem: true);

            toDecision = inputKey == Buttom.Z && firstKey;
            toCancel = (inputKey == Buttom.X || inputKey == Buttom.Esc) && firstKey;

            if(toDecision) soundSE(sys.decisionSE, Choice.DECISION_SE_VORUME, isSystem: true);
            if(toCancel) soundSE(sys.cancelSE, Choice.CANCEL_SE_VORUME, isSystem: true);

            if(inputKey == Buttom.Up || inputKey == Buttom.Down)
            {
                if(firstKey)
                {
                    inputUpKey = inputKey == Buttom.Up;
                    inputDownKey = inputKey == Buttom.Down;
                    keepKeyVertical = 0;
                }
                else
                {
                    if(inputKey == Buttom.Up) keepKeyVertical++;
                    if(inputKey == Buttom.Down) keepKeyVertical--;
                    if(Mathf.Abs(keepKeyVertical) > Choice.KEEP_VERTICAL_LIMIT && keepKeyVertical % Choice.KEEP_VERTICAL_INTERVAL == 0)
                    {
                        inputUpKey = keepKeyVertical > 0;
                        inputDownKey = keepKeyVertical < 0;
                    }
                }
            }
            if(inputKey == Buttom.Right || inputKey == Buttom.Left)
            {
                if(firstKey)
                {
                    horizontalCount = 0;
                    inputHorizontalFirst = true;
                    inputHorizontalKey = inputKey == Buttom.Right;
                }
                else if(horizontalBarrage)
                    inputHorizontalKey = inputKey == Buttom.Right;
            }
            if(inputUpKey || inputDownKey || inputHorizontalKey != null) soundSE(sys.setectingSE, Choice.SETECTING_SE_VORUME, isSystem: true);

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
