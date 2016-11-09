using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Events;
using System.Linq;

public partial class Methods : MonoBehaviour
{
    /// <summary>
    /// 選択肢表示名生成
    /// </summary>
    protected static string choiceTextName(int index)
    {
        return "choices" + _choicesDataList.ToArray().Length + "-" + index;
    }
    protected class ChoicesData
    {
        public ChoicesData() { textNames = new List<string>(); }
        public List<string> textNames { get; set; }
        public Window backWindow { get; set; }
        public Vector2 underLeft { get { return backWindow.underLeft; } }
        public Vector2 upperRight { get { return backWindow.upperRight; } }
    }
    private static Stack<ChoicesData> _choicesDataList = new Stack<ChoicesData>();
    protected static ChoicesData nowChoicesData
    {
        get
        {
            if (_choicesDataList.ToArray().Length <= 0) return null;
            return _choicesDataList.Peek();
        }
    }
    protected static IEnumerator deleteChoices()
    {
        var deleteData = _choicesDataList.Pop();
        for (int i = 0; i < deleteData.textNames.Count; i++) deleteSysText(deleteData.textNames[i]);
        deleteWindow(deleteData.backWindow);
        yield break;
    }
    /// <summary>
    /// 選択肢関数
    /// 結果の値はlastSelectに保存
    /// </summary>
    protected static IEnumerator getChoices(List<string> choices,
        UnityAction<int> endProcess,
        UnityAction<int, ChoicesData> selectedProcess = null,
        UnityAction<int, bool, bool, ChoicesData> horizontalProcess = null,
        bool horizontalBarrage = false,
        int horizontalInterval = 0,
        Vector2? setPosition = null,
        TextAnchor pibot = TextAnchor.UpperCenter,
        bool ableKeepVertical = true,
        bool ableCancel = false,
        int? maxChoices = null,
        int? textSize = null,
        int initialSelected = 0)
    {
        const int keepVerticalLimit = 36;
        const int keepVerticalInterval = 6;

        int lastSelected = -1;
        var choicesData = new ChoicesData();

        var choiceNums = choices
            .Select((value, index) => index)
            .Where(num => choices[num].Length > 0).ToList();

        if (choices.Count <= 0 || choiceNums.Count <= 0)
        {
            Debug.Log("zero choices");
            foreach (var choice in choices) Debug.Log(choice);
            _choicesDataList.Push(choicesData);
            endProcess(lastSelected);
            yield break;
        }

        int selectNum = Mathf.Clamp(choiceNums.IndexOf(initialSelected), 0, choiceNums.Count - 1);
        int firstDisplaied = selectNum;
        int choiceableCount = Mathf.Min(maxChoices ?? choiceNums.Count, choiceNums.Count);

        int baseTextSize = textSize ?? defaultTextSize;
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
            + Vector2.right * (screenSize.x - maxWidth) / 2
            + Vector2.up * textHeight / 2;

        Window backWindow = setWindow(windowPosition);
        choicesData.backWindow = backWindow;

        yield return null;

        bool toDecision = false;
        bool toCancel = false;
        long horizontalCount = 0;
        int keepKeyVertical = 0;
        int oldSelectNum = -1;
        while (!toDecision && !toCancel)
        {
            selectNum %= choiceNums.Count;

            firstDisplaied = Mathf.Clamp(firstDisplaied,
                Mathf.Max(selectNum + 1 - choiceableCount, 0),
                Mathf.Min(selectNum, choiceNums.Count - choiceableCount));
            var endDisplaied = firstDisplaied + choiceableCount;

            choicesData.textNames = new List<string>();
            for (int i = firstDisplaied; i < endDisplaied; i++)
            {
                var index = i - firstDisplaied;
                var choice = (i == selectNum ? ">\t" : "\t") + choices[choiceNums[i]];
                var nowPosition = textBasePosition + Vector2.down * monoHeight * index;
                setSysText(choice, choiceTextName(index), nowPosition, baseTextSize, TextAnchor.MiddleLeft);
                choicesData.textNames.Add(choiceTextName(index));
            }
            backWindow.size = Vector2.right * windowSize.x / baseMas.x
                + Vector2.up * windowSize.y / baseMas.y;

            if (oldSelectNum != selectNum && selectedProcess != null) selectedProcess(choiceNums[selectNum], choicesData);
            oldSelectNum = selectNum;

            bool inputUpKey = false;
            bool inputDownKey = false;
            bool? inputHorizontalKey = null;
            bool inputHorizontalFirst = false;

            KeyCode? inputKey = null;
            bool firstKey = false;
            var ableKeyList = new List<KeyCode>();
            ableKeyList.Add(ButtomZ);
            if (ableCancel) ableKeyList.Add(ButtomX);
            ableKeyList.Add(ButtomUp);
            ableKeyList.Add(ButtomDown);
            if (horizontalProcess != null)
            {
                ableKeyList.Add(ButtomRight);
                ableKeyList.Add(ButtomLeft);
            }

            yield return waitKey(ableKeyList, (key, first) =>
            {
                inputKey = key;
                firstKey = first;
            });

            toDecision = inputKey == ButtomZ && firstKey;
            toCancel = inputKey == ButtomX && firstKey;
            if (inputKey == ButtomUp || inputKey == ButtomDown)
            {
                if (firstKey)
                {
                    inputUpKey = inputKey == ButtomUp;
                    inputDownKey = inputKey == ButtomDown;
                    keepKeyVertical = 0;
                }
                else
                {
                    if (inputKey == ButtomUp) keepKeyVertical++;
                    if (inputKey == ButtomDown) keepKeyVertical--;
                    if (Mathf.Abs(keepKeyVertical) > keepVerticalLimit && keepKeyVertical % keepVerticalInterval == 0)
                    {
                        inputUpKey = keepKeyVertical > 0;
                        inputDownKey = keepKeyVertical < 0;
                    }
                }
            }
            if (inputKey == ButtomRight || inputKey == ButtomLeft)
            {
                if (firstKey)
                {
                    horizontalCount = 0;
                    inputHorizontalFirst = true;
                    inputHorizontalKey = inputKey == ButtomRight;
                }
                else if (horizontalBarrage) inputHorizontalKey = inputKey == ButtomRight;
            }

            if (horizontalProcess != null
                && inputHorizontalKey != null
                && horizontalCount++ == 0)
                horizontalProcess(choiceNums[selectNum], (bool)inputHorizontalKey, inputHorizontalFirst, choicesData);
            horizontalCount %= (horizontalInterval + 1);

            if (inputDownKey) selectNum += 1;
            if (inputUpKey) selectNum += choiceNums.Count - 1;
            if (toCancel) selectNum = -1;
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
